using casino.core.Games.Poker.Actions;
using System;
using System.Linq;

namespace casino.core.Games.Poker.Players.Strategies;

public class AggressiveStrategy : IPlayerStrategy
{
    public Actions.GameAction DecideAction(GameContext contexte)
    {
        var actions = contexte.AvailableActions;
        var Player = contexte.CurrentPlayer;
        var relanceMinimum = Math.Max(contexte.Round.CurrentBet + contexte.Round.StartingBet, contexte.MinimumBet);

        if (actions.Contains(PokerTypeAction.Raise) && Player.Chips > contexte.Round.CurrentBet)
        {
            int mise = Math.Min(Player.Chips, relanceMinimum);
            return new Actions.GameAction(PokerTypeAction.Raise, mise);
        }

        if (actions.Contains(PokerTypeAction.Bet))
        {
            return new Actions.GameAction(PokerTypeAction.Bet, contexte.MinimumBet);
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
