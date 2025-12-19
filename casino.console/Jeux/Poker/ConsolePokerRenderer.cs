using casino.console.Jeux.Commons;
using casino.core.Jeux.Poker;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Parties.Phases;
using casino.core.Jeux.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.console.jeux.Poker;

public class ConsolePokerRenderer
{
    public static void RenderTable(PokerGameState state)
    {
        Console.Clear();

        var currentPlayerName = state.JoueurActuel;
        var isHandInProgress = state.Phase != Phase.Showdown.ToString();

        Console.Write("Mise min : ");
        WriteAmount(state.MiseDeDepart);
        Console.Write(" | Pot: ");
        WriteAmount(state.Pot);
        Console.Write(" | Mise actuelle: ");
        WriteAmount(state.MiseActuelle);
        Console.WriteLine();

        Console.Write("Cartes: ");
        WriteCommunityCards(state.CartesCommunes);
        Console.WriteLine("\n");

        foreach (var p in state.Joueurs)
        {
            if (p.EstCouche)
            {
                using (ConsoleColorScope.Foreground(ConsoleColor.DarkGray))
                    RenderPlayerLine(p, currentPlayerName, state, isHandInProgress);
            }
            else
            {
                RenderPlayerLine(p, currentPlayerName, state, isHandInProgress);
            }
        }
    }

    public static void RenderPossibleActions(IReadOnlyList<TypeActionJeu> actions, int minimumBet)
    {
        Console.WriteLine("\nActions possibles :");
        foreach (var a in actions)
        {
            // Afficher la mise minimale pour l'action "Miser"
            if (a == TypeActionJeu.Miser)
            {
                Console.Write($"{(int)a}: {a} (");
                WriteAmount(minimumBet);
                Console.WriteLine(").");
            }
            else // Autres actions sans montant associé
            {
                Console.WriteLine($"{(int)a}: {a}.");
            }
        }
        Console.WriteLine();
    }

    private static void RenderPlayerLine(PokerPlayerState p, string currentPlayerName, PokerGameState state, bool isHandInProgress)
    {
        if (currentPlayerName == p.Nom)
            Console.Write("=> ");

        WritePlayerName(p);

        Console.Write(" (");
        WriteAmount(p.Jetons);
        Console.Write("):");

        var canShowHand =
            p.Main is not null &&
            (p.EstHumain || (!isHandInProgress && !p.EstCouche));

        if (canShowHand)
        {
            Console.Write(" ");
            WriteHand(p.Main!);
            Console.Write($" ({EvaluateurScore.EvaluerScore(p.Main!, state.CartesCommunes)})");
        }

        if (p.DerniereAction != TypeActionJeu.Aucune)
            Console.Write($" [{p.DerniereAction}]");

        if (!isHandInProgress && p.EstGagnant)
        {
            using (ConsoleColorScope.Foreground(ConsoleColor.Green))
                Console.Write(" {GAGNANT}");
        }

        Console.WriteLine();
    }

    // ----------------------------
    // Helpers d'affichage
    // ----------------------------

    private static void WritePlayerName(PokerPlayerState p)
    {
        var color =
            (p.Jetons == 0 || p.EstCouche) ? ConsoleColor.Gray :
            p.EstHumain ? ConsoleColor.Cyan :
            ConsoleColor.DarkRed;

        using (ConsoleColorScope.Foreground(color))
            Console.Write(p.Nom);
    }

    private static void WriteAmount(int amount)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Yellow))
            Console.Write($"{amount}c");
    }

    private static void WriteCard(Carte c)
    {
        var color = (c.Couleur == Couleur.Coeur || c.Couleur == Couleur.Carreau)
            ? ConsoleColor.Red
            : ConsoleColor.Cyan;

        using (ConsoleColorScope.Foreground(color))
            Console.Write(c.ToString());
    }

    private static void WriteHand(CartesMain hand)
    {
        var cards = hand.AsEnumerable().ToList();
        for (int i = 0; i < cards.Count; i++)
        {
            if (i > 0) Console.Write(" ");
            WriteCard(cards[i]);
        }
    }

    private static void WriteCommunityCards(CartesCommunes cc)
    {
        var cards = new List<Carte?> { cc.Flop1, cc.Flop2, cc.Flop3, cc.Turn, cc.River }
            .Where(c => c is not null)
            .Cast<Carte>()
            .ToList();

        for (int i = 0; i < cards.Count; i++)
        {
            if (i > 0) Console.Write(" ");
            WriteCard(cards[i]);
        }
    }
}
