namespace casino.core.Games.Poker.Rounds.Phases;

public class FlopPhaseState : PhaseStateBase
{
    public override void Avancer(Round context)
    {
        context.MoveToNextPhase(Phase.Turn, new TurnState());
        context.RevealTurn(context.Deck.DrawCard());
    }
}
