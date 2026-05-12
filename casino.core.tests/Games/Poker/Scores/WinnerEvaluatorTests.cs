using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Scores;

[TestClass]
public class WinnerEvaluatorTests
{
    [TestMethod]
    public void DeterminerGagnantsParMain_QuandTousLesPlayersSeCouchent_DoitLeverArgumentException()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Two, Suit.Hearts),
            C(CardRank.Three, Suit.Diamonds),
            C(CardRank.Four, Suit.Spades),
            C(CardRank.Five, Suit.Clubs),
            C(CardRank.Six, Suit.Hearts)
        );

        var players = new List<Player>
        {
            CreateHumanPlayer("J1", Hand(C(CardRank.Ace, Suit.Hearts), C(CardRank.King, Suit.Diamonds)), PokerTypeAction.Fold),
            CreateHumanPlayer("J2", Hand(C(CardRank.Queen, Suit.Hearts), C(CardRank.Jack, Suit.Diamonds)), PokerTypeAction.Fold)
        };

        // Act + Assert
        Assert.Throws<ArgumentException>(() => WinnerEvaluator.DetermineWinnersByHand(players, cartesCommunes));
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_DoitIgnorerLesPlayersCouches()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Two, Suit.Hearts),
            C(CardRank.Three, Suit.Diamonds),
            C(CardRank.Four, Suit.Spades),
            C(CardRank.Eight, Suit.Clubs),
            C(CardRank.Nine, Suit.Hearts)
        );

        var j1 = CreateHumanPlayer(
            "J1",
            Hand(C(CardRank.Ace, Suit.Hearts), C(CardRank.King, Suit.Diamonds)),
            PokerTypeAction.Fold);

        var j2 = CreateHumanPlayer(
            "J2",
            Hand(C(CardRank.Queen, Suit.Hearts), C(CardRank.Jack, Suit.Diamonds)),
            PokerTypeAction.Bet);

        var players = new List<Player> { j1, j2 };

        // Act
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, cartesCommunes);

        // Assert
        Assert.HasCount(1, winners);
        Assert.AreSame(j2, winners[0], "Ignorer les players couchés même s'ils auraient une meilleure main.");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_DoitChoisirLaMainLaPlusForte_ParRang()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Two, Suit.Hearts),
            C(CardRank.Three, Suit.Diamonds),
            C(CardRank.Four, Suit.Spades),
            C(CardRank.Five, Suit.Clubs),
            C(CardRank.Nine, Suit.Hearts)
        );

        var j1 = CreateHumanPlayer(
            "J1",
            Hand(C(CardRank.Ace, Suit.Spades), C(CardRank.Queen, Suit.Hearts)),
            PokerTypeAction.Bet);

        var j2 = CreateHumanPlayer(
            "J2",
            Hand(C(CardRank.King, Suit.Spades), C(CardRank.Jack, Suit.Diamonds)),
            PokerTypeAction.Bet);

        var players = new List<Player> { j1, j2 };

        // Act
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, cartesCommunes);

        // Assert
        Assert.HasCount(1, winners);
        Assert.AreSame(j1, winners[0], "Choisir le Player avec la main la plus forte (HandRank le plus élevé).");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandMemeRang_DoitDepartagerParValeur()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Queen, Suit.Hearts),
            C(CardRank.Two, Suit.Diamonds),
            C(CardRank.Five, Suit.Spades),
            C(CardRank.Eight, Suit.Clubs),
            C(CardRank.Nine, Suit.Hearts)
        );

        var j1 = CreateHumanPlayer(
            "J1",
            Hand(C(CardRank.Queen, Suit.Spades), C(CardRank.Three, Suit.Hearts)),
            PokerTypeAction.Bet);

        var j2 = CreateHumanPlayer(
            "J2",
            Hand(C(CardRank.Nine, Suit.Spades), C(CardRank.Four, Suit.Hearts)),
            PokerTypeAction.Bet);

        var players = new List<Player> { j1, j2 };

        // Act
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, cartesCommunes);

        // Assert
        Assert.HasCount(1, winners);
        Assert.AreSame(j1, winners[0], "À rank égal, choisir la meilleure value (paire de Dames > paire de 9).");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandMemeRangEtMemeValeur_DoitDepartagerParKickers()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(CardRank.Ace, Suit.Spades),
            C(CardRank.Seven, Suit.Diamonds),
            C(CardRank.Six, Suit.Spades),
            C(CardRank.Five, Suit.Clubs),
            C(CardRank.Two, Suit.Hearts)
        );

        // Paire d'As pour les deux.
        // Kickers : K > Q => J1 gagne.
        var j1 = CreateHumanPlayer(
            "J1",
            Hand(C(CardRank.Ace, Suit.Hearts), C(CardRank.King, Suit.Diamonds)),
            PokerTypeAction.Bet);

        var j2 = CreateHumanPlayer(
            "J2",
            Hand(C(CardRank.Ace, Suit.Diamonds), C(CardRank.Queen, Suit.Hearts)),
            PokerTypeAction.Bet);

        var players = new List<Player> { j1, j2 };

        // Act
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, cartesCommunes);

        // Assert
        Assert.HasCount(1, winners);
        Assert.AreSame(j1, winners[0], "À rank et value égaux, départager par les kickers ordonnés.");
    }

    [TestMethod]
    public void DeterminerGagnantsParMain_QuandEgaliteParfaite_DoitRetournerTousLesGagnants()
    {
        // Arrange
        // Board "quinte max" : 10-J-Q-K-A, tout le monde a la même main -> égalité parfaite
        var cartesCommunes = Communes(
            C(CardRank.Ten, Suit.Hearts),
            C(CardRank.Jack, Suit.Diamonds),
            C(CardRank.Queen, Suit.Spades),
            C(CardRank.King, Suit.Clubs),
            C(CardRank.Ace, Suit.Hearts)
        );

        var j1 = CreateHumanPlayer("J1", Hand(C(CardRank.Two, Suit.Spades), C(CardRank.Three, Suit.Hearts)), PokerTypeAction.Bet);
        var j2 = CreateHumanPlayer("J2", Hand(C(CardRank.Four, Suit.Spades), C(CardRank.Five, Suit.Hearts)), PokerTypeAction.Bet);

        var players = new List<Player> { j1, j2 };

        // Act
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, cartesCommunes);

        // Assert
        Assert.HasCount(2, winners);
        CollectionAssert.Contains(winners.ToList(), j1);
        CollectionAssert.Contains(winners.ToList(), j2);
    }

    private static Card C(CardRank rank, Suit suit)
        => new Card(rank, suit);

    private static HandCards Hand(Card a, Card b)
        => new HandCards(a, b);

    private static TableCards Communes(Card a, Card b, Card c, Card d, Card e)
        => new TableCards { Flop1 = a, Flop2 = b, Flop3 = c, Turn = d, River = e };

    private static Player CreateHumanPlayer(string name, HandCards main, PokerTypeAction lastAction)
    {
        var j = new HumanPlayer(name, 1000);
        j.Hand = main;
        j.LastAction = lastAction; // possible grâce au InternalsVisibleTo dans casino.core

        return j;
    }
}
