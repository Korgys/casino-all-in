namespace casino.core.Games.Poker.Actions;

public class GameAction
{
    public PokerTypeAction TypeAction { get; set; }
    public int Amount { get; set; } // Montant associé à l'action (si applicable)
    public GameAction(PokerTypeAction typeAction, int amount = 0)
    {
        TypeAction = typeAction;
        Amount = amount;
    }
}
