using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using System;

namespace casino.core.Jeux.Poker.Actions.Commandes;

public class RelancerCommande : IJoueurCommande
{
    private readonly Joueur _joueur;
    private readonly int _montant;

    public RelancerCommande(Joueur joueur, int montant)
    {
        _joueur = joueur;
        _montant = montant;
    }

    public void Execute(Partie partie)
    {
        var contributionActuelle = partie.ObtenirMisePour(_joueur);
        var difference = _montant - contributionActuelle;

        if (_montant <= partie.MiseActuelle)
        {
            throw new ArgumentException("La relance doit être supérieure ou égale à la mise actuelle.");
        }

        if (difference > _joueur.Jetons)
        {
            throw new ArgumentException("Le joueur n'a pas assez de jetons pour relancer autant.");
        }

        if (difference == _joueur.Jetons)
        {
            new TapisCommande(_joueur).Execute(partie);
            return;
        }

        _joueur.DerniereAction = TypeActionJeu.Relancer;
        _joueur.Jetons -= difference;
        partie.MiseActuelle = _montant;
        partie.DefinirMisePour(_joueur, _montant);
        partie.Pot += difference;
    }
}
