using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Jeux.Poker.Parties.Phases;

public abstract class PhaseStateBase : IPhaseState
{
    public abstract void Avancer(Partie context);

    public virtual IEnumerable<JoueurActionType> ObtenirActionsPossibles(Joueur joueur, Partie context)
    {
        var actionsPossibles = new List<JoueurActionType>();

        if (!joueur.EstCouche)
        {
            actionsPossibles.Add(JoueurActionType.SeCoucher);

            if (context.MiseActuelle <= joueur.Jetons)
            {
                if (context.MiseActuelle == 0) actionsPossibles.Add(JoueurActionType.Miser);
                else actionsPossibles.Add(JoueurActionType.Suivre);
                actionsPossibles.Add(JoueurActionType.Relancer);
            }
            else
            {
                actionsPossibles.Add(JoueurActionType.Tapis);
            }

            if (context.MiseActuelle == 0)
            {
                actionsPossibles.Add(JoueurActionType.Check);
            }
        }

        return actionsPossibles.OrderBy(a => (int)a).ToList();
    }

    public virtual void AppliquerAction(Joueur joueur, JoueurAction action, Partie context)
    {
        if (!ObtenirActionsPossibles(joueur, context).Contains(action.TypeAction))
        {
            throw new InvalidOperationException("Action de joueur non autorisée");
        }

        switch (action.TypeAction)
        {
            case JoueurActionType.SeCoucher:
                context.TraiterCoucher(joueur);
                break;
            case JoueurActionType.Miser:
                context.TraiterMiser(joueur, action.Montant);
                break;
            case JoueurActionType.Suivre:
                context.TraiterSuivre(joueur);
                break;
            case JoueurActionType.Relancer:
                context.TraiterRelancer(joueur, action.Montant);
                break;
            case JoueurActionType.Tapis:
                context.TraiterTapis(joueur);
                break;
            case JoueurActionType.Check:
                context.TraiterCheck(joueur);
                break;
            default:
                throw new ArgumentException("Action de joueur invalide");
        }
    }
}
