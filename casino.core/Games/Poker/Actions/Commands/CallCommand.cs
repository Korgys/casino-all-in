using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using System;

namespace casino.core.Games.Poker.Actions.Commands;

public class CallCommand : IPlayerCommand
{
    private readonly Player _player;

    public CallCommand(Player player)
    {
        _player = player;
    }

    public void Execute(Round round)
    {
        var contributionActuelle = round.GetBetFor(_player);
        var diff = round.CurrentBet - contributionActuelle;

        if (diff <= 0)
        {
            throw new InvalidOperationException("Aucune mise supplémentaire à suivre.");
        }

        if (_player.Chips - diff < 0)
        {
            throw new InvalidOperationException("Le joueur n'a pas assez de jetons pour suivre la mise actuelle.");
        }

        if (_player.Chips - diff == 0)
        {
            new AllInCommand(_player).Execute(round);
            return;
        }

        _player.LastAction = PokerTypeAction.Call;
        _player.Chips -= diff;
        round.SetBetFor(_player, round.CurrentBet);
        round.Pot += diff;
    }
}
