using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Poker.Rounds.Phases;

public class FlopPhaseState : PhaseStateBase
{
    public override void Avancer(Round context)
    {
        context.Phase = Phase.Turn;
        context.PhaseState = new TurnState();
        context.CommunityCards.Turn = context.Deck.DrawCard();
    }
}
