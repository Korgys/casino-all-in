using System.Linq;
using casino.console.Games.Poker;
using casino.core;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;

namespace casino.console.Games;

/// <summary>
/// Provides a factory for creating console-based game instances that implement the IGame interface.
/// </summary>
public class ConsoleGameFactory : IGameFactory
{
    public IGame? Create(string gameName, Func<ActionRequest, GameAction> humanActionSelector, Func<bool> continuePlaying)
    {
        return gameName.ToLower() switch
        {
            "poker" => CreatePoker(humanActionSelector, continuePlaying),
            _ => null
        };
    }

    public IGame CreatePoker(Func<ActionRequest, GameAction> humanActionSelector, Func<bool> continuePlaying)
    {
        var Player = new HumanPlayer("Player", 1000);
        var Players = new List<Player>
        {
            Player,
            new ComputerPlayer("Ordi Opportuniste", 1000, new OpportunisticStrategy()),
            new ComputerPlayer("Ordi Agressif", 1000, new AggressiveStrategy()),
            new ComputerPlayer("Ordi Conserv", 1000, new ConservativeStrategy()),
            new ComputerPlayer("Ordi Random", 1000, new RandomStrategy())
        };

        return new PokerGame(Players, () => new Deck(), humanActionSelector, continuePlaying, new ConsoleWaitStrategy());
    }
}
