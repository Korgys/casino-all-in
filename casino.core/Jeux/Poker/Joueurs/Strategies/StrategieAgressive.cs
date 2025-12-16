using casino.core.Jeux.Poker.Actions;
using System;
using System.Linq;

namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public class StrategieAgressive : IStrategieJoueur
{
    public Actions.Action ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var joueur = contexte.JoueurCourant;
        var relanceMinimum = Math.Max(contexte.Partie.MiseActuelle + contexte.Partie.MiseDeDepart, contexte.MiseMinimum);

        if (actions.Contains(TypeAction.Relancer) && joueur.Jetons > contexte.Partie.MiseActuelle)
        {
            int mise = Math.Min(joueur.Jetons, relanceMinimum);
            return new Actions.Action(TypeAction.Relancer, mise);
        }

        if (actions.Contains(TypeAction.Miser))
        {
            return new Actions.Action(TypeAction.Miser, contexte.MiseMinimum);
        }

        if (actions.Contains(TypeAction.Suivre))
        {
            return new Actions.Action(TypeAction.Suivre);
        }

        if (actions.Contains(TypeAction.Tapis))
        {
            return new Actions.Action(TypeAction.Tapis);
        }

        if (actions.Contains(TypeAction.Check))
        {
            return new Actions.Action(TypeAction.Check);
        }

        return new Actions.Action(TypeAction.SeCoucher);
    }
}
