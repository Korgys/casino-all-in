using casino.core.Games.Poker.Actions;
using System;
using System.Linq;

namespace casino.core.Games.Poker.Players.Strategies;

public class StrategieRandom : IStrategiePlayer
{
    public Actions.ActionJeu ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var action = actions[Random.Shared.Next(actions.Count)];

        int montant = action switch
        {
            TypeActionJeu.Miser => contexte.MinimumBet,
            TypeActionJeu.Relancer => CalculerRelance(contexte),
            _ => 0
        };

        return new Actions.ActionJeu(action, montant);
    }

    private static int CalculerRelance(ContexteDeJeu contexte)
    {
        var miseActuelle = contexte.Partie.CurrentBet;
        var minimum = Math.Max(miseActuelle + 1, contexte.MinimumBet);
        var maximum = Math.Max(minimum, Math.Min(contexte.PlayerCourant.Chips, miseActuelle + contexte.Partie.StartingBet * 3));

        if (minimum >= contexte.PlayerCourant.Chips)
        {
            return contexte.PlayerCourant.Chips;
        }

        return Random.Shared.Next(minimum, maximum + 1);
    }
}
