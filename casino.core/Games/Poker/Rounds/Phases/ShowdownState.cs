using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;

namespace casino.core.Games.Poker.Rounds.Phases;

public class ShowdownState : IPhaseState
{
    public void Avancer(Round context)
    {
        // Round is complete; no additional phase is available.
    }

    public IEnumerable<PokerTypeAction> GetAvailableActions(Player player, Round context)
    {
        return Array.Empty<PokerTypeAction>();
    }

    public void ApplyAction(Player player, Actions.GameAction action, Round context)
    {
        throw new InvalidOperationException("No action is allowed during showdown.");
    }
}
