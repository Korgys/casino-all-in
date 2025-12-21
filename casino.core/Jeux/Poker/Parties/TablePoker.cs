using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Jeux.Poker.Parties;

public class TablePoker
{
    public string Nom { get; set; }
    public Partie Partie { get; private set; }
    public List<Joueur> Joueurs { get; private set; }
    public GestionnaireDeTour GestionnaireDeTour { get; private set; }
    public int JoueurInitialIndex => GestionnaireDeTour?.JoueurInitialIndex ?? _joueurInitialIndex;
    public int JoueurActuelIndex => GestionnaireDeTour?.JoueurActuelIndex ?? _joueurInitialIndex;
    private int _joueurInitialIndex = -1;

    public void DemarrerPartie(List<Joueur> joueurs, IDeck deck)
    {
        joueurs.ForEach(j => j.Reinitialiser());

        Joueurs = joueurs;
        Partie = new Partie(joueurs, deck);
        _joueurInitialIndex = (_joueurInitialIndex + 1) % joueurs.Count;
        GestionnaireDeTour = new GestionnaireDeTour(Partie, _joueurInitialIndex);
    }

    public List<TypeActionJeu> ObtenirActionsPossibles(Joueur joueur)
        => Partie.ObtenirActionsPossibles(joueur).OrderBy(a => (int)a).ToList();

    public void TraiterActionJoueur(Joueur joueur, Actions.ActionJeu choix)
    {
        GestionnaireDeTour.TraiterActionJoueur(joueur, choix);
    }

    public Joueur ObtenirJoueurQuiDoitJouer()
        => GestionnaireDeTour.ObtenirJoueurQuiDoitJouer();
}