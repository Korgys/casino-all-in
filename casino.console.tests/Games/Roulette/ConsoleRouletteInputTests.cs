using casino.console.Games.Roulette;
using casino.core.Games.Roulette;

namespace casino.console.tests.Games.Roulette;

[TestClass]
public class ConsoleRouletteInputTests
{
    [TestMethod]
    public void GetBet_ReturnsStraightNumberBet_AfterInvalidType()
    {
        var state = CreateState();
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            Console.SetIn(new StringReader("9\n1\n17\n4\n"));
            Console.SetOut(new StringWriter());

            var bet = ConsoleRouletteInput.GetBet(state);

            Assert.AreEqual(RouletteBetKind.Number, bet.Kind);
            Assert.AreEqual(17, bet.Number);
            Assert.AreEqual(4, bet.Amount);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    public void GetBet_ReturnsDefaultBet_WhenBackIsRequested()
    {
        var state = CreateState();
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            Console.SetIn(new StringReader("back\n"));
            Console.SetOut(new StringWriter());

            var bet = ConsoleRouletteInput.GetBet(state);

            Assert.AreEqual(RouletteBetKind.Red, bet.Kind);
            Assert.AreEqual(state.MinBet, bet.Amount);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    [DataRow("2", RouletteBetKind.Red)]
    [DataRow("3", RouletteBetKind.Black)]
    [DataRow("4", RouletteBetKind.Even)]
    [DataRow("5", RouletteBetKind.Odd)]
    public void GetBet_ReturnsOutsideBetKinds_ForMenuChoices(string choice, RouletteBetKind expectedKind)
    {
        var state = CreateState();
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            Console.SetIn(new StringReader($"{choice}\n6\n"));
            Console.SetOut(new StringWriter());

            var bet = ConsoleRouletteInput.GetBet(state);

            Assert.AreEqual(expectedKind, bet.Kind);
            Assert.AreEqual(6, bet.Amount);
            Assert.IsNull(bet.Number);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    public void GetBet_UsesZero_WhenBackIsRequestedDuringNumberPrompt()
    {
        var state = CreateState();
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            Console.SetIn(new StringReader("1\nback\n3\n"));
            Console.SetOut(new StringWriter());

            var bet = ConsoleRouletteInput.GetBet(state);

            Assert.AreEqual(RouletteBetKind.Number, bet.Kind);
            Assert.AreEqual(0, bet.Number);
            Assert.AreEqual(3, bet.Amount);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    public void GetBet_UsesMinimumBet_WhenBackIsRequestedDuringAmountPrompt()
    {
        var state = CreateState();
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            Console.SetIn(new StringReader("2\nback\n"));
            Console.SetOut(new StringWriter());

            var bet = ConsoleRouletteInput.GetBet(state);

            Assert.AreEqual(RouletteBetKind.Red, bet.Kind);
            Assert.AreEqual(state.MinBet, bet.Amount);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    public void AskContinueNewGame_ReturnsTrue_ForRussianYesAlias()
    {
        var originalIn = Console.In;

        try
        {
            Console.SetIn(new StringReader("da\n"));

            var result = ConsoleRouletteInput.AskContinueNewGame();

            Assert.IsTrue(result);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    private static RouletteGameState CreateState() => new()
    {
        Credits = 50,
        CurrentBet = 0,
        LastPayout = 0,
        MinBet = 1,
        MaxBet = 20,
        TotalSpins = 0,
        WinningSpins = 0,
        BiggestPayout = 0,
        CurrentPocket = null,
        BetKind = RouletteBetKind.Red,
        BetSummary = "Red",
        IsSpinning = false,
        IsRoundOver = false,
        IsWinningBet = false,
        StatusMessage = "Place your bet."
    };
}
