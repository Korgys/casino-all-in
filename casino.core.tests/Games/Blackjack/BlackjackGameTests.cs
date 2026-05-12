using System.Globalization;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker.Cards;
using casino.core.Properties.Languages;
using casino.core.tests.Fakes;

namespace casino.core.tests.Games.Blackjack;

[TestClass]
public class BlackjackGameTests
{
    [TestMethod]
    public void Run_PlayerBusts_DealerWinsRound()
    {
        var previousCulture = Resources.Culture;

        try
        {
            Resources.Culture = CultureInfo.GetCultureInfo("en");

            var cards = new List<Card>
            {
                new(CardRank.Ten, Suit.Hearts),
                new(CardRank.Nine, Suit.Hearts),
                new(CardRank.Six, Suit.Spades),
                new(CardRank.Seven, Suit.Clubs),
                new(CardRank.Eight, Suit.Diamonds)
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

            Assert.AreEqual("Dealer", winner);
            Assert.IsNotNull(finalState);
            Assert.AreEqual(BlackjackRoundOutcome.DealerWin, finalState.RoundOutcome);
            Assert.AreEqual(0, finalState.PlayerWins);
            Assert.AreEqual(1, finalState.DealerWins);
            Assert.AreEqual(0, finalState.Pushes);
        }
        finally
        {
            Resources.Culture = previousCulture;
        }
    }

    [TestMethod]
    public void Run_TracksStatsAcrossMultipleRounds()
    {
        var firstRoundCards = new List<Card>
        {
            new(CardRank.Ten, Suit.Spades),
            new(CardRank.Nine, Suit.Hearts),
            new(CardRank.Nine, Suit.Clubs),
            new(CardRank.Seven, Suit.Diamonds),
            new(CardRank.Five, Suit.Spades)
        };

        var secondRoundCards = new List<Card>
        {
            new(CardRank.Ten, Suit.Hearts),
            new(CardRank.Nine, Suit.Spades),
            new(CardRank.Queen, Suit.Diamonds),
            new(CardRank.Seven, Suit.Clubs)
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
