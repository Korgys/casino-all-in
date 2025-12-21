using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Linq;

namespace casino.core.Jeux.Poker.Parties;

public class GestionnaireDeTour
{
    private readonly Partie _partie;

    public int JoueurInitialIndex { get; }
    public int JoueurActuelIndex { get; private set; }

    public GestionnaireDeTour(Partie partie, int joueurInitialIndex)
    {
        _partie = partie ?? throw new ArgumentNullException(nameof(partie));
        JoueurInitialIndex = joueurInitialIndex;
        JoueurActuelIndex = joueurInitialIndex;
    }

    public Joueur ObtenirJoueurQuiDoitJouer()
    {
        if (!_partie.EnCours())
        {
            return _partie.Joueurs[JoueurActuelIndex];
        }

        PositionnerSurJoueurDisponible();

        if (!_partie.EnCours())
        {
            return _partie.Joueurs[JoueurActuelIndex];
        }

        if (AucunJoueurPeutJouer())
        {
            throw new InvalidOperationException("Aucun joueur en jeu à la table.");
        }

        return _partie.Joueurs[JoueurActuelIndex];
    }

    public void TraiterActionJoueur(Joueur joueur, ActionJeu action)
    {
        _partie.AppliquerAction(joueur, action);
        PasserAuJoueurSuivant();
    }

    private void PasserAuJoueurSuivant()
    {
        if (!_partie.EnCours())
        {
            return;
        }

        if (_partie.EstTourEnchereCloture())
        {
            AvancerJusquALaProchainePhaseAvecActions();
            return;
        }

        DeplacerIndexVersProchainJoueur();

        if (AucunJoueurPeutJouer())
        {
            AvancerJusquALaProchainePhaseAvecActions();
        }
    }

    private void PositionnerSurJoueurDisponible()
    {
        if (AucunJoueurPeutJouer())
        {
            AvancerJusquALaProchainePhaseAvecActions();
            return;
        }

        if (!JoueurPeutJouer(_partie.Joueurs[JoueurActuelIndex]))
        {
            DeplacerIndexVersProchainJoueur();
        }
    }

    private void AvancerJusquALaProchainePhaseAvecActions()
    {
        while (_partie.EnCours())
        {
            if (!AucunJoueurPeutJouer() && !_partie.EstTourEnchereCloture())
            {
                DeplacerIndexVersProchainJoueur();
                return;
            }

            _partie.AvancerPhase();

            if (!_partie.EnCours())
            {
                return;
            }

            JoueurActuelIndex = JoueurInitialIndex;

            if (!AucunJoueurPeutJouer())
            {
                if (!JoueurPeutJouer(_partie.Joueurs[JoueurActuelIndex]))
                {
                    DeplacerIndexVersProchainJoueur();
                }

                return;
            }
        }
    }

    private void DeplacerIndexVersProchainJoueur()
    {
        var essais = 0;
        do
        {
            JoueurActuelIndex = (JoueurActuelIndex + 1) % _partie.Joueurs.Count;
            essais++;
        } while (!JoueurPeutJouer(_partie.Joueurs[JoueurActuelIndex]) && essais <= _partie.Joueurs.Count);
    }

    private bool JoueurPeutJouer(Joueur joueur)
    {
        if (joueur.DerniereAction == TypeActionJeu.SeCoucher)
            return false;

        if (joueur.DerniereAction == TypeActionJeu.Tapis)
            return false;

        return _partie.ObtenirActionsPossibles(joueur).Any();
    }

    private bool AucunJoueurPeutJouer() => !_partie.Joueurs.Any(JoueurPeutJouer);
}