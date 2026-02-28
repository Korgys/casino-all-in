using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using System;
using System.Linq;

namespace casino.core.Games.Poker.Parties;

public class GestionnaireDeTour
{
    private readonly Partie _partie;

    public int PlayerInitialIndex { get; }
    public int PlayerActuelIndex { get; private set; }

    public GestionnaireDeTour(Partie partie, int PlayerInitialIndex)
    {
        _partie = partie ?? throw new ArgumentNullException(nameof(partie));
        PlayerInitialIndex = PlayerInitialIndex;
        PlayerActuelIndex = PlayerInitialIndex;
    }

    public Player ObtenirPlayerQuiDoitJouer()
    {
        if (!_partie.EnCours())
        {
            return _partie.Players[PlayerActuelIndex];
        }

        PositionnerSurPlayerDisponible();

        if (!_partie.EnCours())
        {
            return _partie.Players[PlayerActuelIndex];
        }

        if (AucunPlayerPeutJouer())
        {
            throw new InvalidOperationException("Aucun Player en jeu à la table.");
        }

        return _partie.Players[PlayerActuelIndex];
    }

    public void TraiterActionPlayer(Player Player, ActionJeu action)
    {
        _partie.AppliquerAction(Player, action);
        PasserAuPlayerSuivant();
    }

    private void PasserAuPlayerSuivant()
    {
        if (!_partie.EnCours())
        {
            return;
        }

        if (_partie.IsBettingRoundClosed())
        {
            AvancerJusquALaProchainePhaseAvecActions();
            return;
        }

        DeplacerIndexVersProchainPlayer();

        if (AucunPlayerPeutJouer())
        {
            AvancerJusquALaProchainePhaseAvecActions();
        }
    }

    private void PositionnerSurPlayerDisponible()
    {
        if (AucunPlayerPeutJouer())
        {
            AvancerJusquALaProchainePhaseAvecActions();
            return;
        }

        if (!PlayerPeutJouer(_partie.Players[PlayerActuelIndex]))
        {
            DeplacerIndexVersProchainPlayer();
        }
    }

    private void AvancerJusquALaProchainePhaseAvecActions()
    {
        while (_partie.EnCours())
        {
            if (!AucunPlayerPeutJouer() && !_partie.IsBettingRoundClosed())
            {
                DeplacerIndexVersProchainPlayer();
                return;
            }

            _partie.AvancerPhase();

            if (!_partie.EnCours())
            {
                return;
            }

            PlayerActuelIndex = PlayerInitialIndex;

            if (!AucunPlayerPeutJouer())
            {
                if (!PlayerPeutJouer(_partie.Players[PlayerActuelIndex]))
                {
                    DeplacerIndexVersProchainPlayer();
                }

                return;
            }
        }
    }

    private void DeplacerIndexVersProchainPlayer()
    {
        var essais = 0;
        do
        {
            PlayerActuelIndex = (PlayerActuelIndex + 1) % _partie.Players.Count;
            essais++;
        } while (!PlayerPeutJouer(_partie.Players[PlayerActuelIndex]) && essais <= _partie.Players.Count);
    }

    private bool PlayerPeutJouer(Player Player)
    {
        if (Player.LastAction == TypeActionJeu.SeCoucher)
            return false;

        if (Player.LastAction == TypeActionJeu.Tapis)
            return false;

        return _partie.ObtenirActionsPossibles(Player).Any();
    }

    private bool AucunPlayerPeutJouer() => !_partie.Players.Any(PlayerPeutJouer);
}