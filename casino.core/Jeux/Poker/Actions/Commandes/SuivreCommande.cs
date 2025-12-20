using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using System;

namespace casino.core.Jeux.Poker.Actions.Commandes;

public class SuivreCommande : IJoueurCommande
{
    private readonly Joueur _joueur;

    public SuivreCommande(Joueur joueur)
    {
        _joueur = joueur;
    }

    public void Execute(Partie partie)
    {
        var contributionActuelle = partie.ObtenirMisePour(_joueur);
        var difference = partie.MiseActuelle - contributionActuelle;

        if (difference <= 0)
        {
            throw new InvalidOperationException("Aucune mise supplémentaire à suivre.");
        }

        if (_joueur.Jetons - difference < 0)
        {
            throw new InvalidOperationException("Le joueur n'a pas assez de jetons pour suivre la mise actuelle.");
        }

        if (_joueur.Jetons - difference == 0)
        {
            new TapisCommande(_joueur).Execute(partie);
            return;
        }

        _joueur.DerniereAction = TypeActionJeu.Suivre;
        _joueur.Jetons -= difference;
        partie.DefinirMisePour(_joueur, partie.MiseActuelle);
        partie.Pot += difference;
    }
}
