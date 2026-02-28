namespace casino.core.Games.Poker.Parties.Phases;

using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using System.Collections.Generic;

public interface IPhaseState
{
    void Avancer(Partie context);
    IEnumerable<TypeActionJeu> ObtenirActionsPossibles(Player Player, Partie context);
    void AppliquerAction(Player Player, ActionJeu action, Partie context);
}
