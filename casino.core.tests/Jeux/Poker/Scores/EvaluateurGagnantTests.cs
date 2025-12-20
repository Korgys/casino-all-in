using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using casino.core.Jeux.Poker.Scores;
using casino.core.tests.Fakes;

namespace casino.core.tests.Jeux.Poker.Scores;

[TestClass]
public class EvaluateurGagnantTests
{
    [TestMethod]
    public void DeterminerGagnantsParMain_QuandTousLesJoueursSeCouchent_DoitLeverArgumentException()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(RangCarte.Deux, Couleur.Coeur),
            C(RangCarte.Trois, Couleur.Carreau),
            C(RangCarte.Quatre, Couleur.Pique),
            C(RangCarte.Cinq, Couleur.Trefle),
            C(RangCarte.Six, Couleur.Coeur)
        );

        var joueurs = new List<Joueur>
        {
            CreerJoueurHumain("J1", Main(C(RangCarte.As, Couleur.Coeur), C(RangCarte.Roi, Couleur.Carreau)), TypeActionJeu.SeCoucher),
            CreerJoueurHumain("J2", Main(C(RangCarte.Dame, Couleur.Coeur), C(RangCarte.Valet, Couleur.Carreau)), TypeActionJeu.SeCoucher)
        };

        // Act + Assert
        Assert.Throws<ArgumentException>(() => EvaluateurGagnant.DeterminerGagnantsParMain(joueurs, cartesCommunes));
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_DoitIgnorerLesJoueursCouches()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(RangCarte.Deux, Couleur.Coeur),
            C(RangCarte.Trois, Couleur.Carreau),
            C(RangCarte.Quatre, Couleur.Pique),
            C(RangCarte.Huit, Couleur.Trefle),
            C(RangCarte.Neuf, Couleur.Coeur)
        );

        var j1 = CreerJoueurHumain(
            "J1",
            Main(C(RangCarte.As, Couleur.Coeur), C(RangCarte.Roi, Couleur.Carreau)),
            TypeActionJeu.SeCoucher);

        var j2 = CreerJoueurHumain(
            "J2",
            Main(C(RangCarte.Dame, Couleur.Coeur), C(RangCarte.Valet, Couleur.Carreau)),
            TypeActionJeu.Miser);

        var joueurs = new List<Joueur> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(joueurs, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j2, gagnants[0], "Ignorer les joueurs couchés même s'ils auraient une meilleure main.");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_DoitChoisirLaMainLaPlusForte_ParRang()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(RangCarte.Deux, Couleur.Coeur),
            C(RangCarte.Trois, Couleur.Carreau),
            C(RangCarte.Quatre, Couleur.Pique),
            C(RangCarte.Cinq, Couleur.Trefle),
            C(RangCarte.Neuf, Couleur.Coeur)
        );

        var j1 = CreerJoueurHumain(
            "J1",
            Main(C(RangCarte.As, Couleur.Pique), C(RangCarte.Dame, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var j2 = CreerJoueurHumain(
            "J2",
            Main(C(RangCarte.Roi, Couleur.Pique), C(RangCarte.Valet, Couleur.Carreau)),
            TypeActionJeu.Miser);

        var joueurs = new List<Joueur> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(joueurs, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j1, gagnants[0], "Choisir le joueur avec la main la plus forte (RangMain le plus élevé).");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandMemeRang_DoitDepartagerParValeur()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(RangCarte.Dame, Couleur.Coeur),
            C(RangCarte.Deux, Couleur.Carreau),
            C(RangCarte.Cinq, Couleur.Pique),
            C(RangCarte.Huit, Couleur.Trefle),
            C(RangCarte.Neuf, Couleur.Coeur)
        );

        var j1 = CreerJoueurHumain(
            "J1",
            Main(C(RangCarte.Dame, Couleur.Pique), C(RangCarte.Trois, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var j2 = CreerJoueurHumain(
            "J2",
            Main(C(RangCarte.Neuf, Couleur.Pique), C(RangCarte.Quatre, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var joueurs = new List<Joueur> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(joueurs, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j1, gagnants[0], "À rang égal, choisir la meilleure valeur (paire de Dames > paire de 9).");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandMemeRangEtMemeValeur_DoitDepartagerParKickers()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(RangCarte.As, Couleur.Pique),
            C(RangCarte.Sept, Couleur.Carreau),
            C(RangCarte.Six, Couleur.Pique),
            C(RangCarte.Cinq, Couleur.Trefle),
            C(RangCarte.Deux, Couleur.Coeur)
        );

        // Paire d'As pour les deux.
        // Kickers : K > Q => J1 gagne.
        var j1 = CreerJoueurHumain(
            "J1",
            Main(C(RangCarte.As, Couleur.Coeur), C(RangCarte.Roi, Couleur.Carreau)),
            TypeActionJeu.Miser);

        var j2 = CreerJoueurHumain(
            "J2",
            Main(C(RangCarte.As, Couleur.Carreau), C(RangCarte.Dame, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var joueurs = new List<Joueur> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(joueurs, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j1, gagnants[0], "À rang et valeur égaux, départager par les kickers ordonnés.");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandEgaliteParfaite_DoitRetournerTousLesGagnants()
    {
        // Arrange
        // Board "quinte max" : 10-J-Q-K-A, tout le monde a la même main -> égalité parfaite
        var cartesCommunes = Communes(
            C(RangCarte.Dix, Couleur.Coeur),
            C(RangCarte.Valet, Couleur.Carreau),
            C(RangCarte.Dame, Couleur.Pique),
            C(RangCarte.Roi, Couleur.Trefle),
            C(RangCarte.As, Couleur.Coeur)
        );

        var j1 = CreerJoueurHumain("J1", Main(C(RangCarte.Deux, Couleur.Pique), C(RangCarte.Trois, Couleur.Coeur)), TypeActionJeu.Miser);
        var j2 = CreerJoueurHumain("J2", Main(C(RangCarte.Quatre, Couleur.Pique), C(RangCarte.Cinq, Couleur.Coeur)), TypeActionJeu.Miser);

        var joueurs = new List<Joueur> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(joueurs, cartesCommunes);

        // Assert
        Assert.HasCount(2, gagnants);
        CollectionAssert.Contains(gagnants.ToList(), j1);
        CollectionAssert.Contains(gagnants.ToList(), j2);
    }

    private static Carte C(RangCarte rang, Couleur couleur)
        => new Carte(rang, couleur);

    private static CartesMain Main(Carte a, Carte b)
        => new CartesMain(a, b);

    private static CartesCommunes Communes(Carte a, Carte b, Carte c, Carte d, Carte e)
        => new CartesCommunes { Flop1 = a, Flop2 = b, Flop3 = c, Turn = d, River = e };

    private static Joueur CreerJoueurHumain(string nom, CartesMain main, TypeActionJeu derniereAction)
    {
        var j = new JoueurHumain(nom, 1000);
        j.Main = main;
        j.DerniereAction = derniereAction; // possible grâce au InternalsVisibleTo dans casino.core

        return j;
    }
}
