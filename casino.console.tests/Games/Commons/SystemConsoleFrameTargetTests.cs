using System.Globalization;
using casino.console.Games.Commons;

namespace casino.console.tests.Games.Commons;

[TestClass]
public class SystemConsoleFrameTargetTests
{
    private TextWriter? originalOut;

    [TestInitialize]
    public void Initialize()
    {
        originalOut = Console.Out;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (originalOut is not null)
            Console.SetOut(originalOut);
    }

    [TestMethod]
    public void SupportsCursorPositioning_ReturnsFalse_WhenOutputIsRedirected()
    {
        Console.SetOut(new StringWriter());
        var target = new SystemConsoleFrameTarget();

        Assert.IsFalse(target.SupportsCursorPositioning);
    }

    [TestMethod]
    public void WindowDimensions_ReturnNull_WhenGetterThrows()
    {
        var method = typeof(SystemConsoleFrameTarget).GetMethod("TryGetDimension", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.IsNotNull(method);

        var result = method.Invoke(null, [new Func<int>(() => throw new InvalidOperationException())]);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Write_And_WriteLine_DelegateToConsoleOut()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        var target = new SystemConsoleFrameTarget();

        target.Write("abc");
        target.WriteLine();
        target.Write("def");

        Assert.AreEqual("abc" + Environment.NewLine + "def", writer.ToString());
    }
}
