using casino.core.Games.Poker.Actions;

namespace casino.core.Games.Poker.Players.Strategies;

public class RandomStrategy : IPlayerStrategy
{
    public Actions.GameAction DecideAction(GameContext context)
    {
        var actions = context.AvailableActions;
        var action = actions[Random.Shared.Next(actions.Count)];

        int amount = action switch
        {
            PokerTypeAction.Bet => context.MinimumBet,
            PokerTypeAction.Raise => CalculateRaise(context),
            _ => 0
        };

        return new Actions.GameAction(action, amount);
    }

    private static int CalculateRaise(GameContext context)
    {
        var currentBet = context.Round.CurrentBet;
        var currentContribution = context.Round.GetBetFor(context.CurrentPlayer);
        var minimum = Math.Max(currentBet + 1, context.MinimumBet);
        var maximumAllowedTarget = currentContribution + context.CurrentPlayer.Chips;
        var maximum = Math.Max(minimum, Math.Min(maximumAllowedTarget, currentBet + context.Round.StartingBet * 3));

        if (minimum >= maximumAllowedTarget)
        {
            return maximumAllowedTarget;
        }

        return Random.Shared.Next(minimum, maximum + 1);
    }
}
