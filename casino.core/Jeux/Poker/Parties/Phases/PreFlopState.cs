using casino.core.Jeux.Poker.Cartes;

namespace casino.core.Jeux.Poker.Parties.Phases;

public class PreFlopState : PhaseStateBase
{
    public override void Avancer(Partie context)
    {
        context.Phase = Phase.Flop;
        context.PhaseState = new FlopState();
        context.CartesCommunes.Flop1 = JeuDeCartes.Instance.TirerCarte();
        context.CartesCommunes.Flop2 = JeuDeCartes.Instance.TirerCarte();
        context.CartesCommunes.Flop3 = JeuDeCartes.Instance.TirerCarte();
    }
}
