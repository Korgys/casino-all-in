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
    /// <param name="round">The current round.</param>
    /// <param name="currentPlayer">The player making a decision.</param>
    /// <param name="availableActions">The list of available actions.</param>
    public GameContext(Round round, Player currentPlayer, IReadOnlyList<PokerTypeAction> availableActions)
    {
        Round = round;
        CurrentPlayer = currentPlayer;
        AvailableActions = availableActions;
        PlayerScore = ScoreEvaluator.EvaluateScore(currentPlayer.Hand, round.CommunityCards);
    }
}
