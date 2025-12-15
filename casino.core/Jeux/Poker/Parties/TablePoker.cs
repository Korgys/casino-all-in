using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace casino.core.Jeux.Poker.Parties;

public class TablePoker
{
    public string Nom {  get; set; }
    public Partie Partie { get; set; }
    public List<Joueur> Joueurs { get; set; }
    public int JoueurInitialIndex { get; set; } = -1;
    public int JoueurActuelIndex { get; set; }

    public void DemarrerPartie(List<Joueur> joueurs, IDeck deck)
    {
        joueurs.ForEach(j => j.Reinitialiser());

        Joueurs = joueurs;
        Partie = new Partie(joueurs, deck);
        JoueurInitialIndex = (JoueurInitialIndex + 1) % joueurs.Count;
        JoueurActuelIndex = JoueurInitialIndex;
    }

    public Joueur ObtenirJoueurQuiDoitJouer()
        => Joueurs[JoueurActuelIndex];

    public List<JoueurActionType> ObtenirActionsPossibles(Joueur joueur)
        => Partie.ObtenirActionsPossibles(joueur).OrderBy(a => (int) a).ToList();

    public void TraiterActionJoueur(Joueur joueur, JoueurAction choix)
    {
        Partie.AppliquerAction(joueur, choix);
        PasserAuJoueurSuivant();
    }

    private void PasserAuJoueurSuivant()
    {
        // Passer au joueur suivant tant qu'il n'y a pas d'actions possibles
        do
        {
            JoueurActuelIndex = (JoueurActuelIndex + 1) % Joueurs.Count;
            if (JoueurActuelIndex == JoueurInitialIndex)
            {
                Partie.AvancerPhase();
                if (!Partie.EnCours())
                {
                    return;
                }
            }
        } while (Joueurs[JoueurActuelIndex].DerniereAction == JoueurActionType.SeCoucher
            || Joueurs[JoueurActuelIndex].DerniereAction == JoueurActionType.Tapis
            || ObtenirActionsPossibles(Joueurs[JoueurActuelIndex]).Count == 0);
    }
}
