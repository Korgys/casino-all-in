using casino.core.Games.Poker.Actions;
using casino.core.Properties.Langages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Threading;

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
}