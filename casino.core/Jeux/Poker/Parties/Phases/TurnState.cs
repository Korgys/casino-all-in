using casino.core.Jeux.Poker.Cartes;

namespace casino.core.Jeux.Poker.Parties.Phases;

public class TurnState : PhaseStateBase
{
    public override void Avancer(Partie context)
    {
        context.Phase = Phase.River;
        context.PhaseState = new RiverState();
        context.CartesCommunes.River = JeuDeCartes.Instance.TirerCarte();
    }
}
