using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Properties.Langages;
using System;

namespace casino.core.Games.Poker.Actions.Commands;

/// <summary>
/// Represents a command that allows a player to raise in a poker game, increasing the current bet by a specified amount.
/// </summary>
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
            throw new ArgumentException(Resources.ErrorTheRaiseMustBeGreaterOrEqualThanTheCurrentBet);

        if (difference > _player.Chips)
            throw new ArgumentException(Resources.ErrorThePlayerDoesNotHaveEnoughChipsToRaiseThatAmount);

        if (difference == _player.Chips)
        {
            new AllInCommand(_player).Execute(round);
            return;
        }

        // Update the player's last action, chips, current bet, and the round's pot
        _player.LastAction = PokerTypeAction.Raise;
        _player.Chips -= difference;
        round.SetCurrentBet(_amount);
        round.SetBetFor(_player, _amount);
        round.AddToPot(difference);
    }
}
