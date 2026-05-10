using System.Globalization;

namespace casino.console.tests;

[TestClass]
[DoNotParallelize]
public class ProgramTests
{
    private CultureInfo? originalCulture;
    private CultureInfo? originalUiCulture;
    private CultureInfo? originalDefaultCulture;
    private CultureInfo? originalDefaultUiCulture;
    private TextWriter? originalOut;
    private TextWriter? originalError;

    [TestInitialize]
    public void Initialize()
    {
        originalCulture = CultureInfo.CurrentCulture;
        originalUiCulture = CultureInfo.CurrentUICulture;
        originalDefaultCulture = CultureInfo.DefaultThreadCurrentCulture;
        originalDefaultUiCulture = CultureInfo.DefaultThreadCurrentUICulture;
        originalOut = Console.Out;
        originalError = Console.Error;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (originalOut is not null)
            Console.SetOut(originalOut);

        if (originalError is not null)
            Console.SetError(originalError);

        if (originalCulture is not null)
            CultureInfo.CurrentCulture = originalCulture;

        if (originalUiCulture is not null)
            CultureInfo.CurrentUICulture = originalUiCulture;

        CultureInfo.DefaultThreadCurrentCulture = originalDefaultCulture;
        CultureInfo.DefaultThreadCurrentUICulture = originalDefaultUiCulture;
    }

    [TestMethod]
    public void Main_WithHelp_PrintsUsageAndReturnsSuccess()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        Console.SetOut(output);
        Console.SetError(error);

        var exitCode = Program.Main(["--help"]);

        Assert.AreEqual(0, exitCode);
        Assert.Contains("casino blackjack", output.ToString());
        Assert.AreEqual(string.Empty, error.ToString());
    }

    [TestMethod]
    public void Main_WithoutCommand_PrintsUsageAndReturnsFailure()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        Console.SetOut(output);
        Console.SetError(error);

        var exitCode = Program.Main([]);

        Assert.AreEqual(1, exitCode);
        Assert.AreEqual(string.Empty, output.ToString());
        Assert.Contains("A game command is required.", error.ToString());
        Assert.Contains("casino poker -p 4 -d 4 -c 1000", error.ToString());
    }
}
