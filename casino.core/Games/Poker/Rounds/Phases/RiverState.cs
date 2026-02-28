namespace casino.core.Games.Poker.Parties.Phases;

public class RiverState : PhaseStateBase
{
    public override void Avancer(Partie context)
    {
        context.EndGame();
        context.PhaseState = new ShowdownState();
    }
}
