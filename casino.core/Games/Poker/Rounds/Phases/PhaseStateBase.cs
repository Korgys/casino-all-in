using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Games.Poker.Rounds.Phases;

public abstract class PhaseStateBase : IPhaseState
{
    public abstract void Avancer(Round context);

    public virtual IEnumerable<TypeGameAction> GetAvailableActions(Player Player, Round context)
    {
        var actionsPossibles = new List<TypeGameAction>();

        // Un Player couché ou tapis n'a pas d'actions possibles
        if (Player.IsFolded() || Player.IsAllIn())
            return actionsPossibles;

        // Calcul de la diff�rence entre la mise actuelle et la mise du Player
        var misePlayer = context.GetBetFor(Player);
        var difference = context.CurrentBet - misePlayer;

        // G�re le dealer (premi�re mise du PreFlop)
        if (context.Phase == Phase.PreFlop && context.CurrentBet == 0 && misePlayer == 0)
        {
            if (Player.Chips <= 0)
            {
                actionsPossibles.Add(TypeGameAction.SeCoucher);
            }
            else if (Player.Chips <= context.StartingBet)
            {
                actionsPossibles.Add(TypeGameAction.Tapis);
            }
            else
            {
                actionsPossibles.Add(TypeGameAction.Miser);
                actionsPossibles.Add(TypeGameAction.Relancer);
                actionsPossibles.Add(TypeGameAction.Tapis);
            }
            return actionsPossibles.OrderBy(a => (int)a).ToList();
        }

        actionsPossibles.Add(TypeGameAction.SeCoucher);

        if (difference > 0)
        {
            if (difference < Player.Chips)
            {
                actionsPossibles.Add(TypeGameAction.Suivre);
                actionsPossibles.Add(TypeGameAction.Relancer);
                actionsPossibles.Add(TypeGameAction.Tapis);
            }
            else if (difference == Player.Chips)
            {
                actionsPossibles.Add(TypeGameAction.Tapis);
            }
            else
            {
                actionsPossibles.Add(TypeGameAction.Tapis);
            }
        }
        else // difference <= 0, aucune mise � rattraper
        {
            actionsPossibles.Add(TypeGameAction.Check);

            if (context.CurrentBet == 0)
            {
                actionsPossibles.Add(TypeGameAction.Miser);
            }

            if (Player.Chips > 0 && (context.CurrentBet == 0 || Player.Chips + misePlayer > context.CurrentBet))
            {
                actionsPossibles.Add(TypeGameAction.Relancer);
            }
        }

        return actionsPossibles.OrderBy(a => (int)a).ToList();
    }

    public virtual void ApplyAction(Player Player, Actions.GameAction action, Round context)
    {
        context.ActionService.ExecuterAction(context, Player, action);
    }
}