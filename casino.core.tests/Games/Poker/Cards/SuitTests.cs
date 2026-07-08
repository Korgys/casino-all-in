using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class SuitTests
{
    [TestMethod]
    [DataRow(Suit.Hearts, "♥")]
    [DataRow(Suit.Diamonds, "♦")]
    [DataRow(Suit.Clubs, "♣")]
    [DataRow(Suit.Spades, "♠")]
    public void ToSymbol_ShouldReturnExpectedSymbol(Suit suit, string expected)
    {
        // Act
        var result = suit.ToSymbol();

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ToSymbol_WhenSuitIsInvalid_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var invalidSuit = (Suit)999;

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => invalidSuit.ToSymbol());
    }
}
