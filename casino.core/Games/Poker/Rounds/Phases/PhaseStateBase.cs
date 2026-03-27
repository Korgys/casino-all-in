using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;

namespace casino.core.Games.Poker.Rounds.Phases;

public abstract class PhaseStateBase : IPhaseState
{
    // Captures per-turn values used by multiple rule blocks.
    private readonly record struct ActionDerivationState(int PlayerBet, int Difference, bool CanCoverCall);

    public abstract void Avancer(Round context);

    public virtual IEnumerable<PokerTypeAction> GetAvailableActions(Player Player, Round context)
    {
        // Folded/all-in players cannot perform any action.
        if (!IsPlayerActionable(Player))
        {
            return Enumerable.Empty<PokerTypeAction>();
        }

        var playerBet = context.GetBetFor(Player);
        var state = new ActionDerivationState(
            PlayerBet: playerBet,
            Difference: context.CurrentBet - playerBet,
            CanCoverCall: context.CurrentBet - playerBet < Player.Chips);

        // Pre-flop opening round (no posted bet yet).
        if (context.Phase == Phase.PreFlop && context.CurrentBet == 0 && state.PlayerBet == 0)
        {
            return BuildPreFlopOpeningActions(Player, context).OrderBy(a => (int)a).ToList();
        }

        // Player is facing a bet and must respond.
        if (state.Difference > 0)
        {
            return BuildFacingBetActions(state).OrderBy(a => (int)a).ToList();
        }

        // No bet to call; player can check and may be able to bet/raise.
        return BuildNoBetToCallActions(Player, context, state).OrderBy(a => (int)a).ToList();
    }

    private static bool IsPlayerActionable(Player player)
    {
        return !player.IsFolded() && !player.IsAllIn();
    }

    private static IEnumerable<PokerTypeAction> BuildPreFlopOpeningActions(Player player, Round context)
    {
        if (player.Chips <= 0)
        {
            return [PokerTypeAction.Fold];
        }

        if (player.Chips <= context.StartingBet)
        {
            return [PokerTypeAction.AllIn];
        }

        return [PokerTypeAction.Bet, PokerTypeAction.Raise, PokerTypeAction.AllIn];
    }

    private static IEnumerable<PokerTypeAction> BuildFacingBetActions(ActionDerivationState state)
    {
        if (state.CanCoverCall)
        {
            return [
                PokerTypeAction.Fold,
                PokerTypeAction.Call,
                PokerTypeAction.Raise,
                PokerTypeAction.AllIn
            ];
        }

        return [PokerTypeAction.Fold, PokerTypeAction.AllIn];
    }

    private static IEnumerable<PokerTypeAction> BuildNoBetToCallActions(Player player, Round context, ActionDerivationState state)
    {
        var actions = new List<PokerTypeAction>
        {
            PokerTypeAction.Fold,
            PokerTypeAction.Check
        };

        if (context.CurrentBet == 0)
        {
            actions.Add(PokerTypeAction.Bet);
        }

        if (player.Chips > 0 && (context.CurrentBet == 0 || player.Chips + state.PlayerBet > context.CurrentBet))
        {
            actions.Add(PokerTypeAction.Raise);
        }

        return actions;
    }

    public virtual void ApplyAction(Player Player, Actions.GameAction action, Round context)
    {
        context.ActionService.ExecuterAction(context, Player, action);
    }
}
