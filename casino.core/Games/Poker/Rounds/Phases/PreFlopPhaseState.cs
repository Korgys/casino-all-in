namespace casino.core.Games.Poker.Rounds.Phases;

public class PreFlopPhaseState : PhaseStateBase
{
    public override void Avancer(Round context)
    {
        context.MoveToNextPhase(Phase.Flop, new FlopPhaseState());
        context.RevealFlop(context.Deck.DrawCard(), context.Deck.DrawCard(), context.Deck.DrawCard());
    }
}
