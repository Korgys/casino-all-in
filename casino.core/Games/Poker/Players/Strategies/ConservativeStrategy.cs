using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Players.Strategies;

public class ConservativeStrategy : IPlayerStrategy
{
    public Actions.GameAction DecideAction(GameContext context)
    {
        var actions = context.AvailableActions;
        var score = context.PlayerScore;

        if (actions.Contains(PokerTypeAction.Check))
        {
            return new Actions.GameAction(PokerTypeAction.Check);
        }

        if (actions.Contains(PokerTypeAction.Call) && (score.Rank >= HandRank.TwoPair || context.Round.CurrentBet <= context.MinimumBet))
        {
            return new Actions.GameAction(PokerTypeAction.Call);
        }

        if (actions.Contains(PokerTypeAction.Bet) && score.Rank >= HandRank.OnePair)
        {
            return new Actions.GameAction(PokerTypeAction.Bet, context.MinimumBet);
        }

        if (actions.Contains(PokerTypeAction.Raise) && score.Rank >= HandRank.FullHouse)
        {
            return new Actions.GameAction(PokerTypeAction.Raise, context.MinimumBet);
        }

        if (actions.Contains(PokerTypeAction.Fold))
        {
            return new Actions.GameAction(PokerTypeAction.Fold);
        }

        if (actions[0] == PokerTypeAction.Bet)
        {
            return new Actions.GameAction(PokerTypeAction.Bet, context.MinimumBet);
        }

        return new Actions.GameAction(actions[0]);
    }
}
