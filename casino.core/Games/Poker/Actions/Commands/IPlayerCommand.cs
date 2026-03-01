using casino.core.Games.Poker.Rounds;

namespace casino.core.Games.Poker.Actions.Commands;

public interface IPlayerCommand
{
    void Execute(Round round);
}
