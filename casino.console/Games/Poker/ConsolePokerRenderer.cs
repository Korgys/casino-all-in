using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using casino.console.Games.Commons;
using casino.console.Localization;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;

namespace casino.console.Games.Poker;

/// <summary>
/// Provides methods for rendering the state of a poker game to the console, including the table, players, and available
/// actions.
/// </summary>
[ExcludeFromCodeCoverage]
public class ConsolePokerRenderer
{
    private const int PreferredTableWidth = 70;
    private const int LiveRenderSimulationCount = 500;
    private const int DetailedOddsSimulationCount = 2000;

    private static readonly Func<HandCards, TableCards, int, int, double> DefaultEstimateWinProbability =
        static (hand, communityCards, opponents, simulations) =>
            ProbabilityEvaluator.EstimateWinProbability(hand, communityCards, opponents, simulations);

    private static Func<HandCards, TableCards, int, int, double> _estimateWinProbability = DefaultEstimateWinProbability;

    public static void SetEstimateWinProbabilityForTests(Func<HandCards, TableCards, int, int, double> estimateWinProbability)
    {
        ArgumentNullException.ThrowIfNull(estimateWinProbability);
        _estimateWinProbability = estimateWinProbability;
    }

    public static void ResetEstimateWinProbability()
    {
        _estimateWinProbability = DefaultEstimateWinProbability;
    }

    private readonly Dictionary<string, ProbabilityCacheEntry> winProbabilityByPlayer = new();
    private readonly Dictionary<string, PendingProbabilityEntry> pendingProbabilityByPlayer = new();
    private string? lastRenderedPhase;

    public bool DetailedOddsEnabled { get; set; }
    public bool UseAsyncProbabilityComputation { get; set; }

    public void ResetRoundCache()
    {
        winProbabilityByPlayer.Clear();
        pendingProbabilityByPlayer.Clear();
    }

    /// <summary>
    /// Renders the poker table and all players using adaptive frame width rules.
    /// </summary>
    /// <param name="state">The current poker game state.</param>
    public void RenderTable(PokerGameState state)
    {
        Console.Clear();
        ResetRoundCacheIfNewRound(state.Phase);

        Console.SetCursorPosition(0, 0);

        var tableWidth = ConsoleLayout.ResolveContentWidth(PreferredTableWidth);

        ConsoleLayout.WriteTopBorder(tableWidth);

        ConsoleLayout.WriteFramedLine(BuildHeaderLine(state), tableWidth);
        ConsoleLayout.WriteSeparator(tableWidth);
        ConsoleLayout.WriteFramedLine(BuildTableLine(state), tableWidth);

        ConsoleLayout.WriteBottomBorder(tableWidth);
        Console.WriteLine();

        foreach (var player in state.Players)
            RenderPlayerLine(player, state.CurrentPlayer, state);
    }

    /// <summary>
    /// Renders the list of available actions in a framed block adapted to the current console width.
    /// </summary>
    /// <param name="actions">The actions available to the current player.</param>
    /// <param name="minimumBet">The minimum bet amount to display for the bet action.</param>
    public static void RenderAvailableActions(IReadOnlyList<PokerTypeAction> actions, int minimumBet)
    {
        Console.WriteLine();
        var tableWidth = ConsoleLayout.ResolveContentWidth(PreferredTableWidth);

        Console.WriteLine("┌" + new string('─', tableWidth) + "┐");

        foreach (var action in actions)
        {
            var label = $"{(int)action}. {action.ToDisplayString()}";

            if (action == PokerTypeAction.Bet)
                label += $" ({minimumBet}c)";

            ConsoleLayout.WriteFramedLine(label, tableWidth, '│', '│');
        }

        Console.WriteLine("└" + new string('─', tableWidth) + "┘");
    }

    private void RenderPlayerLine(PokerPlayerState player, string currentPlayerName, PokerGameState state)
    {
        var isCurrent = currentPlayerName == player.Name;

        if (player.IsFolded)
            ConsoleColorScope.Foreground(ConsoleColor.DarkGray);

        Console.Write(isCurrent ? "▶ " : "  ");

        ConsolePokerWriter.WritePlayerName(player);

        Console.Write(" (");
        ConsolePokerWriter.WriteAmount(player.Chips);
        Console.Write("): ");

        bool canShowHand =
            player.Hand is not null &&
            (player.IsHuman || (state.Phase == Phase.Showdown.ToString() && !player.IsFolded));

        if (canShowHand)
        {
            Console.Write(" ");
            ConsolePokerWriter.WriteHand(player.Hand!);
            WriteScoreAndProbability(player, state, currentPlayerName);
        }

        if (player.LastAction != PokerTypeAction.None)
            Console.Write($" [{player.LastAction.ToDisplayString()}]");

        if (player.IsWinner)
        {
            using (ConsoleColorScope.Foreground(ConsoleColor.Green))
                Console.Write($" {{{ConsoleText.WinnerTag.ToUpperInvariant()}}}");
        }

        Console.WriteLine();
        Console.ResetColor();
    }

