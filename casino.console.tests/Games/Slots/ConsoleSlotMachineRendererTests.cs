using casino.console.Games.Commons;
using casino.console.Games.Slots;
using casino.console.tests.Games.Commons;
using casino.core.Games.Slots;

namespace casino.console.tests.Games.Slots;

[TestClass]
public class ConsoleSlotMachineRendererTests
{
    [TestCleanup]
    public void Cleanup()
    {
        ConsoleSlotMachineRenderer.ResetPause();
        SetFrameBuffer(new ConsoleFrameBuffer(new SystemConsoleFrameTarget()));
    }

    [TestMethod]
    public void RenderTable_RendersDeterministicFrameLines()
    {
        var target = new FakeConsoleFrameTarget();
        SetFrameBuffer(new ConsoleFrameBuffer(target));
        ConsoleSlotMachineRenderer.SetPauseForTests(_ => { });

        ConsoleSlotMachineRenderer.RenderTable(BuildState("Spinning..."));

        Assert.AreEqual(1, target.ClearCalls);
        Assert.IsTrue(target.Lines.Any(line => line.Contains("[🍒]", StringComparison.Ordinal)));
        Assert.IsTrue(target.Lines.Any(line => line.Contains("Spinning...", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void RenderTable_UsesPartialUpdates_OnSubsequentRender()
    {
        var target = new FakeConsoleFrameTarget();
        SetFrameBuffer(new ConsoleFrameBuffer(target));
        ConsoleSlotMachineRenderer.SetPauseForTests(_ => { });

        ConsoleSlotMachineRenderer.RenderTable(BuildState("Spinning..."));
        var movesAfterFirstRender = target.CursorMoves.Count;

        ConsoleSlotMachineRenderer.RenderTable(BuildState("Win!", payout: 20, roundOver: true));

        Assert.AreEqual(1, target.ClearCalls);
        Assert.IsTrue(target.CursorMoves.Count > movesAfterFirstRender);
        Assert.IsTrue(target.Lines.Any(line => line.Contains("Win!", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void RenderTable_DoesNotThrow_WhenCursorPositioningIsUnavailable()
    {
        var target = new FakeConsoleFrameTarget
        {
            SupportsCursorPositioning = false
        };

        SetFrameBuffer(new ConsoleFrameBuffer(target));

        ConsoleSlotMachineRenderer.RenderTable(BuildState("Redirected output."));
        ConsoleSlotMachineRenderer.RenderTable(BuildState("Still redirected."));

        Assert.AreEqual(0, target.CursorMoves.Count);
    }

    private static SlotMachineGameState BuildState(string status, int payout = 0, bool roundOver = false)
    {
        return new SlotMachineGameState
        {
            Reels = [SlotSymbol.Cherry, SlotSymbol.Bell, SlotSymbol.Seven],
            Credits = 120,
            CurrentBet = 5,
            LastPayout = payout,
            TotalSpins = 1,
            WinningSpins = payout > 0 ? 1 : 0,
            BiggestPayout = payout,
            IsRoundOver = roundOver,
            IsJackpot = false,
            IsSpinning = !roundOver,
            StatusMessage = status
        };
    }

    private static void SetFrameBuffer(ConsoleFrameBuffer frameBuffer)
    {
        var field = typeof(ConsoleSlotMachineRenderer).GetField("FrameBuffer", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field.SetValue(null, frameBuffer);
    }
}
