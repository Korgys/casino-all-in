using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Poker.Players;

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

    public bool IsFolded() => LastAction == PokerTypeAction.Fold;
    public bool IsAllIn() => LastAction == PokerTypeAction.AllIn;
    public PokerTypeAction LastAction { get; internal set; }

    public Player(string name, int chips)
    {
        Name = name;
        Chips = chips;
    }

    internal void Reset()
    {
        LastAction = Chips > 0 ? PokerTypeAction.None : PokerTypeAction.Fold;
    }
}
