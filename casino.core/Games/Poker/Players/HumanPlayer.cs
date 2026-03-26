namespace casino.core.Games.Poker.Players;

/// <summary>
/// Represents a human poker player.
/// </summary>
/// <param name="name">The player name.</param>
/// <param name="chips">The starting chip count.</param>
public class HumanPlayer(string name, int chips) : Player(name, chips)
{
}
