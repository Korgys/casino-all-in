using casino.console.Games.Commons;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace casino.console.Games.Poker;

/// <summary>
/// Provides methods for rendering the state of a poker game to the console, including the table, players, and available
/// actions.
/// </summary>
[ExcludeFromCodeCoverage]
public class ConsolePokerRenderer
{
    private const int TableWidth = 70;

    internal static Func<HandCards, TableCards, int, int, double> EstimateWinProbability =
        static (hand, communityCards, opponents, simulations) =>
            ProbabilityEvaluator.EstimateWinProbability(hand, communityCards, opponents, simulations);

    private readonly Dictionary<string, int> winProbabilityByPlayer = new();
    private string? lastRenderedPhase;

    public void ResetRoundCache()
    {
        winProbabilityByPlayer.Clear();
    }

    public void RenderTable(PokerGameState state)
    {
        Console.Clear();
        ResetRoundCacheIfNewRound(state.Phase);

        Console.SetCursorPosition(0, 0);

        DrawTopBorder();

        WriteLineInFrame(BuildHeaderLine(state));
        DrawSeparator();
        WriteLineInFrame(BuildTableLine(state));

        DrawBottomBorder();
        Console.WriteLine();

        foreach (var player in state.Players)
            RenderPlayerLine(player, state.CurrentPlayer, state);
    }

    public static void RenderAvailableActions(IReadOnlyList<PokerTypeAction> actions, int minimumBet)
    {
        Console.WriteLine();
        Console.WriteLine("┌" + new string('─', TableWidth) + "┐");

        foreach (var action in actions)
        {
            var label = $"{(int)action}. {action.ToDisplayString()}";

            if (action == PokerTypeAction.Bet)
                label += $" ({minimumBet}c)";

            WriteRawLine(label);
        }

        Console.WriteLine("└" + new string('─', TableWidth) + "┘");
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
                Console.Write(" {GAGNANT}");
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
            var probability = CalculateProbability(player, state);
            if (probability is not null)
            {
                Console.Write($" | {probability.Value:F0}%");
                winProbabilityByPlayer[player.Name] = (int)Math.Round(probability.Value);
            }
        }
        else if (winProbabilityByPlayer.TryGetValue(player.Name, out var previous))
        {
            Console.Write($" | {previous}%");
        }

        Console.Write(")");
    }

    private static double? CalculateProbability(PokerPlayerState player, PokerGameState state)
    {
        if (player.Hand is null)
            return null;

        var opponents = state.Players.Count(p => !p.IsFolded && p.Name != player.Name);
        if (opponents <= 0)
            return 100d;

        try
        {
            return EstimateWinProbability(player.Hand, state.CommunityCards, opponents, 2000);
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

    // ================= UI HELPERS =================

    private static void DrawTopBorder()
    {
        Console.WriteLine("╔" + new string('═', TableWidth) + "╗");
    }

    private static void DrawSeparator()
    {
        Console.WriteLine("╠" + new string('═', TableWidth) + "╣");
    }

    private static void DrawBottomBorder()
    {
        Console.WriteLine("╚" + new string('═', TableWidth) + "╝");
    }

    private static void WriteLineInFrame(string content)
    {
        int visibleLength = GetVisibleLength(content);
        int padding = TableWidth - visibleLength;

        if (padding < 0)
            content = content.Substring(0, TableWidth);

        Console.Write("║");
        Console.Write(content);
        Console.Write(new string(' ', Math.Max(0, padding)));
        Console.WriteLine("║");
    }

    private static void WriteRawLine(string content)
    {
        int visibleLength = GetVisibleLength(content);
        int padding = TableWidth - visibleLength;

        Console.Write("│");
        Console.Write(content);
        Console.Write(new string(' ', Math.Max(0, padding)));
        Console.WriteLine("│");
    }

    private static int GetVisibleLength(string text)
    {
        return Regex.Replace(text, @"\x1B\[[0-9;]*m", "").Length;
    }

    private static string BuildHeaderLine(PokerGameState state)
    {
        return $" Pot: {state.Pot}c | Mise min: {state.StartingBet}c | Mise actuelle: {state.CurrentBet}c ";
    }

    private static string BuildTableLine(PokerGameState state)
    {
        return $" Table: {FormatCards(state.CommunityCards)} ";
    }

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