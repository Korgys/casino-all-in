using System;
using System.Collections.Generic;
using System.Linq;
using casino.core;
using casino.core.Events;
using casino.core.Jeux.Poker;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Parties;
using casino.core.Jeux.Poker.Parties.Phases;
using casino.core.Jeux.Poker.Scores;
using ActionModel = casino.core.Jeux.Poker.Actions.ActionJeu;

namespace casino.console;

public class Program
{
    private static PokerGameState? _dernierEtat;

    public static void Main(string[] args)
    {
        Console.WriteLine("=== Casino All-In ===\n");

        IGameFactory factory = new ConsoleGameFactory();
        var game = factory.Create("poker", ObtenirChoixJoueur, DemanderContinuerNouvellePartie);

        if (game == null)
        {
            Console.WriteLine("Jeu demandé indisponible.");
            return;
        }

        game.StateUpdated += OnStateUpdated;
        game.GameEnded += OnGameEnded;

        game.Run();

        Console.WriteLine("\nAppuyez sur une touche pour quitter...");
    }

    private static void OnStateUpdated(object? sender, GameStateEventArgs e)
    {
        if (e.State is PokerGameState etat)
        {
            _dernierEtat = etat;
            AfficherEtatTable(etat);
        }
    }

    private static void OnGameEnded(object? sender, GameEndedEventArgs e)
    {
        if (_dernierEtat != null)
        {
            AfficherEtatTable(_dernierEtat);
        }

        Console.WriteLine($"\n{e.WinnerName} remporte le pot de {e.Pot}c.");
    }

    private static ActionModel ObtenirChoixJoueur(RequeteAction request)
    {
        var etat = (PokerGameState)request.EtatTable;
        var joueur = etat.Joueurs.First(j => j.Nom == request.JoueurNom);

        int choix = -1;

        while (!request.ActionsPossibles.Any(a => (int)a == choix))
        {
            AfficherActionsPossibles(request.ActionsPossibles, request.MiseMinimum);
            Console.Write("Quel est votre choix ? ");
            int.TryParse(Console.ReadLine(), out choix);
        }

        if ((TypeActionJeu)choix == TypeActionJeu.Miser)
        {
            return new ActionModel((TypeActionJeu)choix, request.MiseMinimum);
        }

        if ((TypeActionJeu)choix == TypeActionJeu.Relancer)
        {
            int mise = 0;
            while (mise == 0 || mise > joueur.Jetons)
            {
                Console.Write("De combien voulez-vous relancer ? ");
                int.TryParse(Console.ReadLine(), out mise);
            }

            return new ActionModel((TypeActionJeu)choix, mise);
        }

        return new ActionModel((TypeActionJeu)choix, 0);
    }

    private static bool DemanderContinuerNouvellePartie()
    {
        Console.Write("\nVoulez-vous continuer une nouvelle partie ? (o/n) : ");
        var reponse = Console.ReadLine();
        return reponse?.ToLower() == "o";
    }

    private static void AfficherActionsPossibles(IReadOnlyList<TypeActionJeu> actionsPossibles, int minimumMise)
    {
        Console.WriteLine("\nActions possibles : ");
        foreach (var actionPossible in actionsPossibles)
        {
            if (actionPossible == TypeActionJeu.Miser)
            {
                Console.Write($"{(int)actionPossible}: {actionPossible} (");
                EcrireMontantCouleur(minimumMise);
                Console.WriteLine(").");
            }
            else Console.WriteLine($"{(int)actionPossible}: {actionPossible}.");
        }
        Console.WriteLine();
    }

    // ----------------------------
    // AFFICHAGE COULEUR
    // ----------------------------

    private static ConsoleColor CouleurPourCarte(Carte c)
    {
        return (c.Couleur == Couleur.Coeur || c.Couleur == Couleur.Carreau)
            ? ConsoleColor.Red
            : ConsoleColor.Cyan;
    }

