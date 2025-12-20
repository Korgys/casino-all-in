using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Jeux.Poker.Parties.Phases;

public abstract class PhaseStateBase : IPhaseState
{
    public abstract void Avancer(Partie context);

    public virtual IEnumerable<TypeActionJeu> ObtenirActionsPossibles(Joueur joueur, Partie context)
    {
        var actionsPossibles = new List<TypeActionJeu>();

        if (!joueur.EstCouche() && !joueur.EstTapis())
        {
            actionsPossibles.Add(TypeActionJeu.SeCoucher);

            if (joueur.Jetons >= context.MiseActuelle)
            {
                if (context.MiseActuelle == 0) actionsPossibles.Add(TypeActionJeu.Miser);
                else actionsPossibles.Add(TypeActionJeu.Suivre);
                actionsPossibles.Add(TypeActionJeu.Relancer);
            }
            else
            {
                actionsPossibles.Add(TypeActionJeu.Tapis);
            }

            if (context.MiseActuelle == 0)
            {
                actionsPossibles.Add(TypeActionJeu.Check);
            }
        }

        return actionsPossibles.OrderBy(a => (int)a).ToList();
    }

    public virtual void AppliquerAction(Joueur joueur, Actions.ActionJeu action, Partie context)
    {
        context.ActionService.ExecuterAction(context, joueur, action);
    }
}
