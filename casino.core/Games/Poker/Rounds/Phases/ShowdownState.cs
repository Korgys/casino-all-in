using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using System;
using System.Collections.Generic;

namespace casino.core.Games.Poker.Rounds.Phases;

public class ShowdownState : IPhaseState
{
    public void Avancer(Round context)
    {
        // Round terminée, aucune phase supplémentaire.
    }

    public IEnumerable<TypeGameAction> GetAvailableActions(Player Player, Round context)
    {
        return Array.Empty<TypeGameAction>();
    }

    public void ApplyAction(Player Player, Actions.GameAction action, Round context)
    {
        throw new InvalidOperationException("Aucune action n'est autorisée pendant le showdown.");
    }
}
