using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.tests.Fakes;
using casino.core.tests.Games.Poker.Players;

namespace casino.core.tests.Games.Poker.Rounds;

[TestClass]
public class TurnManagerTests
{
    [TestMethod]
    public void Constructor_WhenRoundIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TurnManager(null!, 0));
    }

    [TestMethod]
    public void GetPlayerToAct_ShouldSkipFoldedAndAllInPlayers()
    {
        var folded = new HumanPlayer("Folded", 100) { LastAction = PokerTypeAction.Fold };
        var allIn = new HumanPlayer("AllIn", 100) { LastAction = PokerTypeAction.AllIn };
        var actionable = new HumanPlayer("Actionable", 100);
        var round = new Round(
            [folded, allIn, actionable],
            new FakeDeck(Enumerable.Repeat(new Card(CardRank.Deux, Suit.Hearts), 20)), 0);
        var sut = new TurnManager(round, 0);

        var playerToAct = sut.GetPlayerToAct();

        Assert.AreEqual(actionable, playerToAct);
        Assert.AreEqual(2, sut.CurrentPlayerIndex);
    }

    [TestMethod]
    public void ExecutePlayerAction_WhenBettingRoundCloses_ShouldAdvancePhaseAndResetTurn()
    {
        var alice = new HumanPlayer("Alice", 100);
        var bob = new HumanPlayer("Bob", 100);
        var round = new Round(
            [alice, bob],
            new FakeDeck(Enumerable.Repeat(new Card(CardRank.Trois, Suit.Clubs), 20)), 0);
        var sut = new TurnManager(round, 0);
        PlayerTestHelper.SetCurrentBet(round, 0);

        sut.ExecutePlayerAction(alice, new GameAction(PokerTypeAction.Bet, 10));
        sut.ExecutePlayerAction(bob, new GameAction(PokerTypeAction.Call));

        Assert.AreEqual(Phase.Flop, round.Phase);
        Assert.AreEqual(0, sut.CurrentPlayerIndex);
        Assert.AreEqual(0, round.CurrentBet);
    }

    [TestMethod]
    public void GetPlayerToAct_WhenNoPlayersCanAct_ShouldAdvanceUntilShowdown()
    {
        var alice = new HumanPlayer("Alice", 100) { LastAction = PokerTypeAction.AllIn };
        var bob = new HumanPlayer("Bob", 100) { LastAction = PokerTypeAction.AllIn };
        var round = new Round(
            [alice, bob],
            new FakeDeck(Enumerable.Repeat(new Card(CardRank.Quatre, Suit.Spades), 20)), 0);
        var sut = new TurnManager(round, 0);

        var player = sut.GetPlayerToAct();

        Assert.AreEqual(Phase.Showdown, round.Phase);
        Assert.AreEqual(alice, player);
        Assert.IsNotNull(round.CommunityCards.Flop1);
        Assert.IsNotNull(round.CommunityCards.River);
    }

    [TestMethod]
    public void GetPlayerToAct_WhenNoPlayerCanActAfterPositioning_ShouldThrowInvalidOperationException()
    {
        var alice = new HumanPlayer("Alice", 100);
        var bob = new HumanPlayer("Bob", 100);
        var round = new Round(
            [alice, bob],
            new FakeDeck(Enumerable.Repeat(new Card(CardRank.Cinq, Suit.Diamonds), 20)), 0);
        round.PhaseState = new SingleWindowActionPhaseState(bob);
        var sut = new TurnManager(round, 0);

        var ex = Assert.Throws<InvalidOperationException>(sut.GetPlayerToAct);

        Assert.AreEqual("No active player at the table.", ex.Message);
    }

    private sealed class SingleWindowActionPhaseState(Player temporarilyActionablePlayer) : IPhaseState
    {
        private int _calls;

        public void Avancer(Round context)
        {
            context.MoveToNextPhase(Phase.Flop, new FlopPhaseState());
        }

        public IEnumerable<PokerTypeAction> GetAvailableActions(Player player, Round context)
        {
            if (_calls == 1 && ReferenceEquals(player, temporarilyActionablePlayer))
            {
                _calls++;
                return [PokerTypeAction.Check];
            }

            _calls++;
            return Enumerable.Empty<PokerTypeAction>();
        }

        public void ApplyAction(Player player, GameAction action, Round context)
        {
        }
    }
}