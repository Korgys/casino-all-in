using casino.core.Common.Events;

namespace casino.core.tests.Common.Events;

[TestClass]
public class GameEventArgsTests
{
    [TestMethod]
    public void GamePhaseEventArgs_ShouldExposePhase()
    {
        var args = new GamePhaseEventArgs("River");

        Assert.AreEqual("River", args.Phase);
    }

    [TestMethod]
    public void PotUpdatedEventArgs_ShouldExposePotAndCurrentBet()
    {
        var args = new PotUpdatedEventArgs(180, 40);

        Assert.AreEqual(180, args.Pot);
        Assert.AreEqual(40, args.CurrentBet);
    }
}
