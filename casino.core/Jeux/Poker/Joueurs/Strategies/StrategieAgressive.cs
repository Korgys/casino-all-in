using System;
using System.Linq;

namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public class StrategieAgressive : IJoueurStrategy
{
    public JoueurAction ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var joueur = contexte.JoueurCourant;
        var relanceMinimum = Math.Max(contexte.Partie.MiseActuelle + contexte.Partie.MiseDeDepart, contexte.MiseMinimum);

        if (actions.Contains(JoueurActionType.Relancer) && joueur.Jetons > contexte.Partie.MiseActuelle)
        {
            int mise = Math.Min(joueur.Jetons, relanceMinimum);
            return new JoueurAction(JoueurActionType.Relancer, mise);
        }

        if (actions.Contains(JoueurActionType.Miser))
        {
            return new JoueurAction(JoueurActionType.Miser, contexte.MiseMinimum);
        }

        if (actions.Contains(JoueurActionType.Suivre))
        {
            return new JoueurAction(JoueurActionType.Suivre);
        }

        if (actions.Contains(JoueurActionType.Tapis))
        {
            return new JoueurAction(JoueurActionType.Tapis);
        }

        if (actions.Contains(JoueurActionType.Check))
        {
            return new JoueurAction(JoueurActionType.Check);
        }

        return new JoueurAction(JoueurActionType.SeCoucher);
    }
}
