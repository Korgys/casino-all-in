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
        _joueur.DerniereAction = TypeAction.Tapis;
        partie.Pot += _joueur.Jetons;
        _joueur.Jetons = 0;
        _joueur.EstTapis = true;
    }
}
