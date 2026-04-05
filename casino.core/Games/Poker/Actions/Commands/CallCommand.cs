using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Properties.Languages;

namespace casino.core.Games.Poker.Actions.Commands;

/// <summary>
/// Represents the action of calling the current bet in a poker game.
/// </summary>
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
            throw new InvalidOperationException(Resources.ErrorNoAdditionalBetToCall);

        if (_player.Chips - diff < 0)
            throw new InvalidOperationException(Resources.ErrorThePlayerDoesNotHaveEnoughChipsToCallTheCurrentBet);

        if (_player.Chips - diff == 0)
        {
            new AllInCommand(_player).Execute(round);
            return;
        }

        // The player has enough chips to call the current bet
        // Update the player's last action, chips, and the round's bet and pot
        _player.LastAction = PokerTypeAction.Call;
        _player.Chips -= diff;
        round.SetBetFor(_player, round.CurrentBet);
        round.AddToPot(_player, diff);
    }
}
