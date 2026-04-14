using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Players.Strategies;

public class AdaptiveStrategy(PokerAiProfile profile) : IPlayerStrategy
{
    private readonly PokerAiProfile _profile = profile;

    public GameAction DecideAction(GameContext context)
    {
        var actions = new HashSet<PokerTypeAction>(context.AvailableActions);

        if (_profile.PreferCheckWhenAvailable && actions.Contains(PokerTypeAction.Check))
            return new GameAction(PokerTypeAction.Check);

        var metrics = ComputeMetrics(context);
        var thresholds = ComputeThresholds(metrics.ActivePlayers, metrics.Phase);

        if (ShouldBluff(actions))
            return CreateBluffAction(context, actions);

        return DecideActionByThresholds(context, actions, metrics, thresholds);
    }

    private GameAction DecideActionByThresholds(
        GameContext context,
        HashSet<PokerTypeAction> actions,
        StrategyMetrics metrics,
        (double FoldThreshold, double CallThreshold, double RaiseThreshold, double AllInThreshold) thresholds)
    {
        if (_profile.CanAllInWhenStrong
            && metrics.NormalizedProbability >= thresholds.AllInThreshold
            && actions.Contains(PokerTypeAction.AllIn))
        {
            return new GameAction(PokerTypeAction.AllIn);
        }

        if (metrics.NormalizedProbability < thresholds.FoldThreshold)
            return ChooseDefensiveAction(actions);

        if (metrics.NormalizedProbability < thresholds.RaiseThreshold)
            return ChooseCautiousAction(context, actions);

        return ChooseAggressiveAction(context, actions);
    }

    private static GameAction ChooseDefensiveAction(IReadOnlySet<PokerTypeAction> actions)
    {
        if (actions.Contains(PokerTypeAction.Fold))
            return new GameAction(PokerTypeAction.Fold);

        if (actions.Contains(PokerTypeAction.Check))
            return new GameAction(PokerTypeAction.Check);

        if (actions.Contains(PokerTypeAction.Call))
            return new GameAction(PokerTypeAction.Call);

        return new GameAction(actions.First());
    }

    private GameAction ChooseCautiousAction(GameContext context, IReadOnlySet<PokerTypeAction> actions)
    {
        if (actions.Contains(PokerTypeAction.Check))
            return new GameAction(PokerTypeAction.Check);

        if (actions.Contains(PokerTypeAction.Call))
            return new GameAction(PokerTypeAction.Call);

        if (actions.Contains(PokerTypeAction.Bet))
            return new GameAction(PokerTypeAction.Bet, context.MinimumBet);

        return ChooseAggressiveAction(context, actions);
    }

    private GameAction ChooseAggressiveAction(GameContext context, IReadOnlySet<PokerTypeAction> actions)
    {
        if (actions.Contains(PokerTypeAction.Raise))
            return new GameAction(PokerTypeAction.Raise, ComputeRaiseAmount(context));

        if (actions.Contains(PokerTypeAction.Bet))
            return new GameAction(PokerTypeAction.Bet, context.MinimumBet);

        if (actions.Contains(PokerTypeAction.Call))
            return new GameAction(PokerTypeAction.Call);

        if (actions.Contains(PokerTypeAction.Check))
            return new GameAction(PokerTypeAction.Check);

        return new GameAction(context.AvailableActions[0]);
    }

    private bool ShouldBluff(IReadOnlySet<PokerTypeAction> actions)
    {
        if (_profile.BluffFrequency <= 0)
            return false;

        if (!actions.Contains(PokerTypeAction.Raise) && !actions.Contains(PokerTypeAction.Bet))
            return false;

        return Random.Shared.NextDouble() < _profile.BluffFrequency;
    }

    private static GameAction CreateBluffAction(GameContext context, IReadOnlySet<PokerTypeAction> actions)
    {
        if (actions.Contains(PokerTypeAction.Raise))
            return new GameAction(PokerTypeAction.Raise, Math.Max(context.Round.CurrentBet + 1, context.MinimumBet));

        if (actions.Contains(PokerTypeAction.Bet))
            return new GameAction(PokerTypeAction.Bet, context.MinimumBet);

        return new GameAction(PokerTypeAction.Call);
    }

    private int ComputeRaiseAmount(GameContext context)
    {
        var currentBet = context.Round.CurrentBet;
        var minimumRaise = Math.Max(currentBet + 1, context.MinimumBet);
        var maxAllowedTarget = context.Round.GetBetFor(context.CurrentPlayer) + context.CurrentPlayer.Chips;

        var desiredRaise = (int)Math.Ceiling(context.MinimumBet * _profile.RaiseSizeMultiplier);
        var target = Math.Max(minimumRaise, desiredRaise);

        return Math.Min(target, maxAllowedTarget);
    }

    private static StrategyMetrics ComputeMetrics(GameContext context)
    {
        var activePlayers = context.Round.Players.Count(player => !player.IsFolded());
        var opponents = Math.Max(0, activePlayers - 1);
        var phase = context.Round.Phase;

        var winProbability = ProbabilityEvaluator.EstimateWinProbability(
            context.CurrentPlayer.Hand,
            context.Round.CommunityCards,
            opponents,
            simulations: 1000);

        var normalizedProbability = Math.Clamp(winProbability / 100.0, 0.0, 1.0);

        return new StrategyMetrics(activePlayers, normalizedProbability, phase);
    }

    private (double FoldThreshold, double CallThreshold, double RaiseThreshold, double AllInThreshold) ComputeThresholds(int activePlayers, Phase phase)
    {
        var players = Math.Max(2, activePlayers);
        var neutral = 1.0 / players;
        var phaseAdjustment = phase < Phase.Flop ? 1.05 : 1.00;

        return (
            FoldThreshold: _profile.FoldThresholdFactor * neutral * phaseAdjustment,
            CallThreshold: _profile.CallThresholdFactor * neutral * phaseAdjustment,
            RaiseThreshold: _profile.RaiseThresholdFactor * neutral,
            AllInThreshold: _profile.AllInThresholdFactor * neutral);
    }

    private readonly record struct StrategyMetrics(int ActivePlayers, double NormalizedProbability, Phase Phase);
}



