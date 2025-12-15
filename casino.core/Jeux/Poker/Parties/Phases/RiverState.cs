namespace casino.core.Jeux.Poker.Parties.Phases;

public class RiverState : PhaseStateBase
{
    public override void Avancer(Partie context)
    {
        context.TerminerPartie();
        context.PhaseState = new ShowdownState();
    }
}
