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

            Assert.AreEqual("\u0414\u0438\u043B\u0435\u0440", Resources.BlackjackDealer);
            Assert.AreEqual("\u041E\u043B\u043B-\u0438\u043D", Resources.AllIn);
            Assert.AreEqual("\u0421\u0442\u0430\u0440\u0448\u0430\u044F \u043A\u0430\u0440\u0442\u0430", Resources.HighCard);
        }
        finally
        {
            Resources.Culture = previousCulture;
        }
    }
}
