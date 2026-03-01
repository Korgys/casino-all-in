using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Poker.Rounds.Phases;

public class TurnState : PhaseStateBase
{
    public override void Avancer(Round context)
    {
        context.Phase = Phase.River;
        context.PhaseState = new RiverState();
        context.CommunityCards.River = context.Deck.DrawCard();
    }
}
