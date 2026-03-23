using casino.core.Common.Utils;
using casino.core.Games.Slots;

namespace casino.core.tests.Games.Slots;

[TestClass]
public class SlotMachineGameTests
{
    [TestMethod]
    public void Run_TracksCreditsAndWinningStats_AcrossTwoRounds()
    {
        var random = new SequenceRandom(5, 5, 5, 0, 1, 2, 0, 0, 0, 6, 1, 2);
        var continueCalls = 0;
        var states = new List<SlotMachineGameState>();

        var game = new SlotMachineGame(
            _ => 5,
            () => continueCalls++ == 0,
            random,
            _ => { },
            startingCredits: 20,
            animationFrames: 1);

        game.StateUpdated += (_, e) => states.Add((SlotMachineGameState)e.State);

        game.Run();

        var roundResults = states.Where(state => state.IsRoundOver).ToList();
        Assert.HasCount(3, roundResults);
        Assert.AreEqual(5, roundResults[0].LastPayout);
        Assert.AreEqual(20, roundResults[0].Credits);
        Assert.AreEqual(0, roundResults[1].LastPayout);
        Assert.AreEqual(15, roundResults[1].Credits);
        Assert.AreEqual(2, roundResults[1].TotalSpins);
        Assert.AreEqual(1, roundResults[1].WinningSpins);
        Assert.AreEqual(5, roundResults[1].BiggestPayout);
    }

    [TestMethod]
    public void Run_ClampsRequestedBet_ToAvailableCredits()
    {
        var random = new SequenceRandom(1, 2, 3, 0, 1, 2);
        SlotMachineGameState? finalSpin = null;

        var game = new SlotMachineGame(
            _ => 99,
            () => false,
            random,
            _ => { },
            startingCredits: 3,
            animationFrames: 1);

        game.StateUpdated += (_, e) =>
        {
            var state = (SlotMachineGameState)e.State;
            if (state.IsRoundOver)
                finalSpin = state;
        };

        game.Run();

        Assert.IsNotNull(finalSpin);
        Assert.AreEqual(0, finalSpin.CurrentBet);
        Assert.AreEqual(3, finalSpin.Credits);
        Assert.AreEqual(0, finalSpin.LastPayout);
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
