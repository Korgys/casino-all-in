using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using System.Linq;

namespace casino.core.Jeux.Poker.Actions.Commandes;

public class CoucherCommande : IJoueurCommande
{
    private readonly Joueur _joueur;

    public CoucherCommande(Joueur joueur)
    {
        _joueur = joueur;
    }

    public void Execute(Partie partie)
    {
        _joueur.EstCouche = true;
        _joueur.DerniereAction = TypeActionJeu.SeCoucher;

        if (partie.Joueurs.Count(j => !j.EstCouche) == 1)
        {
            partie.TerminerPartie();
        }
    }
}
