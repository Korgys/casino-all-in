using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;

namespace casino.core.Jeux.Poker.Actions.Commandes;

public class TapisCommande : IJoueurCommande
{
    private readonly Joueur _joueur;

    public TapisCommande(Joueur joueur)
    {
        _joueur = joueur;
    }

    public void Execute(Partie partie)
    {
        var miseAvant = partie.ObtenirMisePour(_joueur);
        var contribution = miseAvant + _joueur.Jetons;
        _joueur.DerniereAction = TypeActionJeu.Tapis;
        partie.Pot += _joueur.Jetons;
        partie.DefinirMisePour(_joueur, contribution);
        partie.MiseActuelle = Math.Max(partie.MiseActuelle, contribution);
        _joueur.Jetons = 0;
    }
}