    private static void AvecCouleur(ConsoleColor color, System.Action action)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        action();
        Console.ForegroundColor = old;
    }

    private static void EcrireCarteCouleur(Carte c)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = CouleurPourCarte(c);
        Console.Write(c.ToString());
        Console.ForegroundColor = old;
    }

    private static void EcrireMainCouleur(CartesMain main)
    {
        var cartes = main.AsEnumerable().ToList();
        for (int i = 0; i < cartes.Count; i++)
        {
            if (i > 0) Console.Write(" ");
            EcrireCarteCouleur(cartes[i]);
        }
    }

    private static void EcrireCartesCommunesCouleur(CartesCommunes cc)
    {
        if (cc.Flop1 != null) { EcrireCarteCouleur(cc.Flop1); Console.Write(" "); }
        if (cc.Flop2 != null) { EcrireCarteCouleur(cc.Flop2); Console.Write(" "); }
        if (cc.Flop3 != null) { EcrireCarteCouleur(cc.Flop3); }
        if (cc.Turn != null) { Console.Write(" "); EcrireCarteCouleur(cc.Turn); }
        if (cc.River != null) { Console.Write(" "); EcrireCarteCouleur(cc.River); }
    }

    private static void EcrireMontantCouleur(int montant)
    {
        AvecCouleur(ConsoleColor.Yellow, () =>
        {
            Console.Write($"{montant}c");
        });
    }

    // ----------------------------
    // ETAT TABLE
    // ----------------------------

    private static void AfficherEtatTable(PokerGameState etat)
    {
        Console.Clear();

        var nomJoueurTour = etat.JoueurActuel;
        var partieEnCours = etat.Phase != Phase.Showdown.ToString();

        Console.Write($"Mise min : ");
        EcrireMontantCouleur(etat.MiseDeDepart);
        Console.Write(" | Pot: ");
        EcrireMontantCouleur(etat.Pot);
        Console.Write(" | Mise actuelle: ");
        EcrireMontantCouleur(etat.MiseActuelle);
        Console.WriteLine();

        Console.Write("Cartes: ");
        EcrireCartesCommunesCouleur(etat.CartesCommunes);
        Console.WriteLine("\n");

        foreach (var joueur in etat.Joueurs)
        {
            if (joueur.EstCouche)
            {
                AvecCouleur(ConsoleColor.DarkGray, () =>
                {
                    AfficherLigneJoueur(joueur, nomJoueurTour, etat, partieEnCours);
                });
            }
            else
            {
                AfficherLigneJoueur(joueur, nomJoueurTour, etat, partieEnCours);
            }
        }
    }

    private static void AfficherLigneJoueur(PokerPlayerState joueur, string nomJoueurTour, PokerGameState etat, bool partieEnCours)
    {
        if (nomJoueurTour == joueur.Nom)
        {
            Console.Write("=> ");
        }

        if (joueur.Jetons == 0 || joueur.EstCouche)
        {
            AvecCouleur(ConsoleColor.Gray, () => Console.Write($"{joueur.Nom}"));
        }
        else if (joueur.EstHumain) AvecCouleur(ConsoleColor.Cyan, () => Console.Write($"{joueur.Nom}"));
        else AvecCouleur(ConsoleColor.DarkRed, () => Console.Write($"{joueur.Nom}"));

        Console.Write(" (");
        EcrireMontantCouleur(joueur.Jetons);
        Console.Write("):");

        if ((joueur.EstHumain || (!partieEnCours && !joueur.EstCouche)) && joueur.Main != null)
        {
            Console.Write(" ");
            EcrireMainCouleur(joueur.Main!);
            Console.Write($" ({EvaluateurScore.EvaluerScore(joueur.Main!, etat.CartesCommunes)})");
        }

        if (joueur.DerniereAction != TypeActionJeu.Aucune)
        {
            Console.Write($" [{joueur.DerniereAction}]");
        }

        if (!partieEnCours && joueur.EstGagnant)
        {
            AvecCouleur(ConsoleColor.Green, () => Console.Write(" {GAGNANT}"));
        }

        Console.WriteLine();
    }
}
