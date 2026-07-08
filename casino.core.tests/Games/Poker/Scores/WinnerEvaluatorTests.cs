using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Scores;

[TestClass]
public class WinnerEvaluatorTests
{
    [TestMethod]
    public void DetermineWinnersByHand_WhenAllPlayersFold_ShouldThrowArgumentException()
    {
        // Arrange
        var communityCards = CommunityCards(
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
        Assert.Throws<ArgumentException>(() => WinnerEvaluator.DetermineWinnersByHand(players, communityCards));
    }

    [TestMethod]
    public void DetermineWinnersByHand_ShouldIgnoreFoldedPlayers()
    {
        // Arrange
        var communityCards = CommunityCards(
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
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, communityCards);

        // Assert
        Assert.HasCount(1, winners);
        Assert.AreSame(j2, winners[0], "Folded players should be ignored even when they would have a better hand.");
    }

    [TestMethod]
    public void DetermineWinnersByHand_ShouldChooseStrongestHandByRank()
    {
        // Arrange
        var communityCards = CommunityCards(
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
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, communityCards);

        // Assert
        Assert.HasCount(1, winners);
        Assert.AreSame(j1, winners[0], "The player with the strongest hand rank should win.");
    }

    [TestMethod]
    public void DetermineWinnersByHand_WhenSameRank_ShouldBreakTieByValue()
    {
        // Arrange
        var communityCards = CommunityCards(
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
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, communityCards);

        // Assert
        Assert.HasCount(1, winners);
        Assert.AreSame(j1, winners[0], "When ranks match, the higher value should win (pair of Queens > pair of 9s).");
    }

    [TestMethod]
    public void DetermineWinnersByHand_WhenSameRankAndValue_ShouldBreakTieByKickers()
    {
        // Arrange
        var communityCards = CommunityCards(
            C(CardRank.Ace, Suit.Spades),
            C(CardRank.Seven, Suit.Diamonds),
            C(CardRank.Six, Suit.Spades),
            C(CardRank.Five, Suit.Clubs),
            C(CardRank.Two, Suit.Hearts)
        );

        // Both players have a pair of Aces.
        // Kickers: K > Q, so J1 wins.
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
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, communityCards);

        // Assert
        Assert.HasCount(1, winners);
        Assert.AreSame(j1, winners[0], "When rank and value match, ordered kickers should break the tie.");
    }

    [TestMethod]
    public void DetermineWinnersByHand_WhenPerfectTie_ShouldReturnAllWinners()
    {
        // Arrange
        // Broadway board: 10-J-Q-K-A, everyone has the same hand, resulting in a perfect tie.
        var communityCards = CommunityCards(
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
        var winners = WinnerEvaluator.DetermineWinnersByHand(players, communityCards);

        // Assert
        Assert.HasCount(2, winners);
        CollectionAssert.Contains(winners.ToList(), j1);
        CollectionAssert.Contains(winners.ToList(), j2);
    }

    private static Card C(CardRank rank, Suit suit)
        => new Card(rank, suit);

    private static HandCards Hand(Card a, Card b)
        => new HandCards(a, b);

    private static TableCards CommunityCards(Card a, Card b, Card c, Card d, Card e)
        => new TableCards { Flop1 = a, Flop2 = b, Flop3 = c, Turn = d, River = e };

    private static Player CreateHumanPlayer(string name, HandCards hand, PokerTypeAction lastAction)
    {
        var player = new HumanPlayer(name, 1000);
        player.Hand = hand;
        player.LastAction = lastAction; // Allowed by InternalsVisibleTo in casino.core.

        return player;
    }
}
