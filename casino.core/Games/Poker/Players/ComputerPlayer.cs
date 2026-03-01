using casino.core.Games.Poker.Players.Strategies;

namespace casino.core.Games.Poker.Players;

public class ComputerPlayer : Player
{
    public IPlayerStrategy Strategy { get; }

    public ComputerPlayer(string name, int chips, IPlayerStrategy? strategie = null) : base(name, chips)
    {
        Strategy = strategie ?? new RandomStrategy();
    }
}
