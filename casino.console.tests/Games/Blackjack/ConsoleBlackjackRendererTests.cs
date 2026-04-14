using casino.console.Games.Blackjack;
using casino.console.Games.Commons;
using casino.console.tests.Games.Commons;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker.Cards;

namespace casino.console.tests.Games.Blackjack;

[TestClass]
public class ConsoleBlackjackRendererTests
{
    [TestCleanup]
    public void Cleanup()
    {
        ConsoleBlackjackRenderer.ResetPause();
        SetFrameBuffer(new ConsoleFrameBuffer(new SystemConsoleFrameTarget()));
    }

    [TestMethod]
    public void RenderTable_RendersDeterministicFrameLines()
    {
        var target = new FakeConsoleFrameTarget();
        SetFrameBuffer(new ConsoleFrameBuffer(target));
        ConsoleBlackjackRenderer.SetPauseForTests(_ => { });

        ConsoleBlackjackRenderer.RenderTable(BuildState(status: "Player stands."));

        Assert.AreEqual(1, target.ClearCalls);
        Assert.IsTrue(target.Lines.Any(line => line.Contains("BLACKJACK", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(target.Lines.Any(line => line.Contains("Player stands.", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void RenderTable_UsesPartialUpdates_OnSubsequentRender()
    {
        var target = new FakeConsoleFrameTarget();
        SetFrameBuffer(new ConsoleFrameBuffer(target));
        ConsoleBlackjackRenderer.SetPauseForTests(_ => { });

        var first = BuildState(status: "Player hits.");
        var second = BuildState(status: "Player busts.", outcome: BlackjackRoundOutcome.DealerWin);

        ConsoleBlackjackRenderer.RenderTable(first);
        var cursorMovesAfterFirstRender = target.CursorMoves.Count;

        ConsoleBlackjackRenderer.RenderTable(second);

        Assert.AreEqual(1, target.ClearCalls, "Console.Clear should happen only on initial draw when dimensions are stable.");
        Assert.IsGreaterThan(cursorMovesAfterFirstRender, target.CursorMoves.Count, "Second render should use cursor movement for partial updates.");
        Assert.IsTrue(target.Lines.Any(line => line.Contains("Player busts.", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void RenderTable_DoesNotThrow_WhenCursorPositioningIsUnavailable()
    {
        var target = new FakeConsoleFrameTarget
        {
            SupportsCursorPositioning = false
        };

        SetFrameBuffer(new ConsoleFrameBuffer(target));

        ConsoleBlackjackRenderer.RenderTable(BuildState(status: "Safe redirected output."));
        ConsoleBlackjackRenderer.RenderTable(BuildState(status: "Second redirected render."));

        Assert.IsEmpty(target.CursorMoves, "No cursor movement should be attempted when output is non-interactive.");
    }

    private static BlackjackGameState BuildState(string status, BlackjackRoundOutcome outcome = BlackjackRoundOutcome.InProgress)
    {
        return new BlackjackGameState
        {
            PlayerCards = [new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts)],
            DealerCards = [new Card(CardRank.Neuf, Suit.Clubs), new Card(CardRank.Sept, Suit.Diamonds)],
            IsDealerHoleCardHidden = false,
            IsRoundOver = false,
            StatusMessage = status,
            RoundOutcome = outcome,
            PlayerWins = 2,
            DealerWins = 1,
            Pushes = 0
        };
    }

    private static void SetFrameBuffer(ConsoleFrameBuffer frameBuffer)
    {
        var field = typeof(ConsoleBlackjackRenderer).GetField("FrameBuffer", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field.SetValue(null, frameBuffer);
    }
}
