using casino.console.Games.Slots;
using casino.core.Games.Slots;

namespace casino.console.tests.Games.Slots;

[TestClass]
public class ConsoleSlotMachineInputTests
{
    [TestMethod]
    public void GetBet_ReturnsEnteredBet_AfterInvalidInput()
    {
        var state = CreateState();
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            Console.SetIn(new StringReader("abc\n99\n4\n"));
            Console.SetOut(new StringWriter());

            var bet = ConsoleSlotMachineInput.GetBet(state);

            Assert.AreEqual(4, bet);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    public void GetBet_ReturnsMinimumBet_WhenBackIsRequested()
    {
        var state = CreateState();
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            Console.SetIn(new StringReader("back\n"));
            Console.SetOut(new StringWriter());

            var bet = ConsoleSlotMachineInput.GetBet(state);

            Assert.AreEqual(state.MinBet, bet);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    public void AskContinueNewGame_ReturnsTrue_ForChineseYesAlias()
    {
        var originalIn = Console.In;

        try
        {
            Console.SetIn(new StringReader("是\n"));

            var result = ConsoleSlotMachineInput.AskContinueNewGame();

            Assert.IsTrue(result);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    private static SlotMachineGameState CreateState() => new()
    {
        Reels = [SlotSymbol.Cherry, SlotSymbol.Bell, SlotSymbol.Seven],
        Credits = 25,
        CurrentBet = 0,
        LastPayout = 0,
        MinBet = 1,
        MaxBet = 10,
        TotalSpins = 0,
        WinningSpins = 0,
        BiggestPayout = 0,
        IsSpinning = false,
        IsRoundOver = false,
        IsJackpot = false,
        StatusMessage = "Bet"
    };
}
