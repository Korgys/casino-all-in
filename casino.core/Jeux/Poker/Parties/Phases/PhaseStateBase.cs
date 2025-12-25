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

        // Un joueur couché ou tapis n'a pas d'actions possibles
        if (joueur.EstCouche() || joueur.EstTapis())
            return actionsPossibles;

        // Calcul de la différence entre la mise actuelle et la mise du joueur
        var miseJoueur = context.ObtenirMisePour(joueur);
        var difference = context.MiseActuelle - miseJoueur;

        // Gère le dealer (première mise du PreFlop)
        if (context.Phase == Phase.PreFlop && context.MiseActuelle == 0 && miseJoueur == 0)
        {
            if (joueur.Jetons <= 0)
            {
                actionsPossibles.Add(TypeActionJeu.SeCoucher);
            }
            else if (joueur.Jetons <= context.MiseDeDepart)
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
            if (difference < joueur.Jetons)
            {
                actionsPossibles.Add(TypeActionJeu.Suivre);
                actionsPossibles.Add(TypeActionJeu.Relancer);
                actionsPossibles.Add(TypeActionJeu.Tapis);
            }
            else if (difference == joueur.Jetons)
            {
                actionsPossibles.Add(TypeActionJeu.Tapis);
            }
            else
            {
                actionsPossibles.Add(TypeActionJeu.Tapis);
            }
        }
        else // difference <= 0, aucune mise à rattraper
        {
            actionsPossibles.Add(TypeActionJeu.Check);

            if (context.MiseActuelle == 0)
            {
                actionsPossibles.Add(TypeActionJeu.Miser);
            }

            if (joueur.Jetons > 0 && (context.MiseActuelle == 0 || joueur.Jetons + miseJoueur > context.MiseActuelle))
            {
                actionsPossibles.Add(TypeActionJeu.Relancer);
            }
        }

        return actionsPossibles.OrderBy(a => (int)a).ToList();
    }

    public virtual void AppliquerAction(Joueur joueur, Actions.ActionJeu action, Partie context)
    {
        context.ActionService.ExecuterAction(context, joueur, action);
    }
}