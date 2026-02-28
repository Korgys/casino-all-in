using casino.core.Games.Poker.Cartes;

namespace casino.core.Games.Poker.Parties.Phases;

public class TurnState : PhaseStateBase
{
    public override void Avancer(Partie context)
    {
        context.Phase = Phase.River;
        context.PhaseState = new RiverState();
        context.CommunityCards.River = context.Deck.TirerCarte();
    }
}
