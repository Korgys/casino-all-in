using casino.core.Jeux.Poker.Actions;
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

    public List<TypeActionJeu> ObtenirActionsPossibles(Joueur joueur)
        => Partie.ObtenirActionsPossibles(joueur).OrderBy(a => (int)a).ToList();

    public void TraiterActionJoueur(Joueur joueur, Actions.ActionJeu choix)
    {
        Partie.AppliquerAction(joueur, choix);
        PasserAuJoueurSuivant();
    }

    public Joueur ObtenirJoueurQuiDoitJouer()
    {
        if (!Partie.EnCours()) 
            return Joueurs[JoueurActuelIndex];

        if (!Joueurs.Any(JoueurPeutJouer))
            throw new InvalidOperationException("Aucun joueur en jeu à la table.");

        while (!JoueurPeutJouer(Joueurs[JoueurActuelIndex]))
        {
            // Passe au joueur suivant si celui-ci est couché
            JoueurActuelIndex = (JoueurActuelIndex + 1) % Joueurs.Count;
        }

        return Joueurs[JoueurActuelIndex];
    }

    private void PasserAuJoueurSuivant()
    {
        // Si la partie est terminée, ne rien faire
        if (!Partie.EnCours())
        {
            return;
        }

        // Vérifie si le tour d'enchère est terminé
        if (Partie.EstTourEnchereCloture())
        {
            // Avance à la phase suivante
            Partie.AvancerPhase();
            if (!Partie.EnCours())
            {
                return;
            }

            JoueurActuelIndex = JoueurInitialIndex;
        }
        else
        {
            // Passe au joueur suivant tant qu'il n'y a pas d'actions possibles
            var essais = 0;
            do
            {
                JoueurActuelIndex = (JoueurActuelIndex + 1) % Joueurs.Count;
                essais++;
            } while (!JoueurPeutJouer(Joueurs[JoueurActuelIndex]) && essais <= Joueurs.Count);
        }
    }

    private bool JoueurPeutJouer(Joueur joueur)
    {
        // Retourner false si le joueur ne peut pas jouer.
        if (joueur.DerniereAction == TypeActionJeu.SeCoucher)
            return false;

        if (joueur.DerniereAction == TypeActionJeu.Tapis)
            return false;

        return ObtenirActionsPossibles(joueur).Any();
    }
}