    private void WriteScoreAndProbability(PokerPlayerState player, PokerGameState state, string currentPlayerName)
    {
        var score = ScoreEvaluator.EvaluateScore(player.Hand!, state.CommunityCards);
        Console.Write($" ({score}");

        if (currentPlayerName == player.Name || state.Phase == Phase.Showdown.ToString())
        {
            WriteCurrentProbability(player, state);
        }
        else if (winProbabilityByPlayer.TryGetValue(player.Name, out var previous))
        {
            Console.Write($" | {previous.Probability}%");
        }

        Console.Write(")");
    }

    private void WriteCurrentProbability(PokerPlayerState player, PokerGameState state)
    {
        var probabilityKey = BuildProbabilityKey(player, state);
        if (probabilityKey is null)
            return;

        if (winProbabilityByPlayer.TryGetValue(player.Name, out var cached) && cached.Key == probabilityKey)
        {
            Console.Write($" | {cached.Probability}%");
            return;
        }

        if (UseAsyncProbabilityComputation)
        {
            if (!pendingProbabilityByPlayer.TryGetValue(player.Name, out var pending) || pending.Key != probabilityKey)
            {
                pending = new PendingProbabilityEntry(
                    probabilityKey,
                    Task.Run(() => CalculateProbability(player, state, probabilityKey.Opponents)));
                pendingProbabilityByPlayer[player.Name] = pending;
            }

            if (!pending.Task.IsCompleted)
            {
                Console.Write(" | ...");
                return;
            }

            var probability = pending.Task.Result;
            pendingProbabilityByPlayer.Remove(player.Name);
            if (probability is null)
                return;

            var roundedProbability = (int)Math.Round(probability.Value);
            winProbabilityByPlayer[player.Name] = new ProbabilityCacheEntry(probabilityKey, roundedProbability);
            Console.Write($" | {roundedProbability}%");
            return;
        }

        var computed = CalculateProbability(player, state, probabilityKey.Opponents);
        if (computed is null)
            return;

        var rounded = (int)Math.Round(computed.Value);
        winProbabilityByPlayer[player.Name] = new ProbabilityCacheEntry(probabilityKey, rounded);
        Console.Write($" | {rounded}%");
    }

    private static ProbabilityCacheKey? BuildProbabilityKey(PokerPlayerState player, PokerGameState state)
    {
        if (player.Hand is null)
            return null;

        var opponents = state.Players.Count(p => !p.IsFolded && p.Name != player.Name);
        return new ProbabilityCacheKey(player.Hand.ToString(), state.CommunityCards.ToString(), opponents, state.Phase);
    }

    private double? CalculateProbability(PokerPlayerState player, PokerGameState state, int opponents)
    {
        if (player.Hand is null)
            return null;

        if (opponents <= 0)
            return 100d;

        try
        {
            var simulationCount = DetailedOddsEnabled ? DetailedOddsSimulationCount : LiveRenderSimulationCount;
            return _estimateWinProbability(player.Hand, state.CommunityCards, opponents, simulationCount);
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Error computing probability: {ex}");
            return null;
        }
    }

    private void ResetRoundCacheIfNewRound(string phase)
    {
        var preFlop = Phase.PreFlop.ToString();

        if (phase == preFlop && lastRenderedPhase != preFlop)
            ResetRoundCache();

        lastRenderedPhase = phase;
    }

    private sealed record ProbabilityCacheEntry(ProbabilityCacheKey Key, int Probability);
    private sealed record ProbabilityCacheKey(string Hand, string CommunityCards, int Opponents, string Phase);
    private sealed record PendingProbabilityEntry(ProbabilityCacheKey Key, Task<double?> Task);

    /// <summary>
    /// Builds the header content line for the poker table frame.
    /// </summary>
    /// <param name="state">The current poker game state.</param>
    /// <returns>The formatted header line.</returns>
    private static string BuildHeaderLine(PokerGameState state)
    {
        return $" {ConsoleText.PotLabel}: {state.Pot}c | {ConsoleText.BetLabel}: {state.StartingBet}c | {ConsoleText.CurrentBetLabel}: {state.CurrentBet}c ";
    }

    /// <summary>
    /// Builds the community cards content line for the poker table frame.
    /// </summary>
    /// <param name="state">The current poker game state.</param>
    /// <returns>The formatted table line.</returns>
    private static string BuildTableLine(PokerGameState state)
    {
        return $" {ConsoleText.TableLabel}: {FormatCards(state.CommunityCards)} ";
    }

    /// <summary>
    /// Formats visible community cards for inline frame rendering.
    /// </summary>
    /// <param name="tableCards">The community cards container.</param>
    /// <returns>A space-separated card string.</returns>
    private static string FormatCards(TableCards tableCards)
    {
        var cards = new List<Card?>
        {
            tableCards.Flop1,
            tableCards.Flop2,
            tableCards.Flop3,
            tableCards.Turn,
            tableCards.River
        }
        .Where(c => c is not null)
        .Cast<Card>();

        return string.Join(" ", cards.Select(ConsolePokerWriter.FormatCard));
    }
}
