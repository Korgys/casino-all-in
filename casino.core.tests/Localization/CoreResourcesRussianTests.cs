using System.Globalization;
using casino.core.Properties.Languages;

namespace casino.core.tests.Localization;

[TestClass]
public class CoreResourcesRussianTests
{
    [TestMethod]
    public void RussianCulture_ReturnsLocalizedGameplayText()
    {
        var previousCulture = Resources.Culture;

        try
        {
            Resources.Culture = new CultureInfo("ru-RU");

            Assert.AreEqual("Дилер", Resources.BlackjackDealer);
            Assert.AreEqual("Олл-ин", Resources.AllIn);
            Assert.AreEqual("Старшая карта", Resources.HighCard);
        }
        finally
        {
            Resources.Culture = previousCulture;
        }
    }
}
