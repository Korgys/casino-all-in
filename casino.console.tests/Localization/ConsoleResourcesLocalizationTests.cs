using System.Globalization;
using System.Resources;

namespace casino.console.tests.Localization;

[TestClass]
public class ConsoleResourcesLocalizationTests
{
    private static readonly ResourceManager ResourceManager = new("casino.console.Localization.ConsoleResources", typeof(casino.console.Program).Assembly);

    [TestMethod]
    public void LanguageNames_AreDisplayedAsNativeAutonyms_InEverySupportedCulture()
    {
        var cultures = new[] { "en", "fr-FR", "de-DE", "es-ES", "ja-JP", "zh-Hans", "ru-RU" };

        foreach (var cultureName in cultures)
        {
            var culture = new CultureInfo(cultureName);
            Assert.AreEqual("Fran\u00E7ais", ResourceManager.GetString("LanguageFrench", culture));
            Assert.AreEqual("English", ResourceManager.GetString("LanguageEnglish", culture));
            Assert.AreEqual("Deutsch", ResourceManager.GetString("LanguageGerman", culture));
            Assert.AreEqual("Espa\u00F1ol", ResourceManager.GetString("LanguageSpanish", culture));
            Assert.AreEqual("\u65E5\u672C\u8A9E", ResourceManager.GetString("LanguageJapanese", culture));
            Assert.AreEqual("\u7B80\u4F53\u4E2D\u6587", ResourceManager.GetString("LanguageSimplifiedChinese", culture));
            Assert.AreEqual("\u0420\u0443\u0441\u0441\u043A\u0438\u0439", ResourceManager.GetString("LanguageRussian", culture));
        }
    }

    [TestMethod]
    public void RussianCulture_ContainsLocalizedMenuLabels()
    {
        var culture = new CultureInfo("ru-RU");

        Assert.AreEqual("\u042F\u0437\u044B\u043A", ResourceManager.GetString("MenuLanguages", culture));
        Assert.AreEqual("\u0412\u042B\u0411\u041E\u0420 \u042F\u0417\u042B\u041A\u0410", ResourceManager.GetString("LanguageMenuTitle", culture));
        Assert.AreEqual("\u0422\u0435\u043A\u0443\u0449\u0438\u0439 \u044F\u0437\u044B\u043A", ResourceManager.GetString("CurrentLanguageLabel", culture));
    }

    [TestMethod]
    public void RouletteKeys_ArePresent_InEverySupportedCulture()
    {
        var cultures = new[] { "en", "fr-FR", "de-DE", "es-ES", "ja-JP", "zh-Hans", "ru-RU" };
        var keys = new[] { "MenuRoulette", "RouletteHeader", "RouletteBetTypePrompt" };

        foreach (var cultureName in cultures)
        {
            var culture = new CultureInfo(cultureName);
            foreach (var key in keys)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(ResourceManager.GetString(key, culture)), $"Missing '{key}' for {cultureName}.");
            }
        }
    }
}
