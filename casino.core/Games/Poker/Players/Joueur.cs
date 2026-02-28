using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;

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

    public HandCards Hand { get; set; }

    public bool IsFolded() => LastAction == TypeActionJeu.SeCoucher;
    public bool IsAllIn() => LastAction == TypeActionJeu.Tapis;
    public TypeActionJeu LastAction { get; internal set; }

    public Player(string name, int chips)
    {
        Name = name;
        Chips = chips;
    }

    internal void Reset()
    {
        LastAction = Chips > 0 ? TypeActionJeu.Aucune : TypeActionJeu.SeCoucher;
    }
}
