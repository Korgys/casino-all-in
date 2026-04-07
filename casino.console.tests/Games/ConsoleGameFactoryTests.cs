using System.Reflection;
using casino.console.Games;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Slots;

namespace casino.console.tests.Games;

[TestClass]
public class ConsoleGameFactoryTests
{
    [TestMethod]
    public void Create_ReturnsNull_ForUnknownGame()
    {
        var factory = new ConsoleGameFactory();

        var result = factory.Create("roulette", _ => new GameAction(PokerTypeAction.Check), () => false);

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

    [TestMethod]
    public void CreatePoker_UsesCustomSetupForChipsAndStrategies()
    {
        var factory = new ConsoleGameFactory();
        var setup = new PokerGameSetup(2200, 7,
        [
            new PokerOpponentSetup(PokerDifficulty.Beginner),
            new PokerOpponentSetup(PokerDifficulty.VeryEasy),
            new PokerOpponentSetup(PokerDifficulty.Easy),
            new PokerOpponentSetup(PokerDifficulty.Medium),
            new PokerOpponentSetup(PokerDifficulty.Hard),
            new PokerOpponentSetup(PokerDifficulty.VeryHard)
        ]);

        var game = factory.CreatePoker(_ => new GameAction(PokerTypeAction.Check), () => false, setup);

        Assert.IsInstanceOfType<PokerGame>(game);

        var field = typeof(PokerGame).GetField("_players", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field);

        var players = (IReadOnlyList<Player>)field.GetValue(game)!;
        Assert.HasCount(7, players);
        Assert.IsTrue(players.All(player => player.Chips == 2200));
        Assert.IsInstanceOfType<HumanPlayer>(players[0]);

        for (var i = 1; i < players.Count; i++)
            Assert.IsInstanceOfType<AdaptiveStrategy>(((ComputerPlayer)players[i]).Strategy);
    }

    [TestMethod]
    public void Create_ReturnsBlackjackGame_ForBlackjackNameCaseInsensitive()
    {
        var factory = new ConsoleGameFactory();

        var result = factory.Create("BLACKJACK", _ => new GameAction(PokerTypeAction.Check), () => false);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<BlackjackGame>(result);
    }

    [TestMethod]
    public void Create_ReturnsSlotMachineGame_ForSlotAliases()
    {
        var factory = new ConsoleGameFactory();

        var result = factory.Create("slot machine", _ => new GameAction(PokerTypeAction.Check), () => false);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<SlotMachineGame>(result);
    }
}
