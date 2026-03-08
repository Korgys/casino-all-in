using System.Reflection;
using System.Diagnostics;
using casino.console.Games.Poker;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;

namespace casino.console.tests.Games.Poker;

[TestClass]
public class ConsolePokerRendererTests
{
    [TestMethod]
    public void NewRound_DoesNotReuseProbabilityFromPreviousRound()
    {
        var renderer = new ConsolePokerRenderer();

        var alice = CreatePlayer("Alice", isFolded: false);
        var foldedBob = CreatePlayer("Bob", isFolded: true);
        var firstRoundState = CreateState(new[] { alice, foldedBob }, currentPlayer: "Alice", phase: "PreFlop");

        InvokeWriteScoreAndProbabilityOfVictory(renderer, alice, firstRoundState, currentPlayerName: "Alice");

        renderer.ResetRoundCache();

        var output = new StringWriter();
        var originalOut = Console.Out;

        try
        {
            Console.SetOut(output);
            var activeBob = CreatePlayer("Bob", isFolded: false);
            var newRoundState = CreateState(new[] { alice, activeBob }, currentPlayer: "Bob", phase: "PreFlop");
            InvokeWriteScoreAndProbabilityOfVictory(renderer, alice, newRoundState, currentPlayerName: "Bob");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.DoesNotContain(output.ToString(), "%", "A new round should not reuse a cached probability from a previous round.");
    }

    [TestMethod]
    public void CalculateProbabilityOfVictory_ReturnsNull_ForKnownInvalidState()
    {
        var player = CreatePlayer("Alice", isFolded: false);
        var communityCards = new TableCards {
            Flop1 = new Card(CardRank.As, Suit.Hearts),
        };
        var state = CreateState(new[] { player, CreatePlayer("Bob", isFolded: false) }, "Alice", "Flop", communityCards);

        var probability = InvokeCalculateProbabilityOfVictory(player, state);

        Assert.IsNull(probability, "Known invalid game states should degrade gracefully.");
    }

    [TestMethod]
    public void CalculateProbabilityOfVictory_UnexpectedError_IsNotSilentlySwallowed()
    {
        var originalEstimator = GetEstimateWinProbabilityDelegate();
        SetEstimateWinProbabilityDelegate((_, _, _, _) => throw new NotSupportedException("boom"));

        try
        {
            var player = CreatePlayer("Alice", isFolded: false);
            var state = CreateState(new[] { player, CreatePlayer("Bob", isFolded: false) }, "Alice", "Turn");

#if DEBUG
            var ex = Assert.Throws<TargetInvocationException>(() => InvokeCalculateProbabilityOfVictory(player, state));
            Assert.IsInstanceOfType<NotSupportedException>(ex.InnerException);
#else
            var listener = new CaptureTraceListener();
            Trace.Listeners.Add(listener);
            try
            {
                var probability = InvokeCalculateProbabilityOfVictory(player, state);
                Assert.IsNull(probability);
                Assert.Contains(listener.Messages, "Unexpected error when estimating poker win probability", StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                Trace.Listeners.Remove(listener);
            }
#endif
        }
        finally
        {
            SetEstimateWinProbabilityDelegate(originalEstimator);
        }
    }

    private static PokerPlayerState CreatePlayer(string name, bool isFolded)
    {
        return new PokerPlayerState(
            Name: name,
            Chips: 100,
            Contribution: 0,
            IsHuman: true,
            IsFolded: isFolded,
            LastAction: PokerTypeAction.None,
            Hand: new HandCards(new Card(CardRank.As, Suit.Hearts), new Card(CardRank.Roi, Suit.Hearts)),
            IsWinner: false);
    }

    private static PokerGameState CreateState(
        IReadOnlyList<PokerPlayerState> players,
        string currentPlayer,
        string phase,
        TableCards? communityCards = null)
    {
        return new PokerGameState(
            Phase: phase,
            StartingBet: 10,
            Pot: 20,
            CurrentBet: 10,
            CommunityCards: communityCards ?? new TableCards(),
            Players: players,
            CurrentPlayer: currentPlayer);
    }

    private static double? InvokeCalculateProbabilityOfVictory(PokerPlayerState player, PokerGameState state)
    {
        var method = typeof(ConsolePokerRenderer).GetMethod(
            "CalculateProbabilityOfVictory",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(method, "Could not find probability calculation method.");
        return (double?)method.Invoke(null, new object[] { player, state });
    }

    private static Func<HandCards, TableCards, int, int, double> GetEstimateWinProbabilityDelegate()
    {
        var field = typeof(ConsolePokerRenderer).GetField("EstimateWinProbability", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(field, "Could not find estimate win probability delegate field.");
        return (Func<HandCards, TableCards, int, int, double>)field.GetValue(null)!;
    }

    private static void SetEstimateWinProbabilityDelegate(Func<HandCards, TableCards, int, int, double> estimator)
    {
        var field = typeof(ConsolePokerRenderer).GetField("EstimateWinProbability", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(field, "Could not find estimate win probability delegate field.");
        field.SetValue(null, estimator);
    }

    private static void InvokeWriteScoreAndProbabilityOfVictory(
        ConsolePokerRenderer renderer,
        PokerPlayerState player,
        PokerGameState state,
        string currentPlayerName)
    {
        var method = typeof(ConsolePokerRenderer).GetMethod(
            "WriteScoreAndProbabilityOfVictory",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method, "Could not find score/probability rendering method.");
        method.Invoke(renderer, new object[] { player, state, currentPlayerName });
    }

#if !DEBUG
    private sealed class CaptureTraceListener : TraceListener
    {
        public string Messages { get; private set; } = string.Empty;

        public override void Write(string? message)
        {
            Messages += message;
        }

        public override void WriteLine(string? message)
        {
            Messages += message + Environment.NewLine;
        }
    }
#endif
}
