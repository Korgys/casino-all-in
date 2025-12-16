using casino.core.Jeux.Poker.Actions;
using System;
using System.Linq;

namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public class StrategieAgressive : IStrategieJoueur
{
    public Actions.ActionJeu ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var joueur = contexte.JoueurCourant;
        var relanceMinimum = Math.Max(contexte.Partie.MiseActuelle + contexte.Partie.MiseDeDepart, contexte.MiseMinimum);

        if (actions.Contains(TypeActionJeu.Relancer) && joueur.Jetons > contexte.Partie.MiseActuelle)
        {
            int mise = Math.Min(joueur.Jetons, relanceMinimum);
            return new Actions.ActionJeu(TypeActionJeu.Relancer, mise);
        }

        if (actions.Contains(TypeActionJeu.Miser))
        {
            return new Actions.ActionJeu(TypeActionJeu.Miser, contexte.MiseMinimum);
        }

        if (actions.Contains(TypeActionJeu.Suivre))
        {
            return new Actions.ActionJeu(TypeActionJeu.Suivre);
        }

        if (actions.Contains(TypeActionJeu.Tapis))
        {
            return new Actions.ActionJeu(TypeActionJeu.Tapis);
        }

        if (actions.Contains(TypeActionJeu.Check))
        {
            return new Actions.ActionJeu(TypeActionJeu.Check);
        }

        return new Actions.ActionJeu(TypeActionJeu.SeCoucher);
    }
}
