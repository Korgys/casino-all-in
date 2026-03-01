using casino.core.Games.Poker.Actions;
using System;
using System.Linq;

namespace casino.core.Games.Poker.Players.Strategies;

public class RandomStrategy : IPlayerStrategy
{
    public Actions.GameAction DecideAction(GameContext contexte)
    {
        var actions = contexte.AvailableActions;
        var action = actions[Random.Shared.Next(actions.Count)];

        int montant = action switch
        {
            PokerTypeAction.Bet => contexte.MinimumBet,
            PokerTypeAction.Raise => CalculerRelance(contexte),
            _ => 0
        };

        return new Actions.GameAction(action, montant);
    }

    private static int CalculerRelance(GameContext contexte)
    {
        var miseActuelle = contexte.Round.CurrentBet;
        var minimum = Math.Max(miseActuelle + 1, contexte.MinimumBet);
        var maximum = Math.Max(minimum, Math.Min(contexte.CurrentPlayer.Chips, miseActuelle + contexte.Round.StartingBet * 3));

        if (minimum >= contexte.CurrentPlayer.Chips)
        {
            return contexte.CurrentPlayer.Chips;
        }

        return Random.Shared.Next(minimum, maximum + 1);
    }
}
