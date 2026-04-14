using casino.console.Games.Commons;
using casino.console.Games.Roulette;
using casino.console.tests.Games.Commons;
using casino.core.Games.Roulette;

namespace casino.console.tests.Games.Roulette;

[TestClass]
public class ConsoleRouletteRendererTests
{
    [TestCleanup]
    public void Cleanup()
    {
        ConsoleRouletteRenderer.ResetPause();
        SetFrameBuffer(new ConsoleFrameBuffer(new SystemConsoleFrameTarget()));
    }

    [TestMethod]
    public void RenderTable_RendersDeterministicFrameLines()
    {
        var target = new FakeConsoleFrameTarget();
        SetFrameBuffer(new ConsoleFrameBuffer(target));
        ConsoleRouletteRenderer.SetPauseForTests(_ => { });

        ConsoleRouletteRenderer.RenderTable(BuildState("Wheel spinning...", pocket: 18));

        Assert.AreEqual(1, target.ClearCalls);
        Assert.IsTrue(target.Lines.Any(line => line.Contains("18 RED", StringComparison.Ordinal)));
        Assert.IsTrue(target.Lines.Any(line => line.Contains("Wheel spinning...", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void RenderTable_UsesPartialUpdates_OnSubsequentRender()
    {
        var target = new FakeConsoleFrameTarget();
        SetFrameBuffer(new ConsoleFrameBuffer(target));
        ConsoleRouletteRenderer.SetPauseForTests(_ => { });

        ConsoleRouletteRenderer.RenderTable(BuildState("Wheel spinning...", pocket: 18));
        var movesAfterFirstRender = target.CursorMoves.Count;

        ConsoleRouletteRenderer.RenderTable(BuildState("You win!", payout: 10, roundOver: true, pocket: 18));

        Assert.AreEqual(1, target.ClearCalls);
        Assert.IsGreaterThan(movesAfterFirstRender, target.CursorMoves.Count);
        Assert.IsTrue(target.Lines.Any(line => line.Contains("You win!", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void RenderTable_DoesNotThrow_WhenCursorPositioningIsUnavailable()
    {
        var target = new FakeConsoleFrameTarget
        {
            SupportsCursorPositioning = false
        };

        SetFrameBuffer(new ConsoleFrameBuffer(target));

        ConsoleRouletteRenderer.RenderTable(BuildState("Redirected output.", pocket: 0));
        ConsoleRouletteRenderer.RenderTable(BuildState("Still redirected.", pocket: 32));

        Assert.IsEmpty(target.CursorMoves);
    }

    [TestMethod]
    public void RenderTable_RendersGreenPocketAndAnimation_WhenRoundIsWon()
    {
        var target = new FakeConsoleFrameTarget();
        SetFrameBuffer(new ConsoleFrameBuffer(target));
        ConsoleRouletteRenderer.SetPauseForTests(_ => { });

        ConsoleRouletteRenderer.RenderTable(BuildState("Jackpot!", payout: 36, roundOver: true, pocket: 0));

        Assert.IsTrue(target.Lines.Any(line => line.Contains("0 GREEN", StringComparison.Ordinal)));
        Assert.IsTrue(target.Lines.Any(line => line.Contains("Jackpot!", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void RenderTable_RendersBlackPocket()
    {
        var target = new FakeConsoleFrameTarget();
        SetFrameBuffer(new ConsoleFrameBuffer(target));
        ConsoleRouletteRenderer.SetPauseForTests(_ => { });

        ConsoleRouletteRenderer.RenderTable(BuildState("Black pocket.", pocket: 2));

        Assert.IsTrue(target.Lines.Any(line => line.Contains("2 BLACK", StringComparison.Ordinal)));
    }

    private static RouletteGameState BuildState(string status, int payout = 0, bool roundOver = false, int? pocket = null)
    {
        return new RouletteGameState
        {
            Credits = 120,
            CurrentBet = 5,
            LastPayout = payout,
            MinBet = 1,
            MaxBet = 20,
            TotalSpins = 1,
            WinningSpins = payout > 0 ? 1 : 0,
            BiggestPayout = payout,
            CurrentPocket = pocket,
            BetKind = RouletteBetKind.Red,
            BetSummary = "Red",
            IsRoundOver = roundOver,
            IsWinningBet = payout > 0,
            IsSpinning = !roundOver,
            StatusMessage = status
        };
    }

    private static void SetFrameBuffer(ConsoleFrameBuffer frameBuffer)
    {
        var field = typeof(ConsoleRouletteRenderer).GetField("FrameBuffer", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field.SetValue(null, frameBuffer);
    }
}
