using casino.console.Games;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;

namespace casino.console.tests.Games;

[TestClass]
public class ConsoleGameFactoryTests
{
    [TestMethod]
    public void Create_ReturnsNull_ForUnknownGame()
    {
        var factory = new ConsoleGameFactory();

        var result = factory.Create("blackjack", _ => new GameAction(PokerTypeAction.Check), () => false);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Create_ReturnsPokerGame_ForPokerNameCaseInsensitive()
    {
        var factory = new ConsoleGameFactory();

        var result = factory.Create("PoKeR", _ => new GameAction(PokerTypeAction.Check), () => false);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<PokerGame>(result);
    }
}
