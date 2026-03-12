namespace casino.core.Games.Poker.Rounds.Phases;

public class TurnState : PhaseStateBase
{
    public override void Avancer(Round context)
    {
        context.MoveToNextPhase(Phase.River, new RiverState());
        context.RevealRiver(context.Deck.DrawCard());
    }
}
