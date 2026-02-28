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
    private static Dictionary<string, int> probabiliteVictoireParJoueur = new();

    public static void RenderTable(PokerGameState state)
    {
        Console.Clear();

        var currentPlayerName = state.JoueurActuel;

        Console.Write("Mise min: ");
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
            RenderPlayerLine(p, currentPlayerName, state);
    }

    public static void RenderPossibleActions(IReadOnlyList<TypeActionJeu> actions, int minimumBet)
    {
        Console.Write("\nActions : ");
        foreach (var a in actions)
        {
            // Afficher la mise minimale pour l'action "Miser"
            if (a == TypeActionJeu.Miser)
            {
                Console.Write($"{(int)a}. {a} (");
                WriteAmount(minimumBet);
                Console.Write(")     ");
            }
            else // Autres actions sans montant associé
            {
                Console.Write($"{(int)a}. {a}     ");
            }
        }
        Console.WriteLine();
    }

    private static void RenderPlayerLine(PokerPlayerState p, string joueurActuelNom, PokerGameState state)
    {
        if (p.EstCouche)
            ConsoleColorScope.Foreground(ConsoleColor.DarkGray);

        if (joueurActuelNom == p.Nom)
            Console.Write("=> ");

        WritePlayerName(p);

        if (p.EstCouche)
        {
            Console.Write($" ({p.Jetons}c):");
        }
        else
        {
            Console.Write(" (");
            WriteAmount(p.Jetons);
            Console.Write("):");
        }

        bool canShowHand =
            p.Main is not null &&
            (p.EstHumain || (state.Phase == Phase.Showdown.ToString() && !p.EstCouche));

        if (canShowHand)
        {
            Console.Write(" ");
            WriteHand(p.Main!);
            EcrireScoreAndProbabiliteVictoire(p, state, joueurActuelNom);
        }

        if (p.DerniereAction != TypeActionJeu.Aucune)
            Console.Write($" [{p.DerniereAction}]");

        if (p.EstGagnant)
        {
            using (ConsoleColorScope.Foreground(ConsoleColor.Green))
                Console.Write(" {GAGNANT}");
        }

        Console.WriteLine();
        ConsoleColorScope.Foreground(ConsoleColor.White);
    }

    private static void EcrireScoreAndProbabiliteVictoire(PokerPlayerState joueur, PokerGameState state, string joueurActuelNom)
    {
        var score = EvaluateurScore.EvaluerScore(joueur.Main!, state.CartesCommunes);
        Console.Write($" ({score}");

        // Calcule la probabilité de victoire uniquement pour le joueur actuel ou si fin de partie
        if (joueurActuelNom == joueur.Nom || state.Phase == Phase.Showdown.ToString())
        {
            var probabilite = CalculerProbabiliteGagner(joueur, state);
            if (probabilite is not null)
            {
                Console.Write($" | {probabilite.Value:F0}%");
                // Mémorise la probabilité pour l'afficher sur les tours des autres joueurs
                probabiliteVictoireParJoueur[joueur.Nom] = (int)Math.Round(probabilite.Value);
            }
        }
        // Sinon, affiche la probabilité précédente si disponible
        else if (probabiliteVictoireParJoueur.TryGetValue(joueur.Nom, out var probabilitePrecedente))
        {
            Console.Write($" | {probabilitePrecedente:F0}%");
        }

        Console.Write(")");
    }

    private static double? CalculerProbabiliteGagner(PokerPlayerState joueur, PokerGameState state)
    {
        if (joueur.Main is null)
            return null;

        // Compter seulement les adversaires encore en jeu.
        var adversaires = state.Joueurs.Count(j => !j.EstCouche && j.Nom != joueur.Nom);
        if (adversaires <= 0)
            return 100d;

        try
        {
            return EvaluateurProbabilite.EstimerProbabiliteDeGagner(
                joueur.Main,
                state.CartesCommunes,
                adversaires,
                simulations: 2000);
        }
        catch
        {
            // L'affichage ne doit jamais interrompre la partie : en cas de problème, on masque la proba.
            return null;
        }
    }

    // ----------------------------
    // Helpers d'affichage
    // ----------------------------

    private static void WritePlayerName(PokerPlayerState p)
    {
        var color =
            (p.EstCouche || (p.Jetons == 0 && p.DerniereAction != TypeActionJeu.Tapis)) ? ConsoleColor.DarkGray :
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
