using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Properties.Langages;
using System;

namespace casino.core.Games.Poker.Actions.Commands;

/// <summary>
/// Represents a command to place a bet in a poker game. This command allows a player to increase their contribution to the pot by placing a bet. 
/// The command checks for valid bet amounts and updates the player's chips, the current bet, and the pot accordingly.
/// </summary>
public class BetCommand : IPlayerCommand
{
    private readonly Player _player;
    private readonly int _amount;

    public BetCommand(Player player, int amount)
    {
        _player = player;
        _amount = amount;
    }

    public void Execute(Round round)
    {
        if (_amount <= 0)
            throw new ArgumentException(Resources.ErrorTheBetAmountShouldBeGreaterThanZero);
        if (round.CurrentBet > _amount)
            throw new InvalidOperationException(Resources.ErrorTheBetAmountCannotBeLowerThanTheInitialBetAmount);

        var currentBet = round.GetBetFor(_player);
        var diff = _amount - currentBet;

        if (diff <= 0)
            throw new InvalidOperationException(Resources.ErrorTheBetShouldIncreaseThePlayerContribution);
        if (diff > _player.Chips)
            throw new InvalidOperationException(Resources.ErrorPlayerHasNotEnoughChips);

        _player.LastAction = PokerTypeAction.Bet;
        // Update the player contribution
        _player.Chips -= diff;
        round.SetBetFor(_player, _amount);
        round.SetCurrentBet(_amount);
        round.AddToPot(diff);
    }
}
