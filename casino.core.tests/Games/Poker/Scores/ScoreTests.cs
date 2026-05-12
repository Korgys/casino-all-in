using System.Globalization;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Scores;

[TestClass]
public class ScoreTests
{
    [TestMethod]
    public void Constructor_WithKickers_SetsProperties()
    {
        // Arrange
        var rank = HandRank.OnePair;
        var cardValue = CardRank.Ace;
        var kickers = new[] { CardRank.King, CardRank.Queen };

        // Act
        var score = new Score(rank, cardValue, kickers);

        // Assert
        Assert.AreEqual(rank, score.Rank);
        Assert.AreEqual(cardValue, score.CardValue);
        Assert.HasCount(2, score.Kickers);
        Assert.AreEqual(CardRank.King, score.Kickers[0]);
        Assert.AreEqual(CardRank.Queen, score.Kickers[1]);
    }

    [TestMethod]
    public void Constructor_WithoutKickers_SetsEmptyKickers()
    {
        // Arrange
        var rank = HandRank.HighCard;
        var cardValue = CardRank.Ten;

        // Act
        var score = new Score(rank, cardValue);

        // Assert
        Assert.AreEqual(rank, score.Rank);
        Assert.AreEqual(cardValue, score.CardValue);
        Assert.IsEmpty(score.Kickers);
    }

    [TestMethod]
    public void CompareTo_NullOther_Returns1()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score.CompareTo(null);

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void CompareTo_DifferentRanks_ReturnsRankComparison()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace);
        var score2 = new Score(HandRank.ThreeOfAKind, CardRank.Ace);

        // Act
        var result1 = score1.CompareTo(score2);
        var result2 = score2.CompareTo(score1);

        // Assert
        Assert.AreEqual(-1, result1); // OnePair < ThreeOfAKind
        Assert.AreEqual(1, result2);
    }

    [TestMethod]
    public void CompareTo_SameRankDifferentCardValue_ReturnsCardValueComparison()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.King);
        var score2 = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score1.CompareTo(score2);

        // Assert
        Assert.AreEqual(-1, result); // Roi < As
    }

    [TestMethod]
    public void CompareTo_SameRankSameCardValue_DifferentKickers_ReturnsKickerComparison()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King, CardRank.Queen });
        var score2 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King, CardRank.Jack });

        // Act
        var result = score1.CompareTo(score2);

        // Assert
        Assert.AreEqual(1, result); // Dame > Valet
    }

    [TestMethod]
    public void CompareTo_SameRankSameCardValue_SameKickers_Returns0()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King, CardRank.Queen });
        var score2 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King, CardRank.Queen });

        // Act
        var result = score1.CompareTo(score2);

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void CompareTo_DifferentKickerLengths_ReturnsCorrectComparison()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King });
        var score2 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King, CardRank.Queen });

        // Act
        var result1 = score1.CompareTo(score2);
        var result2 = score2.CompareTo(score1);

        // Assert
        Assert.AreEqual(-1, result1); // Shorter kickers list is considered smaller
        Assert.AreEqual(1, result2);
    }

    [TestMethod]
    public void Equals_Score_Null_ReturnsFalse()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score.Equals((Score?)null);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_Score_DifferentScores_ReturnsFalse()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace);
        var score2 = new Score(HandRank.ThreeOfAKind, CardRank.Ace);

        // Act
        var result = score1.Equals(score2);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_Score_SameScores_ReturnsTrue()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King });
        var score2 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King });

        // Act
        var result = score1.Equals(score2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_Object_Null_ReturnsFalse()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score.Equals((object?)null);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_Object_DifferentType_ReturnsFalse()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score.Equals("not a score");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_Object_SameScore_ReturnsTrue()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace);
        var score2 = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score1.Equals((object)score2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetHashCode_SameScores_ReturnsSameHashCode()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King });
        var score2 = new Score(HandRank.OnePair, CardRank.Ace, new[] { CardRank.King });

        // Act
        var hash1 = score1.GetHashCode();
        var hash2 = score2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void OperatorEqual_BothNull_ReturnsTrue()
    {
        // Act
        var result = (Score?)null == (Score?)null;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorEqual_LeftNullRightNotNull_ReturnsFalse()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = (Score?)null == score;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void OperatorEqual_SameScores_ReturnsTrue()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace);
        var score2 = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score1 == score2;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorNotEqual_DifferentScores_ReturnsTrue()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace);
        var score2 = new Score(HandRank.ThreeOfAKind, CardRank.Ace);

        // Act
        var result = score1 != score2;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorLessThan_LeftNull_ReturnsTrue()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = (Score?)null < score;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorLessThan_RightNull_ReturnsFalse()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score < (Score?)null;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void OperatorLessThan_LeftLessThanRight_ReturnsTrue()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.King);
        var score2 = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score1 < score2;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorLessThanOrEqual_LeftNull_ReturnsTrue()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = (Score?)null <= score;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorLessThanOrEqual_RightNull_ReturnsFalse()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score <= (Score?)null;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void OperatorLessThanOrEqual_EqualScores_ReturnsTrue()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace);
        var score2 = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score1 <= score2;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorGreaterThan_LeftNull_ReturnsFalse()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = (Score?)null > score;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void OperatorGreaterThan_RightNull_ReturnsTrue()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score > (Score?)null;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorGreaterThan_LeftGreaterThanRight_ReturnsTrue()
    {
        // Arrange
        var score1 = new Score(HandRank.ThreeOfAKind, CardRank.Ace);
        var score2 = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score1 > score2;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorGreaterThanOrEqual_BothNull_ReturnsTrue()
    {
        // Act
        var result = (Score?)null >= (Score?)null;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorGreaterThanOrEqual_LeftNullRightNotNull_ReturnsFalse()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = (Score?)null >= score;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void OperatorGreaterThanOrEqual_RightNull_ReturnsTrue()
    {
        // Arrange
        var score = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score >= (Score?)null;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void OperatorGreaterThanOrEqual_EqualScores_ReturnsTrue()
    {
        // Arrange
        var score1 = new Score(HandRank.OnePair, CardRank.Ace);
        var score2 = new Score(HandRank.OnePair, CardRank.Ace);

        // Act
        var result = score1 >= score2;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ToString_ReturnsExpectedFormat()
    {
        var previousCulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
            var score = new Score(HandRank.OnePair, CardRank.Ace);

            var result = score.ToString();

            Assert.AreEqual("Paire de As", result);
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previousCulture;
        }
    }

    [TestMethod]
    public void ToString_ReturnsExpectedGermanFormat()
    {
        var previousCulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("de");
            var score = new Score(HandRank.OnePair, CardRank.Ace);

            var result = score.ToString();

            Assert.AreEqual("Ein Paar aus Ass", result);
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previousCulture;
        }
    }
}
