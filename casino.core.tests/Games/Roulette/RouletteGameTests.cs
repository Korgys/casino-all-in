using casino.core.Common.Utils;
using casino.core.Games.Roulette;

namespace casino.core.tests.Games.Roulette;

[TestClass]
public class RouletteGameTests
{
    [TestMethod]
    public void Constructor_ThrowsForInvalidConfiguration()
    {
        Assert.Throws<ArgumentNullException>(() => new RouletteGame(null!, () => false));
        Assert.Throws<ArgumentNullException>(() => new RouletteGame(_ => new RouletteBet(RouletteBetKind.Red, 1), null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RouletteGame(_ => new RouletteBet(RouletteBetKind.Red, 1), () => false, startingCredits: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RouletteGame(_ => new RouletteBet(RouletteBetKind.Red, 1), () => false, animationFrames: 0));
    }

    [TestMethod]
    public void Run_TracksWinningAndLosingRounds_ForMixedBetTypes()
    {
        var random = new SequenceRandom(11, 18, 7, 8, 9, 22);
        var continueCalls = 0;
        var states = new List<RouletteGameState>();
        var betCalls = 0;

        var game = new RouletteGame(
            _ => betCalls++ == 0
                ? new RouletteBet(RouletteBetKind.Number, 5, 7)
                : new RouletteBet(RouletteBetKind.Red, 4),
            () => continueCalls++ == 0,
            random,
            _ => { },
            startingCredits: 20,
            animationFrames: 2);

        game.StateUpdated += (_, e) => states.Add((RouletteGameState)e.State);

        game.Run();

        var roundResults = states.Where(state => state.IsRoundOver).ToList();
        Assert.HasCount(3, roundResults);
        Assert.AreEqual(180, roundResults[0].LastPayout);
        Assert.AreEqual(195, roundResults[0].Credits);
        Assert.AreEqual(0, roundResults[1].LastPayout);
        Assert.AreEqual(191, roundResults[1].Credits);
        Assert.AreEqual(2, roundResults[1].TotalSpins);
        Assert.AreEqual(1, roundResults[1].WinningSpins);
        Assert.AreEqual(180, roundResults[1].BiggestPayout);
    }

    [TestMethod]
    public void Run_ClampsRequestedAmount_AndNormalizesInvalidNumber()
    {
        var random = new SequenceRandom(1, 2, 3, 1);
        RouletteGameState? roundResult = null;

        var game = new RouletteGame(
            _ => new RouletteBet(RouletteBetKind.Number, 99, 99),
            () => false,
            random,
            _ => { },
            startingCredits: 3,
            animationFrames: 3);

        game.StateUpdated += (_, e) =>
        {
            var state = (RouletteGameState)e.State;
            if (state.IsRoundOver && state.TotalSpins == 1)
                roundResult = state;
        };

        game.Run();

        Assert.IsNotNull(roundResult);
        Assert.AreEqual(0, roundResult.SelectedNumber);
        Assert.AreEqual(0, roundResult.LastPayout);
        Assert.AreEqual(0, roundResult.Credits);
    }

    [TestMethod]
    [DataRow(RouletteBetKind.Black, 2, 10)]
    [DataRow(RouletteBetKind.Even, 4, 10)]
    [DataRow(RouletteBetKind.Odd, 9, 10)]
    public void Run_PaysOutsideBets_ForWinningPockets(RouletteBetKind kind, int pocket, int expectedPayout)
    {
        var random = new SequenceRandom(1, pocket);
        RouletteGameState? roundResult = null;

        var game = new RouletteGame(
            _ => new RouletteBet(kind, 5),
            () => false,
            random,
            _ => { },
            animationFrames: 1);

        game.StateUpdated += (_, e) =>
        {
            var state = (RouletteGameState)e.State;
            if (state.IsRoundOver && state.TotalSpins == 1)
                roundResult = state;
        };

        game.Run();

        Assert.IsNotNull(roundResult);
        Assert.AreEqual(expectedPayout, roundResult.LastPayout);
        Assert.AreEqual(95 + expectedPayout, roundResult.Credits);
        Assert.IsTrue(roundResult.IsWinningBet);
    }

    [TestMethod]
    public void Run_LosesOutsideBet_WhenPocketIsZero()
    {
        var random = new SequenceRandom(5, 0);
        RouletteGameState? roundResult = null;

        var game = new RouletteGame(
            _ => new RouletteBet(RouletteBetKind.Red, 5),
            () => false,
            random,
            _ => { },
            animationFrames: 1);

        game.StateUpdated += (_, e) =>
        {
            var state = (RouletteGameState)e.State;
            if (state.IsRoundOver && state.TotalSpins == 1)
                roundResult = state;
        };

        game.Run();

        Assert.IsNotNull(roundResult);
        Assert.AreEqual(0, roundResult.LastPayout);
        Assert.AreEqual("0 green. No win on this spin.", roundResult.StatusMessage);
        Assert.IsFalse(roundResult.IsWinningBet);
    }

    [TestMethod]
    public void Run_UsesDefaultRedBet_WhenSelectorReturnsNull()
    {
        var random = new SequenceRandom(2, 1);
        RouletteGameState? roundResult = null;

        var game = new RouletteGame(
            _ => null!,
            () => false,
            random,
            _ => { },
            startingCredits: 4,
            animationFrames: 1);

        game.StateUpdated += (_, e) =>
        {
            var state = (RouletteGameState)e.State;
            if (state.IsRoundOver && state.TotalSpins == 1)
                roundResult = state;
        };

        game.Run();

        Assert.IsNotNull(roundResult);
        Assert.AreEqual(RouletteBetKind.Red, roundResult.BetKind);
        Assert.AreEqual(1, roundResult.CurrentBet);
        Assert.AreEqual(5, roundResult.Credits);
    }

    [TestMethod]
    public void Run_NormalizesUnknownBetKind_ToRed()
    {
        var random = new SequenceRandom(4, 3);
        RouletteGameState? roundResult = null;

        var game = new RouletteGame(
            _ => new RouletteBet((RouletteBetKind)99, 5),
            () => false,
            random,
            _ => { },
            animationFrames: 1);

        game.StateUpdated += (_, e) =>
        {
            var state = (RouletteGameState)e.State;
            if (state.IsRoundOver && state.TotalSpins == 1)
                roundResult = state;
        };

        game.Run();

        Assert.IsNotNull(roundResult);
        Assert.AreEqual(RouletteBetKind.Red, roundResult.BetKind);
        Assert.AreEqual(10, roundResult.LastPayout);
    }

    private sealed class SequenceRandom(params int[] values) : IRandom
    {
        private readonly Queue<int> _values = new(values);

        public int Next(int maxExclusive)
        {
            Assert.IsNotEmpty(_values, "No more random values available.");
            var value = _values.Dequeue();
            Assert.IsTrue(value >= 0 && value < maxExclusive, $"Value {value} is outside 0..{maxExclusive - 1}.");
            return value;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            Assert.IsNotEmpty(_values, "No more random values available.");
            var value = _values.Dequeue();
            Assert.IsTrue(value >= minInclusive && value < maxExclusive, $"Value {value} is outside {minInclusive}..{maxExclusive - 1}.");
            return value;
        }
    }
}
