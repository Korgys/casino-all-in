using casino.console.Games.Commons;

namespace casino.console.tests.Games.Commons;

[TestClass]
public class ConsoleLayoutTests
{
    [TestMethod]
    public void GetVisibleLength_IgnoresAnsiEscapeSequences()
    {
        var text = "\u001b[31mHello\u001b[0m";

        var length = ConsoleLayout.GetVisibleLength(text);

        Assert.AreEqual(5, length);
    }

    [TestMethod]
    public void GetVisibleLength_CountsEmojiAsDoubleWidth()
    {
        var width = ConsoleLayout.GetVisibleLength("[🔔][⭐][💎]");

        Assert.AreEqual(11, width);
    }

    [TestMethod]
    public void FitToWidth_TruncatesAndAddsEllipsis_WhenTextIsTooLong()
    {
        var fitted = ConsoleLayout.FitToWidth("Bonjour tout le monde", 8);

        Assert.AreEqual("Bonjour…", fitted);
    }

    [TestMethod]
    public void FitToWidth_PreservesAnsiSequences_WhenTruncating()
    {
        var text = "\u001b[36mABCD\u001b[0mEFGH";

        var fitted = ConsoleLayout.FitToWidth(text, 5);

        Assert.IsTrue(fitted.Contains("\u001b[36m", StringComparison.Ordinal));
        Assert.EndsWith("…", fitted);
        Assert.AreEqual(5, ConsoleLayout.GetVisibleLength(fitted));
    }
}
