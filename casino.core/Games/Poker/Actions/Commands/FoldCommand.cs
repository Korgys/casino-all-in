using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using System.Linq;

namespace casino.core.Games.Poker.Actions.Commands;

/// <summary>
/// Represents a command that allows a player to fold in a poker game, indicating that they are withdrawing from the current hand.
/// </summary>
public class FoldCommand : IPlayerCommand
{
    private readonly Player _player;

    public FoldCommand(Player player)
    {
        _player = player;
    }

    public void Execute(Round round)
    {
        _player.LastAction = PokerTypeAction.Fold;

        // Mark the player as folded
        if (round.Players.Count(j => !j.IsFolded()) == 1)
            round.EndGame();
    }
}
