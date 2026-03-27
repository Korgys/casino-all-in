using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;

namespace casino.core.Games.Poker.Actions.Commands;

/// <summary>
/// Represents the action of a player going all-in in a poker game. 
/// When executed, it updates the player's chips, the current bet, and the pot accordingly.
/// </summary>
public class AllInCommand : IPlayerCommand
{
    private readonly Player _player;

    public AllInCommand(Player player)
    {
        _player = player;
    }

    public void Execute(Round round)
    {
        var miseAvant = round.GetBetFor(_player);
        var contribution = miseAvant + _player.Chips;
        _player.LastAction = PokerTypeAction.AllIn;
        round.AddToPot(_player.Chips);
        round.SetBetFor(_player, contribution);
        round.RaiseCurrentBetToAtLeast(contribution);
        _player.Chips = 0;
    }
}
