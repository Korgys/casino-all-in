using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Jeux.Poker.Parties.Phases;

public abstract class PhaseStateBase : IPhaseState
{
    public abstract void Avancer(Partie context);

    public virtual IEnumerable<TypeAction> ObtenirActionsPossibles(Joueur joueur, Partie context)
    {
        var actionsPossibles = new List<TypeAction>();

        if (!joueur.EstCouche)
        {
            actionsPossibles.Add(TypeAction.SeCoucher);

            if (context.MiseActuelle <= joueur.Jetons)
            {
                if (context.MiseActuelle == 0) actionsPossibles.Add(TypeAction.Miser);
                else actionsPossibles.Add(TypeAction.Suivre);
                actionsPossibles.Add(TypeAction.Relancer);
            }
            else
            {
                actionsPossibles.Add(TypeAction.Tapis);
            }

            if (context.MiseActuelle == 0)
            {
                actionsPossibles.Add(TypeAction.Check);
            }
        }

        return actionsPossibles.OrderBy(a => (int)a).ToList();
    }

    public virtual void AppliquerAction(Joueur joueur, Actions.Action action, Partie context)
    {
        context.ActionService.ExecuterAction(context, joueur, action);
    }
}
