using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using System;

namespace casino.core.Jeux.Poker.Actions.Commandes;

public class MiserCommande : IJoueurCommande
{
    private readonly Joueur _joueur;
    private readonly int _montant;

    public MiserCommande(Joueur joueur, int montant)
    {
        _joueur = joueur;
        _montant = montant;
    }

    public void Execute(Partie partie)
    {
        if (_montant <= 0)
        {
            throw new ArgumentException("Le montant de mise doit être supérieur à zéro.");
        }

        if (partie.MiseActuelle > _montant)
        {
            throw new InvalidOperationException("La mise ne peut pas être inférieure à la mise de départ/actuelle.");
        }

        if (_montant > _joueur.Jetons)
        {
            throw new InvalidOperationException("Le joueur n'a pas assez de jetons pour miser autant.");
        }

        _joueur.DerniereAction = TypeAction.Miser;
        partie.MiseActuelle = _montant;
        _joueur.Jetons -= _montant;
        partie.Pot += _montant;
    }
}
