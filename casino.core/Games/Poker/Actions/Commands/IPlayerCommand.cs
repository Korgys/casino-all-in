using casino.core.Games.Poker.Parties;

namespace casino.core.Games.Poker.Actions.Commands;

public interface IPlayerCommand
{
    void Execute(Partie partie);
}
