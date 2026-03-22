using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.tests.Fakes;

namespace casino.core.tests.Games.Poker.Rounds.Phases;

[TestClass]
public class ShowdownStateTests
{
    [TestMethod]
    public void Advance_ShouldLeaveRoundInShowdownWithoutThrowing()
    {
        var player = new HumanPlayer("Alice", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateCards()));
        round.MoveToNextPhase(Phase.Showdown, new ShowdownState());

        round.AdvancePhase();

        Assert.AreEqual(Phase.Showdown, round.Phase);
        Assert.IsInstanceOfType<ShowdownState>(round.PhaseState);
    }

    [TestMethod]
    public void GetAvailableActions_ShouldReturnEmptySequence()
    {
        var player = new HumanPlayer("Alice", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateCards()));
        var state = new ShowdownState();

        var actions = state.GetAvailableActions(player, round).ToList();

        Assert.IsEmpty(actions);
    }

    [TestMethod]
    public void ApplyAction_ShouldThrowInvalidOperationException()
    {
        var player = new HumanPlayer("Alice", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateCards()));
        var state = new ShowdownState();

        Assert.Throws<InvalidOperationException>(() => state.ApplyAction(player, new GameAction(PokerTypeAction.Check), round));
    }

    private static IEnumerable<Card> CreateCards() =>
    [
        new Card(CardRank.As, Suit.Hearts),
        new Card(CardRank.Roi, Suit.Spades)
    ];
}
