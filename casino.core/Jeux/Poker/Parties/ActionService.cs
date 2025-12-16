using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Actions.Commandes;
using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Linq;

namespace casino.core.Jeux.Poker.Parties;

public interface IActionService
{
    void ExecuterAction(Partie partie, Joueur joueur, Actions.ActionJeu action);
}

public class ActionService : IActionService
{
    public void ExecuterAction(Partie partie, Joueur joueur, Actions.ActionJeu action)
    {
        if (!partie.PhaseState.ObtenirActionsPossibles(joueur, partie).Contains(action.TypeAction))
        {
            throw new InvalidOperationException("Action de joueur non autorisée");
        }

        var commande = CreerCommande(joueur, action);
        commande.Execute(partie);
    }

    private static IJoueurCommande CreerCommande(Joueur joueur, Actions.ActionJeu action) => action.TypeAction switch
    {
        TypeActionJeu.SeCoucher => new CoucherCommande(joueur),
        TypeActionJeu.Miser => new MiserCommande(joueur, action.Montant),
        TypeActionJeu.Suivre => new SuivreCommande(joueur),
        TypeActionJeu.Relancer => new RelancerCommande(joueur, action.Montant),
        TypeActionJeu.Tapis => new TapisCommande(joueur),
        TypeActionJeu.Check => new CheckCommande(joueur),
        _ => throw new ArgumentException("Action de joueur invalide")
    };
}
