using casino.core.Games.Poker.Actions;
using System;
using System.Linq;

namespace casino.core.Games.Poker.Players.Strategies;

public class StrategieAgressive : IStrategiePlayer
{
    public Actions.ActionJeu ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var Player = contexte.PlayerCourant;
        var relanceMinimum = Math.Max(contexte.Partie.CurrentBet + contexte.Partie.StartingBet, contexte.MinimumBet);

        if (actions.Contains(TypeActionJeu.Relancer) && Player.Chips > contexte.Partie.CurrentBet)
        {
            int mise = Math.Min(Player.Chips, relanceMinimum);
            return new Actions.ActionJeu(TypeActionJeu.Relancer, mise);
        }

        if (actions.Contains(TypeActionJeu.Miser))
        {
            return new Actions.ActionJeu(TypeActionJeu.Miser, contexte.MinimumBet);
        }

        if (actions.Contains(TypeActionJeu.Suivre))
        {
            return new Actions.ActionJeu(TypeActionJeu.Suivre);
        }

        if (actions.Contains(TypeActionJeu.Tapis))
        {
            return new Actions.ActionJeu(TypeActionJeu.Tapis);
        }

        if (actions.Contains(TypeActionJeu.Check))
        {
            return new Actions.ActionJeu(TypeActionJeu.Check);
        }

        return new Actions.ActionJeu(TypeActionJeu.SeCoucher);
    }
}
