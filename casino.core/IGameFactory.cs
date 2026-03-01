using casino.core.Games.Poker.Actions;

namespace casino.core;

public interface IGameFactory
{
    IGame? Create(string gameName, Func<ActionRequest, GameAction> humanActionSelector, Func<bool> continuePlaying);
    IGame CreatePoker(Func<ActionRequest, GameAction> humanActionSelector, Func<bool> continuePlaying);
}
