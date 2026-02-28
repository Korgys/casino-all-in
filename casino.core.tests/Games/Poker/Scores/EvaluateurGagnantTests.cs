using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;
using casino.core.Games.Poker.Scores;
using casino.core.tests.Fakes;

namespace casino.core.tests.Games.Poker.Scores;

[TestClass]
public class EvaluateurGagnantTests
{
    [TestMethod]
    public void DeterminerGagnantsParMain_QuandTousLesPlayersSeCouchent_DoitLeverArgumentException()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(RangCarte.Deux, Couleur.Coeur),
            C(RangCarte.Trois, Couleur.Carreau),
            C(RangCarte.Quatre, Couleur.Pique),
            C(RangCarte.Cinq, Couleur.Trefle),
            C(RangCarte.Six, Couleur.Coeur)
        );

        var Players = new List<Player>
        {
            CreerPlayerHumain("J1", Hand(C(RangCarte.As, Couleur.Coeur), C(RangCarte.Roi, Couleur.Carreau)), TypeActionJeu.SeCoucher),
            CreerPlayerHumain("J2", Hand(C(RangCarte.Dame, Couleur.Coeur), C(RangCarte.Valet, Couleur.Carreau)), TypeActionJeu.SeCoucher)
        };

        // Act + Assert
        Assert.Throws<ArgumentException>(() => EvaluateurGagnant.DeterminerGagnantsParMain(Players, cartesCommunes));
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_DoitIgnorerLesPlayersCouches()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(RangCarte.Deux, Couleur.Coeur),
            C(RangCarte.Trois, Couleur.Carreau),
            C(RangCarte.Quatre, Couleur.Pique),
            C(RangCarte.Huit, Couleur.Trefle),
            C(RangCarte.Neuf, Couleur.Coeur)
        );

        var j1 = CreerPlayerHumain(
            "J1",
            Hand(C(RangCarte.As, Couleur.Coeur), C(RangCarte.Roi, Couleur.Carreau)),
            TypeActionJeu.SeCoucher);

        var j2 = CreerPlayerHumain(
            "J2",
            Hand(C(RangCarte.Dame, Couleur.Coeur), C(RangCarte.Valet, Couleur.Carreau)),
            TypeActionJeu.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(Players, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j2, gagnants[0], "Ignorer les Players couchés même s'ils auraient une meilleure main.");
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

        var j1 = CreerPlayerHumain(
            "J1",
            Hand(C(RangCarte.As, Couleur.Pique), C(RangCarte.Dame, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var j2 = CreerPlayerHumain(
            "J2",
            Hand(C(RangCarte.Roi, Couleur.Pique), C(RangCarte.Valet, Couleur.Carreau)),
            TypeActionJeu.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(Players, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j1, gagnants[0], "Choisir le Player avec la main la plus forte (RangMain le plus élevé).");
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

        var j1 = CreerPlayerHumain(
            "J1",
            Hand(C(RangCarte.Dame, Couleur.Pique), C(RangCarte.Trois, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var j2 = CreerPlayerHumain(
            "J2",
            Hand(C(RangCarte.Neuf, Couleur.Pique), C(RangCarte.Quatre, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(Players, cartesCommunes);

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
        var j1 = CreerPlayerHumain(
            "J1",
            Hand(C(RangCarte.As, Couleur.Coeur), C(RangCarte.Roi, Couleur.Carreau)),
            TypeActionJeu.Miser);

        var j2 = CreerPlayerHumain(
            "J2",
            Hand(C(RangCarte.As, Couleur.Carreau), C(RangCarte.Dame, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(Players, cartesCommunes);

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

        var j1 = CreerPlayerHumain("J1", Hand(C(RangCarte.Deux, Couleur.Pique), C(RangCarte.Trois, Couleur.Coeur)), TypeActionJeu.Miser);
        var j2 = CreerPlayerHumain("J2", Hand(C(RangCarte.Quatre, Couleur.Pique), C(RangCarte.Cinq, Couleur.Coeur)), TypeActionJeu.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(Players, cartesCommunes);

        // Assert
        Assert.HasCount(2, gagnants);
        CollectionAssert.Contains(gagnants.ToList(), j1);
        CollectionAssert.Contains(gagnants.ToList(), j2);
    }

    private static Card C(RangCarte rang, Couleur couleur)
        => new Card(rang, couleur);

    private static HandCards Hand(Card a, Card b)
        => new HandCards(a, b);

    private static TableCards Communes(Card a, Card b, Card c, Card d, Card e)
        => new TableCards { Flop1 = a, Flop2 = b, Flop3 = c, Turn = d, River = e };

    private static Player CreerPlayerHumain(string name, HandCards main, TypeActionJeu lastAction)
    {
        var j = new PlayerHumain(name, 1000);
        j.Hand = main;
        j.LastAction = lastAction; // possible grâce au InternalsVisibleTo dans casino.core

        return j;
    }
}
