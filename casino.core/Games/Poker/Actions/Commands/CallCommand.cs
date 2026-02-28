using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;
using System;

namespace casino.core.Games.Poker.Actions.Commands;

public class CallCommand : IPlayerCommand
{
    private readonly Player _player;

    public CallCommand(Player Player)
    {
        _player = Player;
    }

    public void Execute(Partie partie)
    {
        var contributionActuelle = partie.GetBetFor(_player);
        var difference = partie.CurrentBet - contributionActuelle;

        if (difference <= 0)
        {
            throw new InvalidOperationException("Aucune mise supplémentaire à suivre.");
        }

        if (_player.Chips - difference < 0)
        {
            throw new InvalidOperationException("Le joueur n'a pas assez de jetons pour suivre la mise actuelle.");
        }

        if (_player.Chips - difference == 0)
        {
            new AllInCommand(_player).Execute(partie);
            return;
        }

        _player.LastAction = TypeActionJeu.Suivre;
        _player.Chips -= difference;
        partie.SetBetFor(_player, partie.CurrentBet);
        partie.Pot += difference;
    }
}
