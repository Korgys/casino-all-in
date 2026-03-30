using System.Globalization;
using casino.core.Games.Poker.Scores;
using casino.core.Properties.Languages;

namespace casino.core.tests.Games.Poker.Scores;

[TestClass]
public class HandRankExtensionsTests
{
    [TestMethod]
    public void ToDisplayString_ShouldReturnCorrectFrenchTranslation()
    {
        var previousCulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");

            Assert.AreEqual(Resources.HighCard, HandRank.HighCard.ToDisplayString());
            Assert.AreEqual(Resources.OnePair, HandRank.OnePair.ToDisplayString());
            Assert.AreEqual(Resources.TwoPair, HandRank.TwoPair.ToDisplayString());
            Assert.AreEqual(Resources.ThreeOfAKind, HandRank.ThreeOfAKind.ToDisplayString());
            Assert.AreEqual(Resources.Straight, HandRank.Straight.ToDisplayString());
            Assert.AreEqual(Resources.Flush, HandRank.Flush.ToDisplayString());
            Assert.AreEqual(Resources.FullHouse, HandRank.FullHouse.ToDisplayString());
            Assert.AreEqual(Resources.FourOfAKind, HandRank.FourOfAKind.ToDisplayString());
            Assert.AreEqual(Resources.StraightFlush, HandRank.StraightFlush.ToDisplayString());
            Assert.AreEqual(Resources.RoyalFlush, HandRank.RoyalFlush.ToDisplayString());
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previousCulture;
        }
    }

    [TestMethod]
    public void ToDisplayString_ShouldReturnCorrectEnglishTranslation()
    {
        var previousCulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            Assert.AreEqual(Resources.HighCard, HandRank.HighCard.ToDisplayString());
            Assert.AreEqual(Resources.OnePair, HandRank.OnePair.ToDisplayString());
            Assert.AreEqual(Resources.TwoPair, HandRank.TwoPair.ToDisplayString());
            Assert.AreEqual(Resources.ThreeOfAKind, HandRank.ThreeOfAKind.ToDisplayString());
            Assert.AreEqual(Resources.Straight, HandRank.Straight.ToDisplayString());
            Assert.AreEqual(Resources.Flush, HandRank.Flush.ToDisplayString());
            Assert.AreEqual(Resources.FullHouse, HandRank.FullHouse.ToDisplayString());
            Assert.AreEqual(Resources.FourOfAKind, HandRank.FourOfAKind.ToDisplayString());
            Assert.AreEqual(Resources.StraightFlush, HandRank.StraightFlush.ToDisplayString());
            Assert.AreEqual(Resources.RoyalFlush, HandRank.RoyalFlush.ToDisplayString());
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previousCulture;
        }
    }

    [TestMethod]
    public void ToDisplayString_ShouldReturnCorrectSimplifiedChineseTranslation()
    {
        var previousCulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-Hans");

            Assert.AreEqual("高牌", HandRank.HighCard.ToDisplayString());
            Assert.AreEqual("一对", HandRank.OnePair.ToDisplayString());
            Assert.AreEqual("两对", HandRank.TwoPair.ToDisplayString());
            Assert.AreEqual("三条", HandRank.ThreeOfAKind.ToDisplayString());
            Assert.AreEqual("顺子", HandRank.Straight.ToDisplayString());
            Assert.AreEqual("同花", HandRank.Flush.ToDisplayString());
            Assert.AreEqual("葫芦", HandRank.FullHouse.ToDisplayString());
            Assert.AreEqual("四条", HandRank.FourOfAKind.ToDisplayString());
            Assert.AreEqual("同花顺", HandRank.StraightFlush.ToDisplayString());
            Assert.AreEqual("皇家同花顺", HandRank.RoyalFlush.ToDisplayString());
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previousCulture;
        }
    }
}
