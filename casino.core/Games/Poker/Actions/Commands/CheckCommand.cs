using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;
using System;

namespace casino.core.Games.Poker.Actions.Commands;

/// <summary>
/// Represents a command that allows a player to check in a poker game, indicating that they do not wish to raise the
/// current bet.
/// </summary>
/// <remarks>The check command can only be executed when there is no active bet on the table. Attempting to check
/// when a bet is present will result in an exception. Upon successful execution, the player's last action is updated to
/// indicate a check.</remarks>
public class CheckCommand : IPlayerCommand
{
    private readonly Player _player;

    public CheckCommand(Player Player)
    {
        _player = Player;
    }

    public void Execute(Partie partie)
    {
        if (partie.CurrentBet != 0)
        {
            throw new InvalidOperationException("Le Player ne peut pas checker car il y a une mise sur la table.");
        }

        _player.LastAction = TypeActionJeu.Check;
    }
}
