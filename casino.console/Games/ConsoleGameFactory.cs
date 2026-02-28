using System.Linq;
using casino.core;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;

namespace casino.console.Games;

/// <summary>
/// Provides a factory for creating console-based game instances that implement the IGame interface.
/// </summary>
public class ConsoleGameFactory : IGameFactory
{
    public IGame? Create(string gameName, Func<RequeteAction, ActionJeu> humanActionSelector, Func<bool> continuePlaying)
    {
        return gameName.ToLower() switch
        {
            "poker" => CreatePoker(humanActionSelector, continuePlaying),
            _ => null
        };
    }

    public IGame CreatePoker(Func<RequeteAction, ActionJeu> humanActionSelector, Func<bool> continuePlaying)
    {
        var Player = new PlayerHumain("Player", 1000);
        var Players = new List<Player>
        {
            Player,
            new PlayerOrdi("Ordi Opportuniste", 1000, new StrategieOpportuniste()),
            new PlayerOrdi("Ordi Agressif", 1000, new StrategieAgressive()),
            new PlayerOrdi("Ordi Conserv", 1000, new StrategieConservatrice()),
            new PlayerOrdi("Ordi Random", 1000, new StrategieRandom())
        };

        return new PokerGame(Players, () => new JeuDeCartes(), humanActionSelector, continuePlaying);
    }
}
