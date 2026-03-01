using casino.core.Games.Poker.Actions;
using System;
using System.Linq;

namespace casino.core.Games.Poker.Players.Strategies;

public class AggressiveStrategy : IPlayerStrategy
{
    public Actions.GameAction ProposerAction(GameContext contexte)
    {
        var actions = contexte.AvailableActions;
        var Player = contexte.CurrentPlayer;
        var relanceMinimum = Math.Max(contexte.Round.CurrentBet + contexte.Round.StartingBet, contexte.MinimumBet);

        if (actions.Contains(TypeGameAction.Relancer) && Player.Chips > contexte.Round.CurrentBet)
        {
            int mise = Math.Min(Player.Chips, relanceMinimum);
            return new Actions.GameAction(TypeGameAction.Relancer, mise);
        }

        if (actions.Contains(TypeGameAction.Miser))
        {
            return new Actions.GameAction(TypeGameAction.Miser, contexte.MinimumBet);
        }

        if (actions.Contains(TypeGameAction.Suivre))
        {
            return new Actions.GameAction(TypeGameAction.Suivre);
        }

        if (actions.Contains(TypeGameAction.Tapis))
        {
            return new Actions.GameAction(TypeGameAction.Tapis);
        }

        if (actions.Contains(TypeGameAction.Check))
        {
            return new Actions.GameAction(TypeGameAction.Check);
        }

        return new Actions.GameAction(TypeGameAction.SeCoucher);
    }
}
