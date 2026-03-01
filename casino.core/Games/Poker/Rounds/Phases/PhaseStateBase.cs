using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Games.Poker.Rounds.Phases;

public abstract class PhaseStateBase : IPhaseState
{
    public abstract void Avancer(Round context);

    public virtual IEnumerable<PokerTypeAction> GetAvailableActions(Player Player, Round context)
    {
        var actionsPossibles = new List<PokerTypeAction>();

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
                actionsPossibles.Add(PokerTypeAction.Fold);
            }
            else if (Player.Chips <= context.StartingBet)
            {
                actionsPossibles.Add(PokerTypeAction.AllIn);
            }
            else
            {
                actionsPossibles.Add(PokerTypeAction.Bet);
                actionsPossibles.Add(PokerTypeAction.Raise);
                actionsPossibles.Add(PokerTypeAction.AllIn);
            }
            return actionsPossibles.OrderBy(a => (int)a).ToList();
        }

        actionsPossibles.Add(PokerTypeAction.Fold);

        if (difference > 0)
        {
            if (difference < Player.Chips)
            {
                actionsPossibles.Add(PokerTypeAction.Call);
                actionsPossibles.Add(PokerTypeAction.Raise);
                actionsPossibles.Add(PokerTypeAction.AllIn);
            }
            else if (difference == Player.Chips)
            {
                actionsPossibles.Add(PokerTypeAction.AllIn);
            }
            else
            {
                actionsPossibles.Add(PokerTypeAction.AllIn);
            }
        }
        else // difference <= 0, aucune mise � rattraper
        {
            actionsPossibles.Add(PokerTypeAction.Check);

            if (context.CurrentBet == 0)
            {
                actionsPossibles.Add(PokerTypeAction.Bet);
            }

            if (Player.Chips > 0 && (context.CurrentBet == 0 || Player.Chips + misePlayer > context.CurrentBet))
            {
                actionsPossibles.Add(PokerTypeAction.Raise);
            }
        }

        return actionsPossibles.OrderBy(a => (int)a).ToList();
    }

    public virtual void ApplyAction(Player Player, Actions.GameAction action, Round context)
    {
        context.ActionService.ExecuterAction(context, Player, action);
    }
}