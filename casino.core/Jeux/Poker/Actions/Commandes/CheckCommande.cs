using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using System;

namespace casino.core.Jeux.Poker.Actions.Commandes;

public class CheckCommande : IJoueurCommande
{
    private readonly Joueur _joueur;

    public CheckCommande(Joueur joueur)
    {
        _joueur = joueur;
    }

    public void Execute(Partie partie)
    {
        if (partie.MiseActuelle != 0)
        {
            throw new InvalidOperationException("Le joueur ne peut pas checker car il y a une mise sur la table.");
        }

        _joueur.DerniereAction = TypeAction.Check;
    }
}
