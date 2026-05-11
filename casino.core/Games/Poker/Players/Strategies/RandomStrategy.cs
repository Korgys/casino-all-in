using casino.core.Games.Poker.Actions;

namespace casino.core.Games.Poker.Players.Strategies;

public class RandomStrategy : IPlayerStrategy
{
    public Actions.GameAction DecideAction(GameContext context)
    {
        var actions = context.AvailableActions;
        var action = actions[Random.Shared.Next(actions.Count)];

        int montant = action switch
        {
            PokerTypeAction.Bet => context.MinimumBet,
            PokerTypeAction.Raise => CalculerRelance(context),
            _ => 0
        };

        return new Actions.GameAction(action, montant);
    }

    private static int CalculerRelance(GameContext context)
    {
        var miseActuelle = context.Round.CurrentBet;
        var contributionActuelle = context.Round.GetBetFor(context.CurrentPlayer);
        var minimum = Math.Max(miseActuelle + 1, context.MinimumBet);
        var cibleMaximaleAutorisee = contributionActuelle + context.CurrentPlayer.Chips;
        var maximum = Math.Max(minimum, Math.Min(cibleMaximaleAutorisee, miseActuelle + context.Round.StartingBet * 3));

        if (minimum >= cibleMaximaleAutorisee)
        {
            return cibleMaximaleAutorisee;
        }

        return Random.Shared.Next(minimum, maximum + 1);
    }
}
