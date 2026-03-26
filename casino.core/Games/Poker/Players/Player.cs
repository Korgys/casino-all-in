using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Poker.Players;

/// <summary>
/// Represents a poker player.
/// </summary>
public class Player
{
    public string Name { get; }

    private int _chips;
    public int Chips
    {
        get => _chips;
        internal set => _chips = value > 0 ? value : 0;
    }

    public HandCards Hand { get; set; } = null!;

    /// <summary>
    /// Returns whether the player has folded.
    /// </summary>
    /// <returns><see langword="true"/> when the player folded; otherwise <see langword="false"/>.</returns>
    public bool IsFolded() => LastAction == PokerTypeAction.Fold;

    /// <summary>
    /// Returns whether the player is all-in.
    /// </summary>
    /// <returns><see langword="true"/> when the player is all-in; otherwise <see langword="false"/>.</returns>
    public bool IsAllIn() => LastAction == PokerTypeAction.AllIn;
    public PokerTypeAction LastAction { get; internal set; }

    /// <summary>
    /// Initializes a new player.
    /// </summary>
    /// <param name="name">The player name.</param>
    /// <param name="chips">The starting chip count.</param>
    public Player(string name, int chips)
    {
        Name = name;
        Chips = chips;
    }

    /// <summary>
    /// Resets player action state for a new round.
    /// </summary>
    internal void Reset()
    {
        LastAction = Chips > 0 ? PokerTypeAction.None : PokerTypeAction.Fold;
    }
}
