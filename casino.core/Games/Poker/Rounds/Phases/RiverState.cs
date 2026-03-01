namespace casino.core.Games.Poker.Rounds.Phases;

public class RiverState : PhaseStateBase
{
    public override void Avancer(Round context)
    {
        context.EndGame();
        context.PhaseState = new ShowdownState();
    }
}
