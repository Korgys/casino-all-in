using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class CardRankTests
{
    [TestMethod]
    [DataRow(CardRank.Two, "2")]
    [DataRow(CardRank.Three, "3")]
    [DataRow(CardRank.Four, "4")]
    [DataRow(CardRank.Five, "5")]
    [DataRow(CardRank.Six, "6")]
    [DataRow(CardRank.Seven, "7")]
    [DataRow(CardRank.Eight, "8")]
    [DataRow(CardRank.Nine, "9")]
    [DataRow(CardRank.Ten, "10")]
    [DataRow(CardRank.Jack, "J")]
    [DataRow(CardRank.Queen, "Q")]
    [DataRow(CardRank.King, "K")]
    [DataRow(CardRank.Ace, "A")]
    public void ToShortString_ShouldReturnCorrectString(CardRank rank, string expected)
    {
        // Act
        var result = rank.ToShortString();

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ToShortString_QuandRangInvalide_DoitLeverArgumentOutOfRangeException()
    {
        // Arrange
        var rangInvalide = (CardRank)999;

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => rangInvalide.ToShortString());
    }
}
