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

    [TestMethod]
    public void Calculate_ReturnsBarMultiplier_ForTripleBar()
    {
        var payout = SlotMachinePayoutCalculator.Calculate([SlotSymbol.Bar, SlotSymbol.Bar, SlotSymbol.Bar], 5);

        Assert.AreEqual(60, payout);
    }

    [TestMethod]
    public void Calculate_ReturnsBellMultiplier_ForTripleBell()
    {
        var payout = SlotMachinePayoutCalculator.Calculate([SlotSymbol.Bell, SlotSymbol.Bell, SlotSymbol.Bell], 5);

        Assert.AreEqual(50, payout);
    }

    [TestMethod]
    public void Calculate_ReturnsStarMultiplier_ForTripleStar()
    {
        var payout = SlotMachinePayoutCalculator.Calculate([SlotSymbol.Star, SlotSymbol.Star, SlotSymbol.Star], 5);

        Assert.AreEqual(40, payout);
    }

    [TestMethod]
    public void Calculate_ReturnsZero_WhenNoCherryAndNoTripleMatch()
    {
        var payout = SlotMachinePayoutCalculator.Calculate([SlotSymbol.Bar, SlotSymbol.Bell, SlotSymbol.Star], 5);

        Assert.AreEqual(0, payout);
    }

    [TestMethod]
    public void Calculate_ThrowsArgumentNullException_WhenReelsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => SlotMachinePayoutCalculator.Calculate(null!, 5));
    }

    [TestMethod]
    public void Calculate_ThrowsArgumentException_WhenReelsCountIsNotThree()
    {
        Assert.Throws<ArgumentException>(() => SlotMachinePayoutCalculator.Calculate([SlotSymbol.Seven, SlotSymbol.Seven], 5));
    }

    [TestMethod]
    public void Calculate_ThrowsArgumentOutOfRangeException_WhenBetIsZeroOrNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SlotMachinePayoutCalculator.Calculate([SlotSymbol.Seven, SlotSymbol.Seven, SlotSymbol.Seven], 0));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SlotMachinePayoutCalculator.Calculate([SlotSymbol.Seven, SlotSymbol.Seven, SlotSymbol.Seven], -1));
    }
}
