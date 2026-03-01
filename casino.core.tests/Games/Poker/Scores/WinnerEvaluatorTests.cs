using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Scores;
using casino.core.tests.Fakes;

namespace casino.core.tests.Games.Poker.Scores;

[TestClass]
public class WinnerEvaluatorTests
{
    [TestMethod]
    public void DeterminerGagnantsParMain_QuandTousLesPlayersSeCouchent_DoitLeverArgumentException()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Deux, Suit.Coeur),
            C(CardRank.Trois, Suit.Carreau),
            C(CardRank.Quatre, Suit.Pique),
            C(CardRank.Cinq, Suit.Trefle),
            C(CardRank.Six, Suit.Coeur)
        );

        var Players = new List<Player>
        {
            CreerHumanPlayer("J1", Hand(C(CardRank.As, Suit.Coeur), C(CardRank.Roi, Suit.Carreau)), TypeGameAction.SeCoucher),
            CreerHumanPlayer("J2", Hand(C(CardRank.Dame, Suit.Coeur), C(CardRank.Valet, Suit.Carreau)), TypeGameAction.SeCoucher)
        };

        // Act + Assert
        Assert.Throws<ArgumentException>(() => WinnerEvaluator.DeterminerGagnantsParMain(Players, cartesCommunes));
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_DoitIgnorerLesPlayersCouches()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Deux, Suit.Coeur),
            C(CardRank.Trois, Suit.Carreau),
            C(CardRank.Quatre, Suit.Pique),
            C(CardRank.Huit, Suit.Trefle),
            C(CardRank.Neuf, Suit.Coeur)
        );

        var j1 = CreerHumanPlayer(
            "J1",
            Hand(C(CardRank.As, Suit.Coeur), C(CardRank.Roi, Suit.Carreau)),
            TypeGameAction.SeCoucher);

        var j2 = CreerHumanPlayer(
            "J2",
            Hand(C(CardRank.Dame, Suit.Coeur), C(CardRank.Valet, Suit.Carreau)),
            TypeGameAction.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DeterminerGagnantsParMain(Players, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j2, gagnants[0], "Ignorer les Players couchés même s'ils auraient une meilleure main.");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_DoitChoisirLaMainLaPlusForte_ParRang()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Deux, Suit.Coeur),
            C(CardRank.Trois, Suit.Carreau),
            C(CardRank.Quatre, Suit.Pique),
            C(CardRank.Cinq, Suit.Trefle),
            C(CardRank.Neuf, Suit.Coeur)
        );

        var j1 = CreerHumanPlayer(
            "J1",
            Hand(C(CardRank.As, Suit.Pique), C(CardRank.Dame, Suit.Coeur)),
            TypeGameAction.Miser);

        var j2 = CreerHumanPlayer(
            "J2",
            Hand(C(CardRank.Roi, Suit.Pique), C(CardRank.Valet, Suit.Carreau)),
            TypeGameAction.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DeterminerGagnantsParMain(Players, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j1, gagnants[0], "Choisir le Player avec la main la plus forte (HandRank le plus élevé).");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandMemeRang_DoitDepartagerParValeur()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Dame, Suit.Coeur),
            C(CardRank.Deux, Suit.Carreau),
            C(CardRank.Cinq, Suit.Pique),
            C(CardRank.Huit, Suit.Trefle),
            C(CardRank.Neuf, Suit.Coeur)
        );

        var j1 = CreerHumanPlayer(
            "J1",
            Hand(C(CardRank.Dame, Suit.Pique), C(CardRank.Trois, Suit.Coeur)),
            TypeGameAction.Miser);

        var j2 = CreerHumanPlayer(
            "J2",
            Hand(C(CardRank.Neuf, Suit.Pique), C(CardRank.Quatre, Suit.Coeur)),
            TypeGameAction.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DeterminerGagnantsParMain(Players, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j1, gagnants[0], "À rang égal, choisir la meilleure valeur (paire de Dames > paire de 9).");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandMemeRangEtMemeValeur_DoitDepartagerParKickers()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.As, Suit.Pique),
            C(CardRank.Sept, Suit.Carreau),
            C(CardRank.Six, Suit.Pique),
            C(CardRank.Cinq, Suit.Trefle),
            C(CardRank.Deux, Suit.Coeur)
        );

        // Paire d'As pour les deux.
        // Kickers : K > Q => J1 gagne.
        var j1 = CreerHumanPlayer(
            "J1",
            Hand(C(CardRank.As, Suit.Coeur), C(CardRank.Roi, Suit.Carreau)),
            TypeGameAction.Miser);

        var j2 = CreerHumanPlayer(
            "J2",
            Hand(C(CardRank.As, Suit.Carreau), C(CardRank.Dame, Suit.Coeur)),
            TypeGameAction.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DeterminerGagnantsParMain(Players, cartesCommunes);

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
            C(CardRank.Dix, Suit.Coeur),
            C(CardRank.Valet, Suit.Carreau),
            C(CardRank.Dame, Suit.Pique),
            C(CardRank.Roi, Suit.Trefle),
            C(CardRank.As, Suit.Coeur)
        );

        var j1 = CreerHumanPlayer("J1", Hand(C(CardRank.Deux, Suit.Pique), C(CardRank.Trois, Suit.Coeur)), TypeGameAction.Miser);
        var j2 = CreerHumanPlayer("J2", Hand(C(CardRank.Quatre, Suit.Pique), C(CardRank.Cinq, Suit.Coeur)), TypeGameAction.Miser);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DeterminerGagnantsParMain(Players, cartesCommunes);

        // Assert
        Assert.HasCount(2, gagnants);
        CollectionAssert.Contains(gagnants.ToList(), j1);
        CollectionAssert.Contains(gagnants.ToList(), j2);
    }

    private static Card C(CardRank rang, Suit couleur)
        => new Card(rang, couleur);

    private static HandCards Hand(Card a, Card b)
        => new HandCards(a, b);

    private static TableCards Communes(Card a, Card b, Card c, Card d, Card e)
        => new TableCards { Flop1 = a, Flop2 = b, Flop3 = c, Turn = d, River = e };

    private static Player CreerHumanPlayer(string name, HandCards main, TypeGameAction lastAction)
    {
        var j = new HumanPlayer(name, 1000);
        j.Hand = main;
        j.LastAction = lastAction; // possible grâce au InternalsVisibleTo dans casino.core

        return j;
    }
}
