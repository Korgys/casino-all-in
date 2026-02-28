using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;
using System;

namespace casino.core.Games.Poker.Actions.Commands;

public class RaiseCommand : IPlayerCommand
{
    private readonly Player _player;
    private readonly int _montant;

    public RaiseCommand(Player Player, int montant)
    {
        _player = Player;
        _montant = montant;
    }

    public void Execute(Partie partie)
    {
        var contributionActuelle = partie.GetBetFor(_player);
        var difference = _montant - contributionActuelle;

        if (_montant <= partie.CurrentBet)
        {
            throw new ArgumentException("La relance doit être supérieure ou égale à la mise actuelle.");
        }

        if (difference > _player.Chips)
        {
            throw new ArgumentException("Le Player n'a pas assez de jetons pour relancer autant.");
        }

        if (difference == _player.Chips)
        {
            new AllInCommand(_player).Execute(partie);
            return;
        }

        _player.LastAction = TypeActionJeu.Relancer;
        _player.Chips -= difference;
        partie.CurrentBet = _montant;
        partie.SetBetFor(_player, _montant);
        partie.Pot += difference;
    }
}
