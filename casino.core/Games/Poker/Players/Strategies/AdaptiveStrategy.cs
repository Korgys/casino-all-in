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

        if (ShouldPreferCheck(actions))
            return new GameAction(PokerTypeAction.Check);

        var metrics = ComputeMetrics(context);
        var thresholds = ComputeThresholds(metrics.ActivePlayers, metrics.Phase);

        if (TryCreateBluffAction(context, actions, out var bluffAction))
            return bluffAction;

        return ResolveThresholdAction(context, actions, metrics.NormalizedProbability, thresholds);
    }

    private bool ShouldPreferCheck(IReadOnlySet<PokerTypeAction> actions)
    {
        return _profile.PreferCheckWhenAvailable && actions.Contains(PokerTypeAction.Check);
    }

    private bool TryCreateBluffAction(
        GameContext context,
        IReadOnlySet<PokerTypeAction> actions,
        out GameAction action)
    {
        action = default!;

        if (_profile.BluffFrequency <= 0 || Random.Shared.NextDouble() >= _profile.BluffFrequency)
            return false;

        if (actions.Contains(PokerTypeAction.Raise))
        {
            action = new GameAction(PokerTypeAction.Raise, Math.Max(context.Round.CurrentBet + 1, context.MinimumBet));
            return true;
        }

        if (actions.Contains(PokerTypeAction.Bet))
        {
            action = new GameAction(PokerTypeAction.Bet, context.MinimumBet);
            return true;
        }

        return false;
    }

    private GameAction ResolveThresholdAction(
        GameContext context,
        HashSet<PokerTypeAction> actions,
        double normalizedProbability,
        StrategyThresholds thresholds)
    {
        if (ShouldGoAllIn(normalizedProbability, thresholds.AllInThreshold, actions))
            return new GameAction(PokerTypeAction.AllIn);

        if (normalizedProbability < thresholds.FoldThreshold)
            return ChooseDefensiveAction(actions);

        if (normalizedProbability < thresholds.RaiseThreshold)
            return ChooseCautiousAction(context, actions);

        return ChooseAggressiveAction(context, actions);
    }

    private bool ShouldGoAllIn(
        double normalizedProbability,
        double allInThreshold,
        IReadOnlySet<PokerTypeAction> actions)
    {
        return _profile.CanAllInWhenStrong
            && normalizedProbability >= allInThreshold
            && actions.Contains(PokerTypeAction.AllIn);
    }

    private GameAction ChooseAggressiveAction(GameContext context, IReadOnlySet<PokerTypeAction> actions)
    {
        if (actions.Contains(PokerTypeAction.Raise))
            return new GameAction(PokerTypeAction.Raise, ComputeRaiseAmount(context));

        return ChooseNonRaisingAggressiveAction(context, actions);
    }

    private static GameAction ChooseNonRaisingAggressiveAction(GameContext context, IReadOnlySet<PokerTypeAction> actions)
    {
        if (actions.Contains(PokerTypeAction.Bet))
            return new GameAction(PokerTypeAction.Bet, context.MinimumBet);

        if (actions.Contains(PokerTypeAction.Call))
            return new GameAction(PokerTypeAction.Call);

        if (actions.Contains(PokerTypeAction.Check))
            return new GameAction(PokerTypeAction.Check);

        return new GameAction(context.AvailableActions[0]);
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

    private StrategyThresholds ComputeThresholds(int activePlayers, Phase phase)
    {
        var players = Math.Max(2, activePlayers);
        var neutral = 1.0 / players;
        var phaseAdjustment = phase < Phase.Flop ? 1.05 : 1.00;

        return new StrategyThresholds(
            FoldThreshold: _profile.FoldThresholdFactor * neutral * phaseAdjustment,
            RaiseThreshold: _profile.RaiseThresholdFactor * neutral,
            AllInThreshold: _profile.AllInThresholdFactor * neutral);
    }

    private readonly record struct StrategyMetrics(int ActivePlayers, double NormalizedProbability, Phase Phase);

    private readonly record struct StrategyThresholds(double FoldThreshold, double RaiseThreshold, double AllInThreshold);
}
