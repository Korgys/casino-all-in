using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;

namespace casino.core.Games.Poker.Scores;

public static class WinnerEvaluator
{
    /// <summary>
    /// Evaluates the winner of the table.
    /// </summary>
    /// <param name="players">The players to compare.</param>
    /// <param name="tableCards">The community cards available at showdown.</param>
    /// <returns>The player or players tied with the best hand.</returns>
    /// <exception cref="ArgumentException">Thrown when no active player remains.</exception>
    public static IReadOnlyList<Player> DetermineWinnersByHand(IEnumerable<Player> players, TableCards tableCards)
    {
        var inGamePlayers = players
            .Where(player => player.LastAction != PokerTypeAction.Fold)
            .Select(player => new
            {
                Player = player,
                Score = ScoreEvaluator.EvaluateScore(player.Hand, tableCards)
            });

        if (!inGamePlayers.Any())
        {
            throw new ArgumentException("At least one player must still be in game to determine winners by hand.");
        }

        var bestScore = inGamePlayers.Max(x => x.Score);

        return inGamePlayers
            .Where(playerScore => playerScore.Score.CompareTo(bestScore) == 0)
            .Select(x => x.Player)
            .ToList();
    }
}
