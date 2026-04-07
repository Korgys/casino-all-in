using casino.core.Games.Poker;

namespace casino.core.tests.Games.Poker;

[TestClass]
public class PokerSetupTests
{
    [TestMethod]
    public void PokerGameSetup_Constructor_ShouldMapAllProperties()
    {
        var opponents = new List<PokerOpponentSetup>
        {
            new(PokerDifficulty.Beginner),
            new(PokerDifficulty.Hard)
        };

        var setup = new PokerGameSetup(1200, 3, opponents);

        Assert.AreEqual(1200, setup.InitialChips);
        Assert.AreEqual(3, setup.PlayerCount);
        Assert.AreSame(opponents, setup.Opponents);
    }

    [TestMethod]
    [DataRow(PokerDifficulty.Beginner, "Débutant")]
    [DataRow(PokerDifficulty.VeryEasy, "Très facile")]
    [DataRow(PokerDifficulty.Easy, "Facile")]
    [DataRow(PokerDifficulty.Medium, "Moyen")]
    [DataRow(PokerDifficulty.Hard, "Difficile")]
    [DataRow(PokerDifficulty.VeryHard, "Très difficile")]
    public void PokerOpponentSetup_Label_ShouldMapEachKnownDifficulty(PokerDifficulty difficulty, string expectedLabel)
    {
        var opponentSetup = new PokerOpponentSetup(difficulty);

        Assert.AreEqual(difficulty, opponentSetup.Difficulty);
        Assert.AreEqual(expectedLabel, opponentSetup.Label);
    }

    [TestMethod]
    public void PokerOpponentSetup_Label_WithOutOfRangeDifficulty_ShouldFallbackToToString()
    {
        var unknownDifficulty = (PokerDifficulty)999;
        var opponentSetup = new PokerOpponentSetup(unknownDifficulty);

        Assert.AreEqual(unknownDifficulty, opponentSetup.Difficulty);
        Assert.AreEqual(unknownDifficulty.ToString(), opponentSetup.Label);
    }
}
