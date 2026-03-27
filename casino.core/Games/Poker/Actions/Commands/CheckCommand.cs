using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Properties.Langages;

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

    public CheckCommand(Player player)
    {
        _player = player;
    }

    public void Execute(Round round)
    {
        var contributionActuelle = round.GetBetFor(_player);

        if (round.CurrentBet - contributionActuelle > 0)
            throw new InvalidOperationException(Resources.ErrorThePlayerCannotCheckBecauseThereIsABetOnTheTable);

        _player.LastAction = PokerTypeAction.Check;
    }
}
