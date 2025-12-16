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
        if (_joueur.Jetons - partie.MiseActuelle < 0)
        {
            throw new InvalidOperationException("Le joueur n'a pas assez de jetons pour suivre la mise actuelle.");
        }

        if (partie.MiseActuelle == 0)
        {
            new CheckCommande(_joueur).Execute(partie);
            return;
        }

        if (_joueur.Jetons - partie.MiseActuelle == 0)
        {
            new TapisCommande(_joueur).Execute(partie);
            return;
        }

        _joueur.DerniereAction = TypeAction.Suivre;
        _joueur.Jetons -= partie.MiseActuelle;
        partie.Pot += partie.MiseActuelle;
    }
}
