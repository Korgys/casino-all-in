using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Poker.Rounds.Phases;

public class PreFlopPhaseState : PhaseStateBase
{
    public override void Avancer(Round context)
    {
        context.Phase = Phase.Flop;
        context.PhaseState = new FlopPhaseState();
        context.CommunityCards.Flop1 = context.Deck.DrawCard();
        context.CommunityCards.Flop2 = context.Deck.DrawCard();
        context.CommunityCards.Flop3 = context.Deck.DrawCard();
    }
}
