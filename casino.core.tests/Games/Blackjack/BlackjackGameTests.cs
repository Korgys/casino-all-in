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
        game.GameEnded += (_, e) => winner = e.WinnerName;

        game.Run();

        Assert.AreEqual("Le croupier", winner);
    }
}
