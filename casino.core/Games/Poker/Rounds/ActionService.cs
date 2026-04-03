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
    public void ExecuteAction(Round round, Player player, Actions.GameAction action)
    {
        if (!round.PhaseState.GetAvailableActions(player, round).Contains(action.TypeAction))
        {
            throw new InvalidOperationException("Player action is not allowed.");
        }

        var command = CreateCommand(player, action);
        command.Execute(round);
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
