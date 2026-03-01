using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Actions.Commands;
using casino.core.Games.Poker.Players;
using System;
using System.Linq;

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
        TypeGameAction.SeCoucher => new FoldCommand(Player),
        TypeGameAction.Miser => new BetCommand(Player, action.Montant),
        TypeGameAction.Suivre => new CallCommand(Player),
        TypeGameAction.Relancer => new RaiseCommand(Player, action.Montant),
        TypeGameAction.Tapis => new AllInCommand(Player),
        TypeGameAction.Check => new CheckCommand(Player),
        _ => throw new ArgumentException("Action de Player invalide")
    };
}
