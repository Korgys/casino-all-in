using System.Linq;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Scores;

namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public class StrategieConservatrice : IStrategieJoueur
{
    public Actions.Action ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var score = contexte.ScoreJoueur;

        if (actions.Contains(TypeAction.Check))
        {
            return new Actions.Action(TypeAction.Check);
        }

        if (actions.Contains(TypeAction.Suivre) && (score.Rang >= RangMain.DoublePaire || contexte.Partie.MiseActuelle <= contexte.MiseMinimum))
        {
            return new Actions.Action(TypeAction.Suivre);
        }

        if (actions.Contains(TypeAction.Miser) && score.Rang >= RangMain.Paire)
        {
            return new Actions.Action(TypeAction.Miser, contexte.MiseMinimum);
        }

        if (actions.Contains(TypeAction.Relancer) && score.Rang >= RangMain.Full)
        {
            return new Actions.Action(TypeAction.Relancer, contexte.MiseMinimum);
        }

        if (actions.Contains(TypeAction.SeCoucher))
        {
            return new Actions.Action(TypeAction.SeCoucher);
        }

        return new Actions.Action(actions.First());
    }
}
