using casino.console.Games.Commons;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.console.Games.Poker;

/// <summary>
/// Renders the poker game state in the console with colors and formatting.
/// </summary>
public class ConsolePokerRenderer
{
    private readonly Dictionary<string, int> winProbabilityByPlayer = new();
    private string? lastRenderedPhase;

    public void ResetRoundCache()
    {
        winProbabilityByPlayer.Clear();
    }

    public void RenderTable(PokerGameState state)
    {
        ResetRoundCacheIfNewRound(state.Phase);

        Console.Clear();

        var currentPlayerName = state.CurrentPlayer;

        Console.Write("Mise min: ");
        ConsolePokerWriter.WriteAmount(state.StartingBet);
        Console.Write(" | Pot: ");
        ConsolePokerWriter.WriteAmount(state.Pot);
        Console.Write(" | Mise actuelle: ");
        ConsolePokerWriter.WriteAmount(state.CurrentBet);
        Console.WriteLine();

        Console.Write("Cartes: ");
        ConsolePokerWriter.WriteCommunityCards(state.CommunityCards);
        Console.WriteLine("\n");

        foreach (var player in state.Players)
            RenderPlayerLine(player, currentPlayerName, state);
    }

    public static void RenderAvailableActions(IReadOnlyList<PokerTypeAction> actions, int minimumBet)
    {
        Console.Write("\nActions : ");
        foreach (var action in actions)
        {
            if (action == PokerTypeAction.Bet)
            {
                Console.Write($"{(int)action}. {action.ToDisplayString()} (");
                ConsolePokerWriter.WriteAmount(minimumBet);
                Console.Write(")     ");
            }
            else
            {
                Console.Write($"{(int)action}. {action.ToDisplayString()}     ");
            }
        }
        Console.WriteLine();
    }

    private void ResetRoundCacheIfNewRound(string phase)
    {
        var preFlopPhase = Phase.PreFlop.ToString();
        if (phase == preFlopPhase && lastRenderedPhase != preFlopPhase)
        {
            ResetRoundCache();
        }

        lastRenderedPhase = phase;
    }

    private void RenderPlayerLine(PokerPlayerState player, string currentPlayerName, PokerGameState state)
    {
        if (player.IsFolded)
            ConsoleColorScope.Foreground(ConsoleColor.DarkGray);

        if (currentPlayerName == player.Name)
            Console.Write("=> ");

        ConsolePokerWriter.WritePlayerName(player);

        if (player.IsFolded)
        {
            Console.Write($" ({player.Chips}c):");
        }
        else
        {
            Console.Write(" (");
            ConsolePokerWriter.WriteAmount(player.Chips);
            Console.Write("): ");
        }

        bool canShowHand =
            player.Hand is not null &&
            (player.IsHuman || (state.Phase == Phase.Showdown.ToString() && !player.IsFolded));

        if (canShowHand)
        {
            Console.Write(" ");
            ConsolePokerWriter.WriteHand(player.Hand!);
            WriteScoreAndProbabilityOfVictory(player, state, currentPlayerName);
        }

        if (player.LastAction != PokerTypeAction.None)
            Console.Write($" [{player.LastAction.ToDisplayString()}]");

        if (player.IsWinner)
        {
            using (ConsoleColorScope.Foreground(ConsoleColor.Green))
                Console.Write(" {GAGNANT}");
        }

        Console.WriteLine();
        ConsoleColorScope.Foreground(ConsoleColor.White);
    }

    private void WriteScoreAndProbabilityOfVictory(PokerPlayerState player, PokerGameState state, string currentPlayerName)
    {
        var score = ScoreEvaluator.EvaluateScore(player.Hand!, state.CommunityCards);
        Console.Write($" ({score}");

        if (currentPlayerName == player.Name || state.Phase == Phase.Showdown.ToString())
        {
            var probability = CalculateProbabilityOfVictory(player, state);
            if (probability is not null)
            {
                Console.Write($" | {probability.Value:F0}%");
                winProbabilityByPlayer[player.Name] = (int)Math.Round(probability.Value);
            }
        }
        else if (winProbabilityByPlayer.TryGetValue(player.Name, out var previousProbability))
        {
            Console.Write($" | {previousProbability:F0}%");
        }

        Console.Write(")");
    }

    private static double? CalculateProbabilityOfVictory(PokerPlayerState player, PokerGameState state)
    {
        if (player.Hand is null)
            return null;

        var opponents = state.Players.Count(p => !p.IsFolded && p.Name != player.Name);
        if (opponents <= 0)
            return 100d;

        try
        {
            return ProbabilityEvaluator.EstimateWinProbability(
                player.Hand,
                state.CommunityCards,
                opponents,
                simulations: 2000);
        }
        catch
        {
            return null;
        }
    }
}
