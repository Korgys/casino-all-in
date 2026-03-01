using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using System;

namespace casino.core.Games.Poker.Actions.Commands;

public class RaiseCommand : IPlayerCommand
{
    private readonly Player _player;
    private readonly int _amount;

    public RaiseCommand(Player player, int amount)
    {
        _player = player;
        _amount = amount;
    }

    public void Execute(Round round)
    {
        var currentBet = round.GetBetFor(_player);
        var difference = _amount - currentBet;

        if (_amount <= round.CurrentBet)
        {
            throw new ArgumentException("La relance doit être supérieure ou égale à la mise actuelle.");
        }

        if (difference > _player.Chips)
        {
            throw new ArgumentException("Le Player n'a pas assez de jetons pour relancer autant.");
        }

        if (difference == _player.Chips)
        {
            new AllInCommand(_player).Execute(round);
            return;
        }

        _player.LastAction = PokerTypeAction.Raise;
        _player.Chips -= difference;
        round.CurrentBet = _amount;
        round.SetBetFor(_player, _amount);
        round.Pot += difference;
    }
}
