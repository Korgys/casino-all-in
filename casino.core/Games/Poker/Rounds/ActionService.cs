using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Actions.Commands;
using casino.core.Games.Poker.Players;
using System;
using System.Linq;

namespace casino.core.Games.Poker.Parties;

public interface IActionService
{
    void ExecuterAction(Partie partie, Player Player, Actions.ActionJeu action);
}

public class ActionService : IActionService
{
    public void ExecuterAction(Partie partie, Player Player, Actions.ActionJeu action)
    {
        if (!partie.PhaseState.ObtenirActionsPossibles(Player, partie).Contains(action.TypeAction))
        {
            throw new InvalidOperationException("Action de Player non autorisée");
        }

        var commande = CreerCommande(Player, action);
        commande.Execute(partie);
    }

    private static IPlayerCommand CreerCommande(Player Player, Actions.ActionJeu action) => action.TypeAction switch
    {
        TypeActionJeu.SeCoucher => new FoldCommand(Player),
        TypeActionJeu.Miser => new BetCommand(Player, action.Montant),
        TypeActionJeu.Suivre => new CallCommand(Player),
        TypeActionJeu.Relancer => new RaiseCommand(Player, action.Montant),
        TypeActionJeu.Tapis => new AllInCommand(Player),
        TypeActionJeu.Check => new CheckCommand(Player),
        _ => throw new ArgumentException("Action de Player invalide")
    };
}
