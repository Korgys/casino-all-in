using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class CardTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeRankAndSuit()
    {
        // Arrange
        var rank = CardRank.Ace;
        var suit = Suit.Hearts;

        // Act
        var card = new Card(rank, suit);

        // Assert
        Assert.AreEqual(rank, card.Rank, "The rank should match the constructor value.");
        Assert.AreEqual(suit, card.Suit, "The suit should match the constructor value.");
    }

    [TestMethod]
    public void ToString_ShouldReturnShortRankAndSuit()
    {
        // Arrange
        var card = new Card(CardRank.Ace, Suit.Spades);

        // Act
        var result = card.ToString();

        // Assert
        Assert.AreEqual("A♠", result, "ToString format is not valid.");
    }

    [TestMethod]
    public void ToString_ShouldBeDeterministic()
    {
        // Arrange
        var card = new Card(CardRank.Ten, Suit.Diamonds);

        // Act
        var first = card.ToString();
        var second = card.ToString();

        // Assert
        Assert.AreEqual(first, second, "ToString should always return the same value.");
    }

    [TestMethod]
    public void Equals_WhenCardsAreIdentical_ShouldReturnTrue()
    {
        // Arrange
        var card1 = new Card(CardRank.King, Suit.Clubs);
        var card2 = new Card(CardRank.King, Suit.Clubs);

        // Act
        var equals = card1.Equals(card2);

        // Assert
        Assert.IsTrue(equals, "Cards with the same rank and suit should be equal.");
    }

    [TestMethod]
    public void Equals_WhenCardsDiffer_ShouldReturnFalse()
    {
        // Arrange
        var card1 = new Card(CardRank.King, Suit.Clubs);
        var card2 = new Card(CardRank.King, Suit.Hearts);

        // Act
        var equals = card1.Equals(card2);

        // Assert
        Assert.IsFalse(equals, "Cards with different suits should not be equal.");
    }

    [TestMethod]
    public void GetHashCode_WhenCardsAreIdentical_ShouldReturnSameValue()
    {
        // Arrange
        var card1 = new Card(CardRank.Queen, Suit.Hearts);
        var card2 = new Card(CardRank.Queen, Suit.Hearts);

        // Act
        var hash1 = card1.GetHashCode();
        var hash2 = card2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2, "Identical cards should have the same hash.");
    }
}
