using casino.core.Games.Poker.Actions;

namespace casino.core.tests.Games.Poker.Actions;

[TestClass]
public class ActionRequestTests
{
    [TestMethod]
    public void Constructor_ShouldMapAllProperties()
    {
        var availableActions = new[] { PokerTypeAction.Check, PokerTypeAction.Bet, PokerTypeAction.Fold };
        var tableState = new object();

        var request = new ActionRequest(
            "Bob",
            availableActions,
            minimumBet: 10,
            currentBet: 20,
            pot: 150,
            tableState);

        Assert.AreEqual("Bob", request.PlayerName);
        CollectionAssert.AreEqual(availableActions, request.AvailableActions.ToArray());
        Assert.AreEqual(10, request.MinimumBet);
        Assert.AreEqual(20, request.CurrentBet);
        Assert.AreEqual(150, request.Pot);
        Assert.AreSame(tableState, request.TableState);
    }
}
