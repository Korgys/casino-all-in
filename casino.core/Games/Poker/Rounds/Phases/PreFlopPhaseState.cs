using casino.core.Games.Poker.Cartes;

namespace casino.core.Games.Poker.Parties.Phases;

public class PreFlopPhaseState : PhaseStateBase
{
    public override void Avancer(Partie context)
    {
        context.Phase = Phase.Flop;
        context.PhaseState = new FlopPhaseState();
        context.CommunityCards.Flop1 = context.Deck.TirerCarte();
        context.CommunityCards.Flop2 = context.Deck.TirerCarte();
        context.CommunityCards.Flop3 = context.Deck.TirerCarte();
    }
}
