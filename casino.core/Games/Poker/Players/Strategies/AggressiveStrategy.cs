using casino.core.Games.Poker.Actions;
using System;
using System.Linq;

namespace casino.core.Games.Poker.Players.Strategies;

public class AggressiveStrategy : IPlayerStrategy
{
    public Actions.GameAction DecideAction(GameContext context)
    {
        var actions = context.AvailableActions;
        var Player = context.CurrentPlayer;
        var relanceMinimum = Math.Max(context.Round.CurrentBet + context.Round.StartingBet, context.MinimumBet);

        if (actions.Contains(PokerTypeAction.Raise) && Player.Chips > context.Round.CurrentBet)
        {
            int mise = Math.Min(Player.Chips, relanceMinimum);
            return new Actions.GameAction(PokerTypeAction.Raise, mise);
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
