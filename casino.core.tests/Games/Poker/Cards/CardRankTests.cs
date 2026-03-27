using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class CardRankTests
{
    [TestMethod]
    [DataRow(CardRank.Deux, "2")]
    [DataRow(CardRank.Trois, "3")]
    [DataRow(CardRank.Quatre, "4")]
    [DataRow(CardRank.Cinq, "5")]
    [DataRow(CardRank.Six, "6")]
    [DataRow(CardRank.Sept, "7")]
    [DataRow(CardRank.Huit, "8")]
    [DataRow(CardRank.Neuf, "9")]
    [DataRow(CardRank.Dix, "10")]
    [DataRow(CardRank.Valet, "J")]
    [DataRow(CardRank.Dame, "Q")]
    [DataRow(CardRank.Roi, "K")]
    [DataRow(CardRank.As, "A")]
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
