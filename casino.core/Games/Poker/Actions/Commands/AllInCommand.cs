using System;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;

namespace casino.core.Games.Poker.Actions.Commands;

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
        round.Pot += _player.Chips;
        round.SetBetFor(_player, contribution);
        round.CurrentBet = Math.Max(round.CurrentBet, contribution);
        _player.Chips = 0;
    }
}
