using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Actions.Commands;
using casino.core.Games.Poker.Players;

namespace casino.core.Games.Poker.Rounds;

public interface IActionService
{
    void ExecuterAction(Round partie, Player Player, Actions.GameAction action);
}

public class ActionService : IActionService
{
    public void ExecuterAction(Round partie, Player Player, Actions.GameAction action)
    {
        if (!partie.PhaseState.GetAvailableActions(Player, partie).Contains(action.TypeAction))
        {
            throw new InvalidOperationException("Action de Player non autorisée");
        }

        var commande = CreerCommande(Player, action);
        commande.Execute(partie);
    }

    private static IPlayerCommand CreerCommande(Player Player, Actions.GameAction action) => action.TypeAction switch
    {
        PokerTypeAction.Fold => new FoldCommand(Player),
        PokerTypeAction.Bet => new BetCommand(Player, action.Amount),
        PokerTypeAction.Call => new CallCommand(Player),
        PokerTypeAction.Raise => new RaiseCommand(Player, action.Amount),
        PokerTypeAction.AllIn => new AllInCommand(Player),
        PokerTypeAction.Check => new CheckCommand(Player),
        _ => throw new ArgumentException("Action de Player invalide")
    };
}
