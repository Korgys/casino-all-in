namespace casino.core.Games.Poker.Rounds.Phases;

using System.Collections.Generic;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;

public interface IPhaseState
{
    void Avancer(Round context);
    IEnumerable<PokerTypeAction> GetAvailableActions(Player player, Round context);
    void ApplyAction(Player player, GameAction action, Round context);
}
