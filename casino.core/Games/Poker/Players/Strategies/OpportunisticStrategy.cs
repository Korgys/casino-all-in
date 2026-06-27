using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Players.Strategies;

public class OpportunisticStrategy : IPlayerStrategy
{
    public GameAction DecideAction(GameContext context)
    {
        var actions = context.AvailableActions;
        if (actions.Count == 1)
            return GetFirstGameAction(context);

        var metrics = ComputeMetrics(context);
        var thresholds = ComputeThresholds(metrics.ActivePlayers, metrics.Phase);

        return ResolveBestLegalAction(context, metrics.NormalizedProbability, thresholds);
    }

    /// <summary>
    /// Calculates the minimum amount a player must raise in the current round, based on the game context.
    /// </summary>
    /// <remarks>The returned value ensures that the raise does not exceed the player's available chips and is
    /// at least the minimum allowed by the game rules.</remarks>
    /// <param name="context">The current game context containing round and player information. Cannot be null.</param>
    /// <returns>The minimum valid raise amount for the current player, considering the current bet, minimum bet, and the
    /// player's available chips.</returns>
    private static int GetMinimumRaiseAmount(GameContext context)
    {
        var currentBet = context.Round.CurrentBet;
        var minimumRaise = Math.Max(currentBet + 1, context.MinimumBet);
        var playerMaximumBet = context.Round.GetBetFor(context.CurrentPlayer) + context.CurrentPlayer.Chips;

        return Math.Min(minimumRaise, playerMaximumBet);
    }

    private static GameAction GetFirstGameAction(GameContext context)
    {
        return context.AvailableActions[0] switch
        {
            PokerTypeAction.Bet => new GameAction(PokerTypeAction.Bet, context.MinimumBet),
            PokerTypeAction.Raise => new GameAction(PokerTypeAction.Raise, GetMinimumRaiseAmount(context)),
            var action => new GameAction(action)
        };
    }

    private static OpportunisticMetrics ComputeMetrics(GameContext context)
    {
        var activePlayers = context.Round.Players.Count(player => !player.IsFolded());
        var opponents = Math.Max(0, activePlayers - 1);
        var phase = context.Round.Phase;

        var rawProbability = ProbabilityEvaluator.EstimateWinProbability(
            context.CurrentPlayer.Hand,
            context.Round.CommunityCards,
            opponents,
            simulations: 1000);

        var normalizedProbability = Math.Clamp(rawProbability / 100.0, 0.0, 1.0);

        return new OpportunisticMetrics(activePlayers, normalizedProbability, phase);
    }

    private static OpportunisticThresholds ComputeThresholds(int activePlayers, Phase phase)
    {
        var activeCount = Math.Max(2, activePlayers);
        var neutralProbability = 1.0 / activeCount;

        var foldMultiplier = phase < Phase.Flop ? 0.75 : 0.70;
        var raiseMultiplier = phase < Phase.Flop ? 1.35 : 1.25;
        var shoveMultiplier = phase < Phase.Flop ? 1.90 : 1.70;

        return new OpportunisticThresholds(
            FoldThreshold: foldMultiplier * neutralProbability,
            RaiseThreshold: raiseMultiplier * neutralProbability,
            AllInThreshold: shoveMultiplier * neutralProbability);
    }

    private static GameAction ResolveBestLegalAction(GameContext context, double probability, OpportunisticThresholds thresholds)
    {
        var actions = context.AvailableActions;
        var availableActions = new HashSet<PokerTypeAction>(actions);

        var orderedRules = new (Func<bool> Predicate, Func<GameAction> ActionFactory)[]
        {
            (() => probability >= thresholds.AllInThreshold && availableActions.Contains(PokerTypeAction.AllIn),
                () => new GameAction(PokerTypeAction.AllIn)),

            (() => probability < thresholds.FoldThreshold && availableActions.Contains(PokerTypeAction.Fold),
                () => new GameAction(PokerTypeAction.Fold)),
            (() => probability < thresholds.FoldThreshold && availableActions.Contains(PokerTypeAction.Call),
                () => new GameAction(PokerTypeAction.Call)),
            (() => probability < thresholds.FoldThreshold,
                () => GetFirstGameAction(context)),

            (() => probability < thresholds.RaiseThreshold && availableActions.Contains(PokerTypeAction.Call),
                () => new GameAction(PokerTypeAction.Call)),
            (() => probability < thresholds.RaiseThreshold && availableActions.Contains(PokerTypeAction.Bet),
                () => new GameAction(PokerTypeAction.Bet, context.MinimumBet)),
            (() => probability < thresholds.RaiseThreshold,
                () => GetFirstGameAction(context)),

            (() => availableActions.Contains(PokerTypeAction.Raise),
                () => new GameAction(PokerTypeAction.Raise, GetMinimumRaiseAmount(context))),
            (() => availableActions.Contains(PokerTypeAction.Bet),
                () => new GameAction(PokerTypeAction.Bet, context.MinimumBet)),
            (() => availableActions.Contains(PokerTypeAction.Call),
                () => new GameAction(PokerTypeAction.Call)),
            (() => true,
                () => GetFirstGameAction(context))
        };

        foreach (var (predicate, actionFactory) in orderedRules)
        {
            if (predicate())
            {
                return actionFactory();
            }
        }

        return GetFirstGameAction(context);
    }

    private readonly record struct OpportunisticMetrics(int ActivePlayers, double NormalizedProbability, Phase Phase);

    private readonly record struct OpportunisticThresholds(double FoldThreshold, double RaiseThreshold, double AllInThreshold);
}
