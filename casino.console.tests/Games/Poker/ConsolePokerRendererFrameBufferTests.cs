using casino.console.Games.Commons;
using casino.console.Games.Poker;
using casino.console.tests.Games.Commons;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Rounds.Phases;

namespace casino.console.tests.Games.Poker;

[TestClass]
public class ConsolePokerRendererFrameBufferTests
{
    [TestMethod]
    public void RenderTable_RendersDeterministicLines()
    {
        var target = new FakeConsoleFrameTarget();
        var renderer = new ConsolePokerRenderer(new ConsoleFrameBuffer(target));

        renderer.RenderTable(BuildState(currentPlayer: "Hero"));

        Assert.AreEqual(1, target.ClearCalls);
        Assert.IsTrue(target.Lines.Any(line => line.Contains("Pot", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(target.Lines.Any(line => line.Contains("Hero", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void RenderTable_UsesCursorPartialUpdates_OnSubsequentRender()
    {
        var target = new FakeConsoleFrameTarget();
        var renderer = new ConsolePokerRenderer(new ConsoleFrameBuffer(target));

        renderer.RenderTable(BuildState(currentPlayer: "Hero"));
        var movesAfterFirstRender = target.CursorMoves.Count;

        renderer.RenderTable(BuildState(currentPlayer: "Villain"));

        Assert.AreEqual(1, target.ClearCalls);
        Assert.IsGreaterThan(movesAfterFirstRender, target.CursorMoves.Count);
    }

    [TestMethod]
    public void RenderTable_DoesNotThrow_ForRedirectedOutput()
    {
        var target = new FakeConsoleFrameTarget
        {
            SupportsCursorPositioning = false
        };

        var renderer = new ConsolePokerRenderer(new ConsoleFrameBuffer(target));

        renderer.RenderTable(BuildState(currentPlayer: "Hero"));
        renderer.RenderTable(BuildState(currentPlayer: "Villain"));

        Assert.IsEmpty(target.CursorMoves);
    }

    [TestMethod]
    public void RenderTable_WhenRoundResetsToPreFlop_ForcesFullFrameReset()
    {
        var target = new FakeConsoleFrameTarget();
        var renderer = new ConsolePokerRenderer(new ConsoleFrameBuffer(target));

        renderer.RenderTable(BuildState(currentPlayer: "Hero", phase: Phase.Flop));
        renderer.RenderTable(BuildState(currentPlayer: "Hero", phase: Phase.PreFlop));

        Assert.AreEqual(2, target.ClearCalls);
    }

    private static PokerGameState BuildState(string currentPlayer, Phase phase = Phase.Flop)
    {
        var hero = new PokerPlayerState(
            Name: "Hero",
            Chips: 1000,
            Contribution: 10,
            IsHuman: true,
            IsFolded: false,
            LastAction: PokerTypeAction.None,
            Hand: new HandCards(new Card(CardRank.As, Suit.Hearts), new Card(CardRank.Roi, Suit.Spades)),
            IsWinner: false);

        var villain = new PokerPlayerState(
            Name: "Villain",
            Chips: 1000,
            Contribution: 10,
            IsHuman: false,
            IsFolded: false,
            LastAction: PokerTypeAction.Check,
            Hand: new HandCards(new Card(CardRank.Dame, Suit.Clubs), new Card(CardRank.Valet, Suit.Clubs)),
            IsWinner: false);

        return new PokerGameState(
            Phase: phase.ToString(),
            StartingBet: 10,
            Pot: 30,
            CurrentBet: 10,
            CommunityCards: new TableCards
            {
                Flop1 = new Card(CardRank.As, Suit.Diamonds),
                Flop2 = new Card(CardRank.Sept, Suit.Clubs),
                Flop3 = new Card(CardRank.Cinq, Suit.Spades)
            },
            Players: [hero, villain],
            CurrentPlayer: currentPlayer);
    }
}
