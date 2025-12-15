using System.Linq;
using casino.core.Jeux.Poker.Scores;

namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public class StrategieConservatrice : IJoueurStrategy
{
    public JoueurAction ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var score = contexte.ScoreJoueur;

        if (actions.Contains(JoueurActionType.Check))
        {
            return new JoueurAction(JoueurActionType.Check);
        }

        if (actions.Contains(JoueurActionType.Suivre) && (score.Rang >= RangMain.DoublePaire || contexte.Partie.MiseActuelle <= contexte.MiseMinimum))
        {
            return new JoueurAction(JoueurActionType.Suivre);
        }

        if (actions.Contains(JoueurActionType.Miser) && score.Rang >= RangMain.Paire)
        {
            return new JoueurAction(JoueurActionType.Miser, contexte.MiseMinimum);
        }

        if (actions.Contains(JoueurActionType.Relancer) && score.Rang >= RangMain.Full)
        {
            return new JoueurAction(JoueurActionType.Relancer, contexte.MiseMinimum);
        }

        if (actions.Contains(JoueurActionType.SeCoucher))
        {
            return new JoueurAction(JoueurActionType.SeCoucher);
        }

        return new JoueurAction(actions.First());
    }
}
