using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using System;
using System.Collections.Generic;

namespace casino.core.Games.Poker.Parties.Phases;

public class ShowdownState : IPhaseState
{
    public void Avancer(Partie context)
    {
        // Partie terminée, aucune phase supplémentaire.
    }

    public IEnumerable<TypeActionJeu> ObtenirActionsPossibles(Player Player, Partie context)
    {
        return Array.Empty<TypeActionJeu>();
    }

    public void AppliquerAction(Player Player, Actions.ActionJeu action, Partie context)
    {
        throw new InvalidOperationException("Aucune action n'est autorisée pendant le showdown.");
    }
}
