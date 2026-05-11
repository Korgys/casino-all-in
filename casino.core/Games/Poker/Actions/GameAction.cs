namespace casino.core.Games.Poker.Actions;

public class GameAction
{
    public PokerTypeAction TypeAction { get; set; }

    /// <summary>
    /// Gets or sets the action amount.
    /// For <see cref="PokerTypeAction.Bet"/> and <see cref="PokerTypeAction.Raise"/>, this is the target total contribution
    /// the player will have in the current betting round after the action is applied.
    /// For <see cref="PokerTypeAction.Call"/>, <see cref="PokerTypeAction.Check"/>, <see cref="PokerTypeAction.Fold"/>,
    /// and <see cref="PokerTypeAction.AllIn"/>, this must be zero.
    /// </summary>
    public int Amount { get; set; }

    public GameAction(PokerTypeAction typeAction, int amount = 0)
    {
        TypeAction = typeAction;
        Amount = amount;
    }
}
