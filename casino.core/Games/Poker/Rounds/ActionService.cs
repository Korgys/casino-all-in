using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Actions.Commands;
using casino.core.Games.Poker.Players;

namespace casino.core.Games.Poker.Rounds;

public interface IActionService
{
    void ExecuteAction(Round round, Player player, Actions.GameAction action);
}

public class ActionService : IActionService
{
    private const string AmountlessActionMustHaveZeroAmount = "Only bet and raise actions can specify an amount.";
    private const string BetAmountMustBeTargetContribution = "Bet amount must be a positive target contribution that increases the player's current contribution.";
    private const string RaiseAmountMustBeTargetContribution = "Raise amount must be a target contribution greater than the current bet and the player's current contribution.";
    private const string ActionAmountExceedsPlayerChips = "Action amount exceeds the player's available chips.";

    public void ExecuteAction(Round round, Player player, Actions.GameAction action)
    {
        ArgumentNullException.ThrowIfNull(round);
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(action);

        if (!round.PhaseState.GetAvailableActions(player, round).Contains(action.TypeAction))
        {
            throw new InvalidOperationException("Player action is not allowed.");
        }

        ValidateAmountContract(round, player, action);

        var command = CreateCommand(player, action);
        command.Execute(round);
    }

    private static void ValidateAmountContract(Round round, Player player, Actions.GameAction action)
    {
        var currentContribution = round.GetBetFor(player);

        switch (action.TypeAction)
        {
            case PokerTypeAction.Bet:
                if (action.Amount <= 0 || action.Amount <= currentContribution)
                    throw new ArgumentException(BetAmountMustBeTargetContribution);
                if (action.Amount - currentContribution > player.Chips)
                    throw new ArgumentException(ActionAmountExceedsPlayerChips);
                break;

            case PokerTypeAction.Raise:
                if (action.Amount <= round.CurrentBet || action.Amount <= currentContribution)
                    throw new ArgumentException(RaiseAmountMustBeTargetContribution);
                if (action.Amount - currentContribution > player.Chips)
                    throw new ArgumentException(ActionAmountExceedsPlayerChips);
                break;

            case PokerTypeAction.Call:
            case PokerTypeAction.Check:
            case PokerTypeAction.Fold:
            case PokerTypeAction.AllIn:
                if (action.Amount != 0)
                    throw new ArgumentException(AmountlessActionMustHaveZeroAmount);
                break;
        }
    }

    private static IPlayerCommand CreateCommand(Player player, Actions.GameAction action) => action.TypeAction switch
    {
        PokerTypeAction.Fold => new FoldCommand(player),
        PokerTypeAction.Bet => new BetCommand(player, action.Amount),
        PokerTypeAction.Call => new CallCommand(player),
        PokerTypeAction.Raise => new RaiseCommand(player, action.Amount),
        PokerTypeAction.AllIn => new AllInCommand(player),
        PokerTypeAction.Check => new CheckCommand(player),
        _ => throw new ArgumentException("Invalid player action.")
    };
}
