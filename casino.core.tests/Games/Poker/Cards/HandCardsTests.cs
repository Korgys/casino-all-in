using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class HandCardsTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeFirstAndSecond()
    {
        // Arrange
        var c1 = new Card(CardRank.Ace, Suit.Spades);
        var c2 = new Card(CardRank.King, Suit.Hearts);

        // Act
        var hand = new HandCards(c1, c2);

        // Assert
        Assert.AreSame(c1, hand.First);
        Assert.AreSame(c2, hand.Second);
    }

    [TestMethod]
    public void AsEnumerable_ShouldReturnTwoCardsInOrder()
    {
        // Arrange
        var c1 = new Card(CardRank.Ace, Suit.Spades);
        var c2 = new Card(CardRank.King, Suit.Hearts);
        var hand = new HandCards(c1, c2);

        // Act
        var cards = hand.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(2, cards);
        Assert.AreSame(c1, cards[0]);
        Assert.AreSame(c2, cards[1]);
    }

    [TestMethod]
    public void AsEnumerable_WhenSecondIsNull_ShouldReturnOnlyFirst()
    {
        // Arrange
        var c1 = new Card(CardRank.Ace, Suit.Spades);
        var hand = new HandCards(c1, second: null!);

        // Act
        var cards = hand.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(1, cards);
        Assert.AreSame(c1, cards[0]);
    }

    [TestMethod]
    public void ToString_ShouldConcatenateBothCards()
    {
        // Arrange
        var c1 = new Card(CardRank.Ace, Suit.Spades);     // "A spade"
        var c2 = new Card(CardRank.King, Suit.Hearts);    // "K heart"
        var hand = new HandCards(c1, c2);

        // Act
        var s = hand.ToString();

        // Assert
        Assert.AreEqual("A♠, K♥", s);
    }
}
