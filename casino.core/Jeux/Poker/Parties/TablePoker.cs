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
    public int JoueurInitialIndex { get; set; } = 0;
    public int JoueurActuelIndex { get; set; } = 0;

    public void DemarrerPartie(List<Joueur> joueurs)
    {
        joueurs.ForEach(j => j.Reinitialiser());

        Joueurs = joueurs;
        Partie = new Partie(joueurs);
        JoueurInitialIndex = (JoueurInitialIndex + 1) % joueurs.Count;
        JoueurActuelIndex = JoueurInitialIndex;
    }

    public Joueur ObtenirJoueurQuiDoitJouer()
    {
        return Joueurs[JoueurActuelIndex];
    }

    public List<JoueurActionType> ObtenirActionsPossibles(Joueur joueur)
    {
        var actionsPossibles = new List<JoueurActionType>();

        if (!joueur.EstCouche)
        {
            actionsPossibles.Add(JoueurActionType.SeCoucher);

            // Si le joueur a assez de jetons
            if (Partie.MiseActuelle <= joueur.Jetons)
            {
                if (Partie.MiseActuelle == 0) actionsPossibles.Add(JoueurActionType.Miser);
                else actionsPossibles.Add(JoueurActionType.Suivre);
                actionsPossibles.Add(JoueurActionType.Relancer);
            }
            else
            {
                actionsPossibles.Add(JoueurActionType.Tapis);
            }

            if (Partie.MiseActuelle == 0)
            {
                actionsPossibles.Add(JoueurActionType.Check);
            }
        }

        return actionsPossibles.OrderBy(a => (int)a).ToList();
    }

    public void TraiterActionJoueur(Joueur joueur, JoueurAction choix)
    {
        if (!ObtenirActionsPossibles(joueur).Contains(choix.TypeAction)) 
        {
            throw new InvalidOperationException("Action de joueur non autorisée");
        }

        switch (choix.TypeAction)
        {
            case JoueurActionType.SeCoucher:
                Partie.TraiterCoucher(joueur);
                break;
            case JoueurActionType.Miser:
                Partie.TraiterMiser(joueur, choix.Montant);
                break;
            case JoueurActionType.Suivre:
                Partie.TraiterSuivre(joueur);
                break;
            case JoueurActionType.Relancer:
                Partie.TraiterRelancer(joueur, choix.Montant);
                break;
            case JoueurActionType.Tapis:
                Partie.TraiterTapis(joueur);
                break;
            case JoueurActionType.Check:
                Partie.TraiterCheck(joueur);
                break;
            default:
                throw new ArgumentException("Action de joueur invalide");
        }

        PasserAuJoueurSuivant();
    }

    private void PasserAuJoueurSuivant()
    {
        // Passer au joueur suivant tant qu'il n'y a pas d'actions possibles
        do
        {
            JoueurActuelIndex = (JoueurActuelIndex + 1) % Joueurs.Count;
            if (JoueurActuelIndex == 0)
            {
                Partie.AvancerPhase();
            }
        } while(Joueurs[JoueurActuelIndex].DerniereAction == JoueurActionType.SeCoucher 
            || Joueurs[JoueurActuelIndex].DerniereAction == JoueurActionType.Tapis
            || !ObtenirActionsPossibles(Joueurs[JoueurActuelIndex]).Any());
    }
}
