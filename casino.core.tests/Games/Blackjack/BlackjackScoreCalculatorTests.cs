using casino.core.Games.Blackjack;
using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Blackjack;

[TestClass]
public class BlackjackScoreCalculatorTests
{
    [TestMethod]
    public void Calculate_UsesFaceCardsAsTen()
    {
        var cards = new List<Card>
        {
            new(CardRank.King, Suit.Hearts),
            new(CardRank.Queen, Suit.Spades)
        };

        var score = BlackjackScoreCalculator.Calculate(cards);

        Assert.AreEqual(20, score);
    }

    [TestMethod]
    public void Calculate_AdjustsAceWhenBusted()
    {
        var cards = new List<Card>
        {
            new(CardRank.Ace, Suit.Hearts),
            new(CardRank.Nine, Suit.Spades),
            new(CardRank.Eight, Suit.Clubs)
        };

        var score = BlackjackScoreCalculator.Calculate(cards);

        Assert.AreEqual(18, score);
    }
}
