using System.Globalization;
using System.Resources;

namespace casino.core.tests.Localization;

[TestClass]
public class CoreResourcesRouletteLocalizationTests
{
    private static readonly ResourceManager ResourceManager = new("casino.core.Properties.Languages.Resources", typeof(casino.core.GameBase).Assembly);

    [TestMethod]
    public void RouletteKeys_ArePresent_InEverySupportedCulture()
    {
        var cultures = new[] { "en", "fr-FR", "de-DE", "es-ES", "ja-JP", "zh-Hans", "ru-RU" };
        var keys = new[] { "RouletteWelcome", "RouletteWheelSpinning", "RouletteBetRed", "RouletteColorBlack" };

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
