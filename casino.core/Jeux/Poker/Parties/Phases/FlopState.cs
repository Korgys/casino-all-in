using casino.core.Jeux.Poker.Cartes;

namespace casino.core.Jeux.Poker.Parties.Phases;

public class FlopState : PhaseStateBase
{
    public override void Avancer(Partie context)
    {
        context.Phase = Phase.Turn;
        context.PhaseState = new TurnState();
        context.CartesCommunes.Turn = context.Deck.TirerCarte();
    }
}
