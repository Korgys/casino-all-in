namespace casino.core.Jeux.Poker.Parties.Phases;

using casino.core.Jeux.Poker.Joueurs;
using System.Collections.Generic;

public interface IPhaseState
{
    void Avancer(Partie context);
    IEnumerable<JoueurActionType> ObtenirActionsPossibles(Joueur joueur, Partie context);
    void AppliquerAction(Joueur joueur, JoueurAction action, Partie context);
}
