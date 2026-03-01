namespace casino.core.Games.Poker.Rounds.Phases;

using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using System.Collections.Generic;

public interface IPhaseState
{
    void Avancer(Round context);
    IEnumerable<PokerTypeAction> GetAvailableActions(Player Player, Round context);
    void ApplyAction(Player Player, GameAction action, Round context);
}
