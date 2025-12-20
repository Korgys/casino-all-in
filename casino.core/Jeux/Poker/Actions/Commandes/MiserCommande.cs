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

        var miseJoueur = partie.ObtenirMisePour(_joueur);
        var difference = _montant - miseJoueur;

        if (difference <= 0)
        {
            throw new InvalidOperationException("La mise doit augmenter la contribution du joueur.");
        }

        if (difference > _joueur.Jetons)
        {
            throw new InvalidOperationException("Le joueur n'a pas assez de jetons pour miser autant.");
        }

        _joueur.DerniereAction = TypeActionJeu.Miser;
        // Mettre à jour la contribution du joueur et le pot en fonction de la différence,
        // pas du montant total, pour gérer les mises partielles déjà effectuées.
        _joueur.Jetons -= difference;
        partie.DefinirMisePour(_joueur, _montant);
        partie.MiseActuelle = _montant;
        partie.Pot += difference;
    }
}
