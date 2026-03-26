using casino.core.Games.Poker.Players.Strategies;

namespace casino.core.Games.Poker.Players;

/// <summary>
/// Represents an AI-controlled poker player.
/// </summary>
public class ComputerPlayer : Player
{
    public IPlayerStrategy Strategy { get; }

    /// <summary>
    /// Initializes a new computer player.
    /// </summary>
    /// <param name="name">The player name.</param>
    /// <param name="chips">The starting chip count.</param>
    /// <param name="strategy">The strategy used to choose actions.</param>
    public ComputerPlayer(string name, int chips, IPlayerStrategy? strategy = null) : base(name, chips)
    {
        Strategy = strategy ?? new RandomStrategy();
    }
}
