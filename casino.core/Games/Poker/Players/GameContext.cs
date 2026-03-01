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
    public IReadOnlyList<TypeGameAction> AvailableActions { get; }
    public Score PlayerScore { get; }
    public int MinimumBet => Math.Max(Round.CurrentBet, Round.StartingBet);

    public GameContext(Round partie, Player CurrentPlayer, IReadOnlyList<TypeGameAction> actionsPossibles)
    {
        Round = partie;
        CurrentPlayer = CurrentPlayer;
        AvailableActions = actionsPossibles;
        PlayerScore = ScoreEvaluator.EvaluerScore(CurrentPlayer.Hand, partie.CommunityCards);
    }
}
