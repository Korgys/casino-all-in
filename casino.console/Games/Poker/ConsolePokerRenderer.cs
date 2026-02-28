using casino.console.Games.Commons;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Parties.Phases;
using casino.core.Games.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.console.Games.Poker;

/// <summary>
/// Render the poker game state in the console, with colors and formatting to enhance readability.
/// </summary>
public class ConsolePokerRenderer
{
    private static Dictionary<string, int> probabiliteVictoireParPlayer = new();

    public static void RenderTable(PokerGameState state)
    {
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

        foreach (var p in state.Players)
            RenderPlayerLine(p, currentPlayerName, state);
    }

    public static void RenderAvailableActions(IReadOnlyList<TypeActionJeu> actions, int minimumBet)
    {
        Console.Write("\nActions : ");
        foreach (var a in actions)
        {
            // Afficher la mise minimale pour l'action "Miser"
            if (a == TypeActionJeu.Miser)
            {
                Console.Write($"{(int)a}. {a} (");
                ConsolePokerWriter.WriteAmount(minimumBet);
                Console.Write(")     ");
            }
            else // Autres actions sans montant associé
            {
                Console.Write($"{(int)a}. {a}     ");
            }
        }
        Console.WriteLine();
    }

    private static void RenderPlayerLine(PokerPlayerState p, string currentPlayerName, PokerGameState state)
    {
        if (p.IsFolded)
            ConsoleColorScope.Foreground(ConsoleColor.DarkGray);

        if (currentPlayerName == p.Name)
            Console.Write("=> ");
        ConsolePokerWriter.WritePlayerName(p);

        if (p.IsFolded)
        {
            Console.Write($" ({p.Chips}c):");
        }
        else
        {
            Console.Write(" (");
            ConsolePokerWriter.WriteAmount(p.Chips);
            Console.Write("): ");
        }

        bool canShowHand =
            p.Hand is not null &&
            (p.IsHuman || (state.Phase == Phase.Showdown.ToString() && !p.IsFolded));

        if (canShowHand)
        {
            Console.Write(" ");
            ConsolePokerWriter.WriteHand(p.Hand!);
            WriteScoreAndProbabilityOfVictory(p, state, currentPlayerName);
        }
        if (p.LastAction != TypeActionJeu.Aucune)
            Console.Write($" [{p.LastAction}]");

        if (p.IsWinner)
        {
            using (ConsoleColorScope.Foreground(ConsoleColor.Green))
                Console.Write(" {GAGNANT}");
        }

        Console.WriteLine();
        ConsoleColorScope.Foreground(ConsoleColor.White);
    }

    private static void WriteScoreAndProbabilityOfVictory(PokerPlayerState player, PokerGameState state, string currentPlayerName)
    {
        var score = EvaluateurScore.EvaluerScore(player.Hand!, state.CommunityCards);
        Console.Write($" ({score}");

        // Calcule la probabilité de victoire uniquement pour le Player actuel ou si fin de partie
        if (currentPlayerName == player.Name || state.Phase == Phase.Showdown.ToString())
        {
            var probabilite = CalculateProbabilityOfVictory(player, state);
            if (probabilite is not null)
            {
                Console.Write($" | {probabilite.Value:F0}%");
                // Mémorise la probabilité pour l'afficher sur les tours des autres Players
                probabiliteVictoireParPlayer[player.Name] = (int)Math.Round(probabilite.Value);
            }
        }
        // Sinon, affiche la probabilité précédente si disponible
        else if (probabiliteVictoireParPlayer.TryGetValue(player.Name, out var probabilitePrecedente))
        {
            Console.Write($" | {probabilitePrecedente:F0}%");
        }

        Console.Write(")");
    }

    private static double? CalculateProbabilityOfVictory(PokerPlayerState player, PokerGameState state)
    {
        if (player.Hand is null)
            return null;

        // Count only active opponents
        var adversaires = state.Players.Count(j => !j.IsFolded && j.Name != player.Name);
        if (adversaires <= 0)
            return 100d;

        try
        {
            return EvaluateurProbabilite.EstimerProbabiliteDeGagner(
                player.Hand,
                state.CommunityCards,
                adversaires,
                simulations: 2000);
        }
        catch
        {
            // L'affichage ne doit jamais interrompre la partie : en cas de problème, on masque la proba.
            return null;
        }
    }
}
