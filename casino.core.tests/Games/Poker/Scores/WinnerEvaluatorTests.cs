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
            C(CardRank.Deux, Suit.Hearts),
            C(CardRank.Trois, Suit.Diamonds),
            C(CardRank.Quatre, Suit.Spades),
            C(CardRank.Cinq, Suit.Clubs),
            C(CardRank.Six, Suit.Hearts)
        );

        var Players = new List<Player>
        {
            CreerHumanPlayer("J1", Hand(C(CardRank.As, Suit.Hearts), C(CardRank.Roi, Suit.Diamonds)), PokerTypeAction.Fold),
            CreerHumanPlayer("J2", Hand(C(CardRank.Dame, Suit.Hearts), C(CardRank.Valet, Suit.Diamonds)), PokerTypeAction.Fold)
        };

        // Act + Assert
        Assert.Throws<ArgumentException>(() => WinnerEvaluator.DetermineWinnersByHand(Players, cartesCommunes));
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_DoitIgnorerLesPlayersCouches()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Deux, Suit.Hearts),
            C(CardRank.Trois, Suit.Diamonds),
            C(CardRank.Quatre, Suit.Spades),
            C(CardRank.Huit, Suit.Clubs),
            C(CardRank.Neuf, Suit.Hearts)
        );

        var j1 = CreerHumanPlayer(
            "J1",
            Hand(C(CardRank.As, Suit.Hearts), C(CardRank.Roi, Suit.Diamonds)),
            PokerTypeAction.Fold);

        var j2 = CreerHumanPlayer(
            "J2",
            Hand(C(CardRank.Dame, Suit.Hearts), C(CardRank.Valet, Suit.Diamonds)),
            PokerTypeAction.Bet);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DetermineWinnersByHand(Players, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j2, gagnants[0], "Ignorer les Players couchés même s'ils auraient une meilleure main.");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_DoitChoisirLaMainLaPlusForte_ParRang()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Deux, Suit.Hearts),
            C(CardRank.Trois, Suit.Diamonds),
            C(CardRank.Quatre, Suit.Spades),
            C(CardRank.Cinq, Suit.Clubs),
            C(CardRank.Neuf, Suit.Hearts)
        );

        var j1 = CreerHumanPlayer(
            "J1",
            Hand(C(CardRank.As, Suit.Spades), C(CardRank.Dame, Suit.Hearts)),
            PokerTypeAction.Bet);

        var j2 = CreerHumanPlayer(
            "J2",
            Hand(C(CardRank.Roi, Suit.Spades), C(CardRank.Valet, Suit.Diamonds)),
            PokerTypeAction.Bet);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DetermineWinnersByHand(Players, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j1, gagnants[0], "Choisir le Player avec la main la plus forte (HandRank le plus élevé).");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandMemeRang_DoitDepartagerParValeur()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Dame, Suit.Hearts),
            C(CardRank.Deux, Suit.Diamonds),
            C(CardRank.Cinq, Suit.Spades),
            C(CardRank.Huit, Suit.Clubs),
            C(CardRank.Neuf, Suit.Hearts)
        );

        var j1 = CreerHumanPlayer(
            "J1",
            Hand(C(CardRank.Dame, Suit.Spades), C(CardRank.Trois, Suit.Hearts)),
            PokerTypeAction.Bet);

        var j2 = CreerHumanPlayer(
            "J2",
            Hand(C(CardRank.Neuf, Suit.Spades), C(CardRank.Quatre, Suit.Hearts)),
            PokerTypeAction.Bet);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DetermineWinnersByHand(Players, cartesCommunes);

        // Assert
        Assert.HasCount(1, gagnants);
        Assert.AreSame(j1, gagnants[0], "À rang égal, choisir la meilleure valeur (paire de Dames > paire de 9).");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandMemeRangEtMemeValeur_DoitDepartagerParKickers()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.As, Suit.Spades),
            C(CardRank.Sept, Suit.Diamonds),
            C(CardRank.Six, Suit.Spades),
            C(CardRank.Cinq, Suit.Clubs),
            C(CardRank.Deux, Suit.Hearts)
        );

        // Paire d'As pour les deux.
        // Kickers : K > Q => J1 gagne.
        var j1 = CreerHumanPlayer(
            "J1",
            Hand(C(CardRank.As, Suit.Hearts), C(CardRank.Roi, Suit.Diamonds)),
            PokerTypeAction.Bet);

        var j2 = CreerHumanPlayer(
            "J2",
            Hand(C(CardRank.As, Suit.Diamonds), C(CardRank.Dame, Suit.Hearts)),
            PokerTypeAction.Bet);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DetermineWinnersByHand(Players, cartesCommunes);

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
            C(CardRank.Dix, Suit.Hearts),
            C(CardRank.Valet, Suit.Diamonds),
            C(CardRank.Dame, Suit.Spades),
            C(CardRank.Roi, Suit.Clubs),
            C(CardRank.As, Suit.Hearts)
        );

        var j1 = CreerHumanPlayer("J1", Hand(C(CardRank.Deux, Suit.Spades), C(CardRank.Trois, Suit.Hearts)), PokerTypeAction.Bet);
        var j2 = CreerHumanPlayer("J2", Hand(C(CardRank.Quatre, Suit.Spades), C(CardRank.Cinq, Suit.Hearts)), PokerTypeAction.Bet);

        var Players = new List<Player> { j1, j2 };

        // Act
        var gagnants = WinnerEvaluator.DetermineWinnersByHand(Players, cartesCommunes);

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

    private static Player CreerHumanPlayer(string name, HandCards main, PokerTypeAction lastAction)
    {
        var j = new HumanPlayer(name, 1000);
        j.Hand = main;
        j.LastAction = lastAction; // possible grâce au InternalsVisibleTo dans casino.core

        return j;
    }
}
