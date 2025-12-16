using casino.core.Jeux.Poker.Actions;

namespace casino.core;

public interface IGameFactory
{
    IGame? Create(string gameName, Func<RequeteAction, ActionJeu> humanActionSelector, Func<bool> continuePlaying);
    IGame CreatePoker(Func<RequeteAction, ActionJeu> humanActionSelector, Func<bool> continuePlaying);
}
