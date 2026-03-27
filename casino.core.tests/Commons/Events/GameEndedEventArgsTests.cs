using casino.core.Common.Events;

namespace casino.core.tests.Commons.Events;

[TestClass]
public class GameEndedEventArgsTests
{
    [TestMethod]
    public void Constructor_ShouldMapAllProperties()
    {
        var args = new GameEndedEventArgs("Alice", 250);

        Assert.AreEqual("Alice", args.WinnerName);
        Assert.AreEqual(250, args.Pot);
    }
}
