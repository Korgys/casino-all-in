using casino.console.Games.Poker;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Rounds.Phases;

namespace casino.console.tests.Games.Poker;

[TestClass]
public class ConsolePokerRendererProbabilityTests
{
    [TestCleanup]
    public void Cleanup()
    {
        ConsolePokerRenderer.ResetEstimateWinProbability();
    }

    [TestMethod]
    public void RenderTable_ReusesCachedProbability_WhenKeyDoesNotChange()
    {
        var calls = 0;
        ConsolePokerRenderer.SetEstimateWinProbabilityForTests((_, _, _, _) =>
        {
            calls++;
            return 55d;
        });

        var renderer = new ConsolePokerRenderer();
        var state = BuildState(phase: Phase.Flop.ToString(), includeSecondOpponent: false);
        var hero = state.Players.Single(p => p.Name == "Hero");

        WriteCurrentProbability(renderer, hero, state);
        WriteCurrentProbability(renderer, hero, state);

        Assert.AreEqual(1, calls, "Probability should be computed once for an unchanged cache key.");
    }

    [TestMethod]
    public void RenderTable_RecomputesProbability_WhenPhaseChanges()
    {
        var calls = 0;
        ConsolePokerRenderer.SetEstimateWinProbabilityForTests((_, _, _, _) =>
        {
            calls++;
            return 60d;
        });

        var renderer = new ConsolePokerRenderer();

        var flop = BuildState(phase: Phase.Flop.ToString(), includeSecondOpponent: false);
        var turn = BuildState(phase: Phase.Turn.ToString(), includeSecondOpponent: false);

        WriteCurrentProbability(renderer, flop.Players.Single(p => p.Name == "Hero"), flop);
        WriteCurrentProbability(renderer, turn.Players.Single(p => p.Name == "Hero"), turn);

        Assert.AreEqual(2, calls, "Changing phase should invalidate the probability cache key.");
    }

    [TestMethod]
    public void RenderTable_RecomputesProbability_WhenActiveOpponentsChange()
    {
        var calls = 0;
        ConsolePokerRenderer.SetEstimateWinProbabilityForTests((_, _, _, _) =>
        {
            calls++;
            return 65d;
        });

        var renderer = new ConsolePokerRenderer();

        var oneOpponent = BuildState(phase: Phase.Turn.ToString(), includeSecondOpponent: false);
        var twoOpponents = BuildState(phase: Phase.Turn.ToString(), includeSecondOpponent: true);

        WriteCurrentProbability(renderer, oneOpponent.Players.Single(p => p.Name == "Hero"), oneOpponent);
        WriteCurrentProbability(renderer, twoOpponents.Players.Single(p => p.Name == "Hero"), twoOpponents);

        Assert.AreEqual(2, calls, "Changing active opponents should invalidate the probability cache key.");
    }

    [TestMethod]
    public void RenderTable_UsesLowerSimulationCountByDefault_AndHigherCountWhenDetailedOddsEnabled()
    {
        var simulationCounts = new List<int>();
        ConsolePokerRenderer.SetEstimateWinProbabilityForTests((_, _, _, simulations) =>
        {
            simulationCounts.Add(simulations);
            return 70d;
        });

        var renderer = new ConsolePokerRenderer();
        var state = BuildState(phase: Phase.Flop.ToString(), includeSecondOpponent: false);
        var hero = state.Players.Single(p => p.Name == "Hero");

        WriteCurrentProbability(renderer, hero, state);
        renderer.DetailedOddsEnabled = true;
        var turn = BuildState(phase: Phase.Turn.ToString(), includeSecondOpponent: false);
        WriteCurrentProbability(renderer, turn.Players.Single(p => p.Name == "Hero"), turn);

        CollectionAssert.AreEqual(new[] { 500, 2000 }, simulationCounts);
    }

    [TestMethod]
    public void RenderTable_WithAsyncComputation_RendersPlaceholderWhileProbabilityIsPending()
    {
        var gate = new ManualResetEventSlim(false);
        ConsolePokerRenderer.SetEstimateWinProbabilityForTests((_, _, _, _) =>
        {
            gate.Wait(TimeSpan.FromSeconds(1));
            return 50d;
        });

        var renderer = new ConsolePokerRenderer
        {
            UseAsyncProbabilityComputation = true
        };

        var state = BuildState(phase: Phase.Flop.ToString(), includeSecondOpponent: false);
        var output = WriteCurrentProbability(renderer, state.Players.Single(p => p.Name == "Hero"), state);

        Assert.Contains("...", output, StringComparison.Ordinal);
        gate.Set();
    }

    private static string WriteCurrentProbability(ConsolePokerRenderer renderer, PokerPlayerState player, PokerGameState state)
    {
        var method = typeof(ConsolePokerRenderer)
            .GetMethod("WriteCurrentProbability", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(method, "Unable to access ConsolePokerRenderer.WriteCurrentProbability via reflection.");

        var originalOut = Console.Out;
        var writer = new StringWriter();
        try
        {
            Console.SetOut(writer);
            method.Invoke(renderer, [player, state]);
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    private static PokerGameState BuildState(string phase, bool includeSecondOpponent)
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

        var villain1 = new PokerPlayerState(
            Name: "Villain1",
            Chips: 1000,
            Contribution: 10,
            IsHuman: false,
            IsFolded: false,
            LastAction: PokerTypeAction.Call,
            Hand: new HandCards(new Card(CardRank.Dame, Suit.Clubs), new Card(CardRank.Valet, Suit.Clubs)),
            IsWinner: false);

        var villain2 = new PokerPlayerState(
            Name: "Villain2",
            Chips: 1000,
            Contribution: 10,
            IsHuman: false,
            IsFolded: !includeSecondOpponent,
            LastAction: PokerTypeAction.Fold,
            Hand: new HandCards(new Card(CardRank.Dix, Suit.Hearts), new Card(CardRank.Neuf, Suit.Hearts)),
            IsWinner: false);

        return new PokerGameState(
            Phase: phase,
            StartingBet: 10,
            Pot: 30,
            CurrentBet: 10,
            CommunityCards: new TableCards
            {
                Flop1 = new Card(CardRank.As, Suit.Diamonds),
                Flop2 = new Card(CardRank.Sept, Suit.Clubs),
                Flop3 = new Card(CardRank.Cinq, Suit.Spades)
            },
            Players: [hero, villain1, villain2],
            CurrentPlayer: "Hero");
    }
}
