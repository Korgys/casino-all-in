using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Players.Strategies;

public class StrategieConservatrice : IStrategiePlayer
{
    public Actions.ActionJeu ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var score = contexte.ScorePlayer;

        if (actions.Contains(TypeActionJeu.Check))
        {
            return new Actions.ActionJeu(TypeActionJeu.Check);
        }

        if (actions.Contains(TypeActionJeu.Suivre) && (score.Rang >= RangMain.DoublePaire || contexte.Partie.CurrentBet <= contexte.MinimumBet))
        {
            return new Actions.ActionJeu(TypeActionJeu.Suivre);
        }

        if (actions.Contains(TypeActionJeu.Miser) && score.Rang >= RangMain.Paire)
        {
            return new Actions.ActionJeu(TypeActionJeu.Miser, contexte.MinimumBet);
        }

        if (actions.Contains(TypeActionJeu.Relancer) && score.Rang >= RangMain.Full)
        {
            return new Actions.ActionJeu(TypeActionJeu.Relancer, contexte.MinimumBet);
        }

        if (actions.Contains(TypeActionJeu.SeCoucher))
        {
            return new Actions.ActionJeu(TypeActionJeu.SeCoucher);
        }

        if (actions.First() == TypeActionJeu.Miser)
        {
            return new Actions.ActionJeu(TypeActionJeu.Miser, contexte.MinimumBet);
        }

        return new Actions.ActionJeu(actions.First());
    }
}
