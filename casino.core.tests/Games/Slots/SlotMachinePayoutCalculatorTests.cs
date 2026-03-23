using casino.core.Games.Slots;

namespace casino.core.tests.Games.Slots;

[TestClass]
public class SlotMachinePayoutCalculatorTests
{
    [TestMethod]
    public void Calculate_ReturnsJackpot_ForTripleSeven()
    {
        var payout = SlotMachinePayoutCalculator.Calculate([SlotSymbol.Seven, SlotSymbol.Seven, SlotSymbol.Seven], 5);

        Assert.AreEqual(100, payout);
    }

    [TestMethod]
    public void Calculate_ReturnsCherryMultiplier_ForMixedCherryCombinations()
    {
        var singleCherry = SlotMachinePayoutCalculator.Calculate([SlotSymbol.Cherry, SlotSymbol.Bell, SlotSymbol.Bar], 4);
        var doubleCherry = SlotMachinePayoutCalculator.Calculate([SlotSymbol.Cherry, SlotSymbol.Star, SlotSymbol.Cherry], 4);
        var tripleCherry = SlotMachinePayoutCalculator.Calculate([SlotSymbol.Cherry, SlotSymbol.Cherry, SlotSymbol.Cherry], 4);

        Assert.AreEqual(4, singleCherry);
        Assert.AreEqual(8, doubleCherry);
        Assert.AreEqual(24, tripleCherry);
    }
}
