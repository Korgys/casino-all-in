using System.Globalization;
using System.Reflection;
using casino.console.Games;
using casino.core;
using casino.core.Games.Poker;

namespace casino.console.tests;

[TestClass]
public class ProgramTests
{
    private CultureInfo? originalCulture;
    private CultureInfo? originalUiCulture;
    private TextReader? originalIn;
    private TextWriter? originalOut;

    [TestInitialize]
    public void Initialize()
    {
        originalCulture = CultureInfo.CurrentCulture;
        originalUiCulture = CultureInfo.CurrentUICulture;
        originalIn = Console.In;
        originalOut = Console.Out;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (originalOut is not null)
            Console.SetOut(originalOut);

        if (originalCulture is not null)
            CultureInfo.CurrentCulture = originalCulture;

        if (originalUiCulture is not null)
            CultureInfo.CurrentUICulture = originalUiCulture;

        if (originalIn is not null)
            Console.SetIn(originalIn);
    }

    [TestMethod]
    public void BuildGame_RoutesPokerBranch_AfterInvalidChoice_UsingAlias()
    {
        using var _ = new ConsoleScope(string.Join(Environment.NewLine, "not-a-choice", "poker", "", "", "") + Environment.NewLine);

        var result = InvokeBuildGame();

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<PokerGame>(result);
    }

    [TestMethod]
    public void BuildGame_RoutesLanguageBranch_ForPluralAlias_AndReturnsOnBack()
    {
        SetFrenchCulture();

        using var _ = new ConsoleScope(string.Join(Environment.NewLine, "languages", "back", "quit") + Environment.NewLine);

        var result = InvokeBuildGame();

        Assert.IsNull(result);
        Assert.AreEqual("fr-FR", CultureInfo.CurrentCulture.Name);
        Assert.AreEqual("fr-FR", CultureInfo.CurrentUICulture.Name);
    }

    [TestMethod]
    public void ShowLanguageMenu_ChangesCulture_ForNumericAlias()
    {
        SetFrenchCulture();

        using var _ = new ConsoleScope("2" + Environment.NewLine);

        InvokeShowLanguageMenu();

        Assert.AreEqual("en", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        Assert.AreEqual("en", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void ShowLanguageMenu_ChangesCulture_ForTextAlias_AfterInvalidChoice()
    {
        SetFrenchCulture();

        using var _ = new ConsoleScope(string.Join(Environment.NewLine, "bogus", "english") + Environment.NewLine);

        InvokeShowLanguageMenu();

        Assert.AreEqual("en", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        Assert.AreEqual("en", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
    }

    private static IGame? InvokeBuildGame()
    {
        var method = typeof(Program).GetMethod("BuildGame", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        return (IGame?)method.Invoke(null, [new ConsoleGameFactory()]);
    }

    private static void InvokeShowLanguageMenu()
    {
        var method = typeof(Program).GetMethod("ShowLanguageMenu", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        method.Invoke(null, null);
    }

    private static void SetFrenchCulture()
    {
        var culture = CultureInfo.GetCultureInfo("fr-FR");
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    private sealed class ConsoleScope : IDisposable
    {
        private readonly TextReader? previousIn;
        private readonly TextWriter? previousOut;

        public ConsoleScope(string input)
        {
            previousIn = Console.In;
            previousOut = Console.Out;

            var output = new StringWriter();
            Console.SetIn(new StringReader(input));
            Console.SetOut(output);
        }

        public void Dispose()
        {
            if (previousIn is not null)
                Console.SetIn(previousIn);

            if (previousOut is not null)
                Console.SetOut(previousOut);
        }
    }
}
