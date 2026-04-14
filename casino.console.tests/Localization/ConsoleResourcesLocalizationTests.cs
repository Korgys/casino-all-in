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
            Assert.AreEqual("Français", ResourceManager.GetString("LanguageFrench", culture));
            Assert.AreEqual("English", ResourceManager.GetString("LanguageEnglish", culture));
            Assert.AreEqual("Deutsch", ResourceManager.GetString("LanguageGerman", culture));
            Assert.AreEqual("Español", ResourceManager.GetString("LanguageSpanish", culture));
            Assert.AreEqual("日本語", ResourceManager.GetString("LanguageJapanese", culture));
            Assert.AreEqual("简体中文", ResourceManager.GetString("LanguageSimplifiedChinese", culture));
            Assert.AreEqual("Русский", ResourceManager.GetString("LanguageRussian", culture));
        }
    }

    [TestMethod]
    public void RussianCulture_ContainsLocalizedMenuLabels()
    {
        var culture = new CultureInfo("ru-RU");

        Assert.AreEqual("Язык", ResourceManager.GetString("MenuLanguages", culture));
        Assert.AreEqual("ВЫБОР ЯЗЫКА", ResourceManager.GetString("LanguageMenuTitle", culture));
        Assert.AreEqual("Текущий язык", ResourceManager.GetString("CurrentLanguageLabel", culture));
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
