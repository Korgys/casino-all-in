using casino.core.Games.Poker.Actions;

namespace casino.core.Games.Poker.Players.Strategies;

public interface IPlayerStrategy
{
    Actions.GameAction DecideAction(GameContext contexte);
}
