using System.Linq;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Scores;

namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public class StrategieConservatrice : IStrategieJoueur
{
    public Actions.ActionJeu ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var score = contexte.ScoreJoueur;

        if (actions.Contains(TypeActionJeu.Check))
        {
            return new Actions.ActionJeu(TypeActionJeu.Check);
        }

        if (actions.Contains(TypeActionJeu.Suivre) && (score.Rang >= RangMain.DoublePaire || contexte.Partie.MiseActuelle <= contexte.MiseMinimum))
        {
            return new Actions.ActionJeu(TypeActionJeu.Suivre);
        }

        if (actions.Contains(TypeActionJeu.Miser) && score.Rang >= RangMain.Paire)
        {
            return new Actions.ActionJeu(TypeActionJeu.Miser, contexte.MiseMinimum);
        }

        if (actions.Contains(TypeActionJeu.Relancer) && score.Rang >= RangMain.Full)
        {
            return new Actions.ActionJeu(TypeActionJeu.Relancer, contexte.MiseMinimum);
        }

        if (actions.Contains(TypeActionJeu.SeCoucher))
        {
            return new Actions.ActionJeu(TypeActionJeu.SeCoucher);
        }

        return new Actions.ActionJeu(actions.First());
    }
}
