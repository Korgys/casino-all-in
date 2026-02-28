using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Games.Poker.Parties.Phases;

public abstract class PhaseStateBase : IPhaseState
{
    public abstract void Avancer(Partie context);

    public virtual IEnumerable<TypeActionJeu> ObtenirActionsPossibles(Player Player, Partie context)
    {
        var actionsPossibles = new List<TypeActionJeu>();

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
                actionsPossibles.Add(TypeActionJeu.SeCoucher);
            }
            else if (Player.Chips <= context.StartingBet)
            {
                actionsPossibles.Add(TypeActionJeu.Tapis);
            }
            else
            {
                actionsPossibles.Add(TypeActionJeu.Miser);
                actionsPossibles.Add(TypeActionJeu.Relancer);
                actionsPossibles.Add(TypeActionJeu.Tapis);
            }
            return actionsPossibles.OrderBy(a => (int)a).ToList();
        }

        actionsPossibles.Add(TypeActionJeu.SeCoucher);

        if (difference > 0)
        {
            if (difference < Player.Chips)
            {
                actionsPossibles.Add(TypeActionJeu.Suivre);
                actionsPossibles.Add(TypeActionJeu.Relancer);
                actionsPossibles.Add(TypeActionJeu.Tapis);
            }
            else if (difference == Player.Chips)
            {
                actionsPossibles.Add(TypeActionJeu.Tapis);
            }
            else
            {
                actionsPossibles.Add(TypeActionJeu.Tapis);
            }
        }
        else // difference <= 0, aucune mise � rattraper
        {
            actionsPossibles.Add(TypeActionJeu.Check);

            if (context.CurrentBet == 0)
            {
                actionsPossibles.Add(TypeActionJeu.Miser);
            }

            if (Player.Chips > 0 && (context.CurrentBet == 0 || Player.Chips + misePlayer > context.CurrentBet))
            {
                actionsPossibles.Add(TypeActionJeu.Relancer);
            }
        }

        return actionsPossibles.OrderBy(a => (int)a).ToList();
    }

    public virtual void AppliquerAction(Player Player, Actions.ActionJeu action, Partie context)
    {
        context.ActionService.ExecuterAction(context, Player, action);
    }
}