using casino.core.Games.Poker.Actions;

namespace casino.core.Games.Poker.Players.Strategies;

public class AggressiveStrategy : IPlayerStrategy
{
    public Actions.GameAction DecideAction(GameContext context)
    {
        var actions = context.AvailableActions;
        var player = context.CurrentPlayer;
        var minimumRaise = Math.Max(context.Round.CurrentBet + context.Round.StartingBet, context.MinimumBet);

        if (actions.Contains(PokerTypeAction.Raise) && player.Chips > context.Round.CurrentBet)
        {
            int amount = Math.Min(player.Chips, minimumRaise);
            return new Actions.GameAction(PokerTypeAction.Raise, amount);
        }

        if (actions.Contains(PokerTypeAction.Bet))
        {
            return new Actions.GameAction(PokerTypeAction.Bet, context.MinimumBet);
        }

        if (actions.Contains(PokerTypeAction.Call))
        {
            return new Actions.GameAction(PokerTypeAction.Call);
        }

        if (actions.Contains(PokerTypeAction.AllIn))
        {
            return new Actions.GameAction(PokerTypeAction.AllIn);
        }

        if (actions.Contains(PokerTypeAction.Check))
        {
            return new Actions.GameAction(PokerTypeAction.Check);
        }

        return new Actions.GameAction(PokerTypeAction.Fold);
    }
}
