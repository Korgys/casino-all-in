using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Players.Strategies;

public class ConservativeStrategy : IPlayerStrategy
{
    public Actions.GameAction ProposerAction(GameContext contexte)
    {
        var actions = contexte.AvailableActions;
        var score = contexte.PlayerScore;

        if (actions.Contains(TypeGameAction.Check))
        {
            return new Actions.GameAction(TypeGameAction.Check);
        }

        if (actions.Contains(TypeGameAction.Suivre) && (score.Rang >= HandRank.DoublePaire || contexte.Round.CurrentBet <= contexte.MinimumBet))
        {
            return new Actions.GameAction(TypeGameAction.Suivre);
        }

        if (actions.Contains(TypeGameAction.Miser) && score.Rang >= HandRank.Paire)
        {
            return new Actions.GameAction(TypeGameAction.Miser, contexte.MinimumBet);
        }

        if (actions.Contains(TypeGameAction.Relancer) && score.Rang >= HandRank.Full)
        {
            return new Actions.GameAction(TypeGameAction.Relancer, contexte.MinimumBet);
        }

        if (actions.Contains(TypeGameAction.SeCoucher))
        {
            return new Actions.GameAction(TypeGameAction.SeCoucher);
        }

        if (actions.First() == TypeGameAction.Miser)
        {
            return new Actions.GameAction(TypeGameAction.Miser, contexte.MinimumBet);
        }

        return new Actions.GameAction(actions.First());
    }
}
