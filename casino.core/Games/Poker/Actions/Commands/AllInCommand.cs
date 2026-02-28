using System;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;

namespace casino.core.Games.Poker.Actions.Commands;

public class AllInCommand : IPlayerCommand
{
    private readonly Player _player;

    public AllInCommand(Player Player)
    {
        _player = Player;
    }

    public void Execute(Partie partie)
    {
        var miseAvant = partie.GetBetFor(_player);
        var contribution = miseAvant + _player.Chips;
        _player.LastAction = TypeActionJeu.Tapis;
        partie.Pot += _player.Chips;
        partie.SetBetFor(_player, contribution);
        partie.CurrentBet = Math.Max(partie.CurrentBet, contribution);
        _player.Chips = 0;
    }
}
