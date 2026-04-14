using System.Globalization;
using casino.console.Localization;
using casino.core.Games.Poker;

namespace casino.console.tests.Localization;

[TestClass]
public class ConsoleTextTests
{
    private CultureInfo? originalCulture;
    private CultureInfo? originalUiCulture;

    [TestInitialize]
    public void Initialize()
    {
        originalCulture = CultureInfo.CurrentCulture;
        originalUiCulture = CultureInfo.CurrentUICulture;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (originalCulture is not null)
            CultureInfo.CurrentCulture = originalCulture;

        if (originalUiCulture is not null)
            CultureInfo.CurrentUICulture = originalUiCulture;
    }

    [TestMethod]
    public void MainMenuChoice_UsesFrenchTranslation_WhenUiCultureIsFrench()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

        Assert.AreEqual("Votre choix: ", ConsoleText.MainMenuChoice);
    }

    [TestMethod]
    public void PokerDifficultyLabel_AndFormattedPrompt_ReturnExpectedText()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

        Assert.AreEqual("Beginner", ConsoleText.PokerDifficultyLabel(PokerDifficulty.Beginner));
        Assert.AreEqual("999", ConsoleText.PokerDifficultyLabel((PokerDifficulty)999));
        Assert.AreEqual("Please enter a value between 1 and 2.", ConsoleText.RangeError(1, 2));
    }
}
