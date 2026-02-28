using casino.core.Games.Poker.Players.Strategies;

namespace casino.core.Games.Poker.Players;

public class PlayerOrdi : Player
{
    public IStrategiePlayer Strategie { get; }

    public PlayerOrdi(string name, int chips, IStrategiePlayer? strategie = null) : base(name, chips)
    {
        Strategie = strategie ?? new StrategieRandom();
    }
}
