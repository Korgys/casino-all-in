using casino.core.Games.Poker.Actions;
using System;
using System.Linq;

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
        var minimum = Math.Max(miseActuelle + 1, context.MinimumBet);
        var maximum = Math.Max(minimum, Math.Min(context.CurrentPlayer.Chips, miseActuelle + context.Round.StartingBet * 3));

        if (minimum >= context.CurrentPlayer.Chips)
        {
            return context.CurrentPlayer.Chips;
        }

        return Random.Shared.Next(minimum, maximum + 1);
    }
}
