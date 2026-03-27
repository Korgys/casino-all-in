using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Players;

/// <summary>
/// Provides context data used to decide a poker action.
/// </summary>
public class GameContext
{
    public Round Round { get; }
    public Player CurrentPlayer { get; }
    public IReadOnlyList<PokerTypeAction> AvailableActions { get; }
    public Score PlayerScore { get; }
    public int MinimumBet => Math.Max(Round.CurrentBet, Round.StartingBet);

    /// <summary>
    /// Initializes a new game context.
    /// </summary>
    /// <param name="partie">The current round.</param>
    /// <param name="currentPlayer">The player making a decision.</param>
    /// <param name="actionsPossibles">The list of available actions.</param>
    public GameContext(Round partie, Player currentPlayer, IReadOnlyList<PokerTypeAction> actionsPossibles)
    {
        Round = partie;
        CurrentPlayer = currentPlayer;
        AvailableActions = actionsPossibles;
        PlayerScore = ScoreEvaluator.EvaluateScore(currentPlayer.Hand, partie.CommunityCards);
    }
}
