using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Collections.Generic;

namespace casino.core.Jeux.Poker.Parties.Phases;

public class ShowdownState : IPhaseState
{
    public void Avancer(Partie context)
    {
        // Partie terminée, aucune phase supplémentaire.
    }

    public IEnumerable<TypeAction> ObtenirActionsPossibles(Joueur joueur, Partie context)
    {
        return Array.Empty<TypeAction>();
    }

    public void AppliquerAction(Joueur joueur, Actions.Action action, Partie context)
    {
        throw new InvalidOperationException("Aucune action n'est autorisée pendant le showdown.");
    }
}
