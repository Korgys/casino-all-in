using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Games.Poker.Parties;

public class TablePoker
{
    public string Name { get; set; }
    public Partie Partie { get; private set; }
    public List<Player> Players { get; private set; }
    public GestionnaireDeTour GestionnaireDeTour { get; private set; }
    public int PlayerInitialIndex => GestionnaireDeTour?.PlayerInitialIndex ?? _playerInitialIndex;
    public int CurrentPlayerIndex => GestionnaireDeTour?.PlayerActuelIndex ?? _playerInitialIndex;
    public int PlayerActuelIndex => CurrentPlayerIndex;
    private int _playerInitialIndex = -1;

    public void DemarrerPartie(List<Player> Players, IDeck deck)
    {
        Players.ForEach(j => j.Reset());

        this.Players = Players;
        Partie = new Partie(this.Players, deck);
        _playerInitialIndex = (_playerInitialIndex + 1) % this.Players.Count;
        GestionnaireDeTour = new GestionnaireDeTour(Partie, _playerInitialIndex);
    }

    public List<TypeActionJeu> ObtenirActionsPossibles(Player Player)
        => Partie.ObtenirActionsPossibles(Player).OrderBy(a => (int)a).ToList();

    public void TraiterActionPlayer(Player Player, Actions.ActionJeu choix)
    {
        GestionnaireDeTour.TraiterActionPlayer(Player, choix);
    }

    public Player ObtenirPlayerQuiDoitJouer()
        => GestionnaireDeTour.ObtenirPlayerQuiDoitJouer();
}