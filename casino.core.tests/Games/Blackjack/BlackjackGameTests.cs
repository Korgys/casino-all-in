using casino.core.Games.Blackjack;
using casino.core.Games.Poker.Cards;
using casino.core.tests.Fakes;

namespace casino.core.tests.Games.Blackjack;

[TestClass]
public class BlackjackGameTests
{
    [TestMethod]
    public void Run_PlayerBusts_DealerWinsRound()
    {
        var cards = new List<Card>
        {
            new(CardRank.Dix, Suit.Hearts),
            new(CardRank.Neuf, Suit.Hearts),
            new(CardRank.Six, Suit.Spades),
            new(CardRank.Sept, Suit.Clubs),
            new(CardRank.Huit, Suit.Diamonds)
        };

        var game = new BlackjackGame(
            _ => BlackjackAction.Hit,
            () => false,
            () => new FakeDeck(cards));

        string? winner = null;
        BlackjackGameState? finalState = null;
        game.GameEnded += (_, e) => winner = e.WinnerName;
        game.StateUpdated += (_, e) => finalState = (BlackjackGameState)e.State;

        game.Run();

        Assert.AreEqual("Le croupier", winner);
        Assert.IsNotNull(finalState);
        Assert.AreEqual(BlackjackRoundOutcome.DealerWin, finalState.RoundOutcome);
        Assert.AreEqual(0, finalState.PlayerWins);
        Assert.AreEqual(1, finalState.DealerWins);
        Assert.AreEqual(0, finalState.Pushes);
    }

    [TestMethod]
    public void Run_TracksStatsAcrossMultipleRounds()
    {
        var firstRoundCards = new List<Card>
        {
            new(CardRank.Dix, Suit.Spades),
            new(CardRank.Neuf, Suit.Hearts),
            new(CardRank.Neuf, Suit.Clubs),
            new(CardRank.Sept, Suit.Diamonds),
            new(CardRank.Cinq, Suit.Spades)
        };

        var secondRoundCards = new List<Card>
        {
            new(CardRank.Dix, Suit.Hearts),
            new(CardRank.Neuf, Suit.Spades),
            new(CardRank.Dame, Suit.Diamonds),
            new(CardRank.Sept, Suit.Clubs)
        };

        var allCards = firstRoundCards.Concat(secondRoundCards).ToList();
        var continueCalls = 0;

        var game = new BlackjackGame(
            _ => BlackjackAction.Stand,
            () => continueCalls++ == 0,
            () => new FakeDeck(allCards));

        BlackjackGameState? finalState = null;
        game.StateUpdated += (_, e) => finalState = (BlackjackGameState)e.State;

        game.Run();

        Assert.IsNotNull(finalState);
        Assert.AreEqual(BlackjackRoundOutcome.PlayerWin, finalState.RoundOutcome);
        Assert.AreEqual(1, finalState.PlayerWins);
        Assert.AreEqual(1, finalState.DealerWins);
        Assert.AreEqual(0, finalState.Pushes);
    }
}