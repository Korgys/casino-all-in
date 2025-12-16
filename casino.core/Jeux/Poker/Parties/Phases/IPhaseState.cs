namespace casino.core.Jeux.Poker.Parties.Phases;

using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using System.Collections.Generic;

public interface IPhaseState
{
    void Avancer(Partie context);
    IEnumerable<TypeAction> ObtenirActionsPossibles(Joueur joueur, Partie context);
    void AppliquerAction(Joueur joueur, Action action, Partie context);
}
