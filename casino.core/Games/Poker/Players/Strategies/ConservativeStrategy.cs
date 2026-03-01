using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Players.Strategies;

public class ConservativeStrategy : IPlayerStrategy
{
    public Actions.GameAction DecideAction(GameContext contexte)
    {
        var actions = contexte.AvailableActions;
        var score = contexte.PlayerScore;

        if (actions.Contains(PokerTypeAction.Check))
        {
            return new Actions.GameAction(PokerTypeAction.Check);
        }

        if (actions.Contains(PokerTypeAction.Call) && (score.Rank >= HandRank.TwoPair || contexte.Round.CurrentBet <= contexte.MinimumBet))
        {
            return new Actions.GameAction(PokerTypeAction.Call);
        }

        if (actions.Contains(PokerTypeAction.Bet) && score.Rank >= HandRank.OnePair)
        {
            return new Actions.GameAction(PokerTypeAction.Bet, contexte.MinimumBet);
        }

        if (actions.Contains(PokerTypeAction.Raise) && score.Rank >= HandRank.FullHouse)
        {
            return new Actions.GameAction(PokerTypeAction.Raise, contexte.MinimumBet);
        }

        if (actions.Contains(PokerTypeAction.Fold))
        {
            return new Actions.GameAction(PokerTypeAction.Fold);
        }

        if (actions.First() == PokerTypeAction.Bet)
        {
            return new Actions.GameAction(PokerTypeAction.Bet, contexte.MinimumBet);
        }

        return new Actions.GameAction(actions.First());
    }
}
