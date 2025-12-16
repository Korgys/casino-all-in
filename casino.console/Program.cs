using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using casino.core.Jeux.Poker.Parties.Phases;
using casino.core.Jeux.Poker.Scores;
using casino.core.Jeux.Poker.Joueurs.Strategies;
using casino.core.Jeux.Poker.Actions;

namespace casino.console;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Casino All-In ===\n");

        var joueur = new JoueurHumain("Player", 1000);
        var joueurs = new List<Joueur>
        {
            joueur,
            new JoueurOrdi("Ordi Agressif", 1000, new StrategieAgressive()),
            new JoueurOrdi("Ordi Conserv", 1000, new StrategieConservatrice()),
            new JoueurOrdi("Ordi Random", 1000, new StrategieRandom())
        };
        var table = new TablePoker();
        table.Nom = "Table Poker";

        // On joue tant qu'il n'y a des jetons
        while (joueur.Jetons > 0)
        {
            var deck = new JeuDeCartes();
            table.DemarrerPartie(joueurs, deck);

            // Boucle principale de la partie
            while (table.Partie.EnCours())
            {
                AfficherEtatTable(table);

                if (table.ObtenirJoueurQuiDoitJouer() is JoueurHumain)
                {
                    var actionsPossibles = table.ObtenirActionsPossibles(joueur);
                    AfficherActionsPossibles(actionsPossibles, table.Partie.MiseDeDepart);

                    var choix = ObtenirChoixJoueur(joueur, actionsPossibles, table.Partie.MiseDeDepart);
                    table.TraiterActionJoueur(joueur, choix);
                }
                else // joue pour l'ordi
                {
                    Thread.Sleep(Random.Shared.Next(500, 2500)); // pause de quelques secondes
                    var cpu = (JoueurOrdi)table.ObtenirJoueurQuiDoitJouer();
                    var actionsPossibles = table.ObtenirActionsPossibles(cpu);
                    var contexte = new ContexteDeJeu(table.Partie, cpu, actionsPossibles);
                    var joueurAction = cpu.Strategie.ProposerAction(contexte);
                    table.TraiterActionJoueur(cpu, joueurAction);
                }
            }

            AfficherEtatTable(table);
            if (!DemanderContinuerNouvellePartie()) break;
        }

        Console.WriteLine("\nAppuyez sur une touche pour quitter...");
    }

    private static core.Jeux.Poker.Actions.Action ObtenirChoixJoueur(JoueurHumain joueur, List<TypeAction> actionsPossibles, int minimumMise)
    {
        int choix = -1;

        // Récupérer le choix d'action
        while (!actionsPossibles.Any(a => (int)a == choix))
        {
            Console.Write("Quel est votre choix ? ");
            int.TryParse(Console.ReadLine(), out choix);
        }

        // Gestion de la mise
        if (choix == (int)TypeAction.Miser)
        {
            return new core.Jeux.Poker.Actions.Action((TypeAction)choix, minimumMise);
        }
        else if (choix == (int)TypeAction.Relancer)
        {
            int mise = 0;
            while (mise == 0 && mise <= joueur.Jetons)
            {
                Console.Write("De combien voulez-vous relancer ? ");
                int.TryParse(Console.ReadLine(), out mise);
            }
            return new core.Jeux.Poker.Actions.Action((TypeAction)choix, mise);
        }

        return new core.Jeux.Poker.Actions.Action((TypeAction)choix, 0);
    }

    private static void AfficherActionsPossibles(List<TypeAction> actionsPossibles, int minimumMise)
    {
        // Affichage des actions
        Console.WriteLine("\nActions possibles : ");
        foreach (var actionPossible in actionsPossibles)
        {
            if (actionPossible == TypeAction.Miser)
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

    private static void AfficherEtatTable(TablePoker table)
    {
        Console.Clear();

        var p = table.Partie;
        var nomJoueurTour = table.ObtenirJoueurQuiDoitJouer().Nom;

        Console.Write($"Mise min : ");
        EcrireMontantCouleur(p.MiseDeDepart);
        Console.Write(" | Pot: ");
        EcrireMontantCouleur(p.Pot);
        Console.Write(" | Mise actuelle: ");
        EcrireMontantCouleur(p.MiseActuelle);
        Console.WriteLine();

        Console.Write("Cartes: ");
        EcrireCartesCommunesCouleur(p.CartesCommunes);
        Console.WriteLine("\n");

        // Affiche les infos des joueurs
        foreach (Joueur joueur in table.Joueurs)
        {
            bool estCouche = joueur.DerniereAction == TypeAction.SeCoucher;

            if (estCouche)
            {
                AvecCouleur(ConsoleColor.DarkGray, () =>
                {
                    AfficherLigneJoueur(joueur, nomJoueurTour, p);
                });
            }
            else
            {
                AfficherLigneJoueur(joueur, nomJoueurTour, p);
            }
        }
    }

    private static void AfficherLigneJoueur(Joueur joueur, string nomJoueurTour, Partie p)
    {
        if (nomJoueurTour == joueur.Nom)
        {
            Console.Write("=> ");
        }

        if (joueur.Jetons == 0 || joueur.DerniereAction == TypeAction.SeCoucher)
        {
            AvecCouleur(ConsoleColor.Gray, () => Console.Write($"{joueur.Nom}"));
        }
        else if (joueur is JoueurHumain) AvecCouleur(ConsoleColor.Cyan, () => Console.Write($"{joueur.Nom}"));
        else AvecCouleur(ConsoleColor.DarkRed, () => Console.Write($"{joueur.Nom}"));

        Console.Write(" (");
        EcrireMontantCouleur(joueur.Jetons);
        Console.Write("):");

        if (joueur is JoueurHumain || (!p.EnCours() && joueur.DerniereAction != TypeAction.SeCoucher))
        {
            Console.Write(" ");
            EcrireMainCouleur(joueur.Main);
            Console.Write($" ({EvaluateurScore.EvaluerScore(joueur.Main, p.CartesCommunes)})");
        }

        if (joueur.DerniereAction != TypeAction.Aucune)
        {
            Console.Write($" [{joueur.DerniereAction}]");
        }

        if (!p.EnCours() && joueur.Nom == p.Gagnant?.Nom)
        {
            AvecCouleur(ConsoleColor.Green, () => Console.Write(" {GAGNANT}"));
        }

        Console.WriteLine();
    }

    private static bool DemanderContinuerNouvellePartie()
    {
        Console.Write("\nVoulez-vous continuer une nouvelle partie ? (o/n) : ");
        var reponse = Console.ReadLine();
        return reponse?.ToLower() == "o";
    }
}
