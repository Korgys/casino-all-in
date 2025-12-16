using casino.core.Jeux.Poker.Cartes;

namespace casino.core.Jeux.Poker.Parties.Phases;

public class PreFlopPhaseState : PhaseStateBase
{
    public override void Avancer(Partie context)
    {
        context.Phase = Phase.Flop;
        context.PhaseState = new FlopPhaseState();
        context.CartesCommunes.Flop1 = context.Deck.TirerCarte();
        context.CartesCommunes.Flop2 = context.Deck.TirerCarte();
        context.CartesCommunes.Flop3 = context.Deck.TirerCarte();
    }
}
