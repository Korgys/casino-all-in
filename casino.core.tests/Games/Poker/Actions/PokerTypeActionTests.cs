using System.Globalization;
using casino.core.Games.Poker.Actions;
using casino.core.Properties.Languages;

namespace casino.core.tests.Games.Poker.Actions;

[TestClass]
public class PokerTypeActionTests
{
    [TestMethod]
    public void ToDisplayString_ShouldReturnCorrectFrenchTranslation()
    {
        // Arrange
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");

        // Act & Assert
        Assert.AreEqual(Resources.None, PokerTypeAction.None.ToDisplayString());
        Assert.AreEqual(Resources.Fold, PokerTypeAction.Fold.ToDisplayString());
        Assert.AreEqual(Resources.Bet, PokerTypeAction.Bet.ToDisplayString());
        Assert.AreEqual(Resources.Call, PokerTypeAction.Call.ToDisplayString());
        Assert.AreEqual(Resources.Raise, PokerTypeAction.Raise.ToDisplayString());
        Assert.AreEqual(Resources.Check, PokerTypeAction.Check.ToDisplayString());
        Assert.AreEqual(Resources.AllIn, PokerTypeAction.AllIn.ToDisplayString());
    }

    [TestMethod]
    public void ToDisplayString_ShouldReturnCorrectEnglishTranslation()
    {
        // Arrange
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

        // Act & Assert
        Assert.AreEqual(Resources.None, PokerTypeAction.None.ToDisplayString());
        Assert.AreEqual(Resources.Fold, PokerTypeAction.Fold.ToDisplayString());
        Assert.AreEqual(Resources.Bet, PokerTypeAction.Bet.ToDisplayString());
        Assert.AreEqual(Resources.Call, PokerTypeAction.Call.ToDisplayString());
        Assert.AreEqual(Resources.Raise, PokerTypeAction.Raise.ToDisplayString());
        Assert.AreEqual(Resources.Check, PokerTypeAction.Check.ToDisplayString());
        Assert.AreEqual(Resources.AllIn, PokerTypeAction.AllIn.ToDisplayString());
    }

    [TestMethod]
    public void ToDisplayString_ShouldReturnCorrectJapaneseTranslation()
    {
        var previousCulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP");

            Assert.AreEqual("なし", PokerTypeAction.None.ToDisplayString());
            Assert.AreEqual("フォールド", PokerTypeAction.Fold.ToDisplayString());
            Assert.AreEqual("ベット", PokerTypeAction.Bet.ToDisplayString());
            Assert.AreEqual("コール", PokerTypeAction.Call.ToDisplayString());
            Assert.AreEqual("レイズ", PokerTypeAction.Raise.ToDisplayString());
            Assert.AreEqual("チェック", PokerTypeAction.Check.ToDisplayString());
            Assert.AreEqual("オールイン", PokerTypeAction.AllIn.ToDisplayString());
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

            Assert.AreEqual("无", PokerTypeAction.None.ToDisplayString());
            Assert.AreEqual("弃牌", PokerTypeAction.Fold.ToDisplayString());
            Assert.AreEqual("下注", PokerTypeAction.Bet.ToDisplayString());
            Assert.AreEqual("跟注", PokerTypeAction.Call.ToDisplayString());
            Assert.AreEqual("加注", PokerTypeAction.Raise.ToDisplayString());
            Assert.AreEqual("过牌", PokerTypeAction.Check.ToDisplayString());
            Assert.AreEqual("全下", PokerTypeAction.AllIn.ToDisplayString());
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previousCulture;
        }
    }
}
