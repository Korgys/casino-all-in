using casino.core.Games.Poker.Cartes;

namespace casino.core.Games.Poker.Parties.Phases;

public class FlopPhaseState : PhaseStateBase
{
    public override void Avancer(Partie context)
    {
        context.Phase = Phase.Turn;
        context.PhaseState = new TurnState();
        context.CommunityCards.Turn = context.Deck.TirerCarte();
    }
}
