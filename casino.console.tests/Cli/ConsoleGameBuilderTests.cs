using System.Reflection;
using casino.console.Cli;
using casino.console.Games;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Roulette;
using casino.core.Games.Slots;

namespace casino.console.tests.Cli;

[TestClass]
public class ConsoleGameBuilderTests
{
    [TestMethod]
    public void Create_PokerWithSetup_ConfiguresPlayers()
    {
        var setup = new PokerGameSetup(1000, 4,
        [
            new PokerOpponentSetup(PokerDifficulty.Medium),
            new PokerOpponentSetup(PokerDifficulty.Medium),
            new PokerOpponentSetup(PokerDifficulty.Medium)
        ]);
        var command = new CasinoCliCommand(CasinoGameKind.Poker, setup);

        var game = ConsoleGameBuilder.Create(new ConsoleGameFactory(), command);

        Assert.IsInstanceOfType<PokerGame>(game);
        var players = GetPokerPlayers((PokerGame)game);
        Assert.HasCount(4, players);
        Assert.IsInstanceOfType<HumanPlayer>(players[0]);
        Assert.IsTrue(players.All(player => player.Chips == 1000));
        Assert.IsTrue(players.Skip(1).Cast<ComputerPlayer>().All(player => player.Strategy is AdaptiveStrategy));
    }

    [TestMethod]
    public void Create_BlackjackCommand_ReturnsBlackjackGame()
    {
        var command = new CasinoCliCommand(CasinoGameKind.Blackjack, PokerSetup: null);

        var game = ConsoleGameBuilder.Create(new ConsoleGameFactory(), command);

        Assert.IsInstanceOfType<BlackjackGame>(game);
    }

    [TestMethod]
    public void Create_SlotMachineCommand_ReturnsSlotMachineGame()
    {
        var command = new CasinoCliCommand(CasinoGameKind.SlotMachine, PokerSetup: null);

        var game = ConsoleGameBuilder.Create(new ConsoleGameFactory(), command);

        Assert.IsInstanceOfType<SlotMachineGame>(game);
    }

    [TestMethod]
    public void Create_RouletteCommand_ReturnsRouletteGame()
    {
        var command = new CasinoCliCommand(CasinoGameKind.Roulette, PokerSetup: null);

        var game = ConsoleGameBuilder.Create(new ConsoleGameFactory(), command);

        Assert.IsInstanceOfType<RouletteGame>(game);
    }

    private static IReadOnlyList<Player> GetPokerPlayers(PokerGame game)
    {
        var field = typeof(PokerGame).GetField("_players", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field);

        return (IReadOnlyList<Player>)field.GetValue(game)!;
    }
}
