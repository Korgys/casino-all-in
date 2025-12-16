using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Actions.Commandes;
using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Linq;

namespace casino.core.Jeux.Poker.Parties;

public interface IActionService
{
    void ExecuterAction(Partie partie, Joueur joueur, Actions.Action action);
}

public class ActionService : IActionService
{
    public void ExecuterAction(Partie partie, Joueur joueur, Actions.Action action)
    {
        if (!partie.PhaseState.ObtenirActionsPossibles(joueur, partie).Contains(action.TypeAction))
        {
            throw new InvalidOperationException("Action de joueur non autorisée");
        }

        var commande = CreerCommande(joueur, action);
        commande.Execute(partie);
    }

    private static IJoueurCommande CreerCommande(Joueur joueur, Actions.Action action) => action.TypeAction switch
    {
        TypeAction.SeCoucher => new CoucherCommande(joueur),
        TypeAction.Miser => new MiserCommande(joueur, action.Montant),
        TypeAction.Suivre => new SuivreCommande(joueur),
        TypeAction.Relancer => new RelancerCommande(joueur, action.Montant),
        TypeAction.Tapis => new TapisCommande(joueur),
        TypeAction.Check => new CheckCommande(joueur),
        _ => throw new ArgumentException("Action de joueur invalide")
    };
}
