using System;
using System.Collections.Generic;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Players;

public class GameContext
{
    public Round Round { get; }
    public Player CurrentPlayer { get; }
    public IReadOnlyList<PokerTypeAction> AvailableActions { get; }
    public Score PlayerScore { get; }
    public int MinimumBet => Math.Max(Round.CurrentBet, Round.StartingBet);

    public GameContext(Round partie, Player currentPlayer, IReadOnlyList<PokerTypeAction> actionsPossibles)
    {
        Round = partie;
        CurrentPlayer = currentPlayer;
        AvailableActions = actionsPossibles;
        PlayerScore = ScoreEvaluator.EvaluateScore(currentPlayer.Hand, partie.CommunityCards);
    }
}
