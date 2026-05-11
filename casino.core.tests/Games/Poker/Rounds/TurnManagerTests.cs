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

    [TestMethod]
    public void GetPlayerToAct_WhenAllButOnePlayerFolded_ShouldReturnRemainingPlayer()
    {
        var foldedA = new HumanPlayer("FoldedA", 100) { LastAction = PokerTypeAction.Fold };
        var active = new HumanPlayer("Active", 100);
        var foldedB = new HumanPlayer("FoldedB", 100) { LastAction = PokerTypeAction.Fold };
        var round = new Round(
            [foldedA, active, foldedB],
            new FakeDeck(Enumerable.Repeat(new Card(CardRank.Six, Suit.Hearts), 20)), 0);
        var sut = new TurnManager(round, 0);

        var playerToAct = sut.GetPlayerToAct();

        Assert.AreEqual(active, playerToAct);
        Assert.AreEqual(1, sut.CurrentPlayerIndex);
    }

    [TestMethod]
    public void GetPlayerToAct_WhenMultiplePlayersAreAllIn_ShouldSelectNextEligiblePlayer()
    {
        var allInA = new HumanPlayer("AllInA", 100) { LastAction = PokerTypeAction.AllIn };
        var allInB = new HumanPlayer("AllInB", 100) { LastAction = PokerTypeAction.AllIn };
        var active = new HumanPlayer("Active", 100);
        var round = new Round(
            [allInA, allInB, active],
            new FakeDeck(Enumerable.Repeat(new Card(CardRank.Sept, Suit.Clubs), 20)), 0);
        var sut = new TurnManager(round, 0);

        var playerToAct = sut.GetPlayerToAct();

        Assert.AreEqual(active, playerToAct);
        Assert.AreEqual(2, sut.CurrentPlayerIndex);
    }

    [TestMethod]
    public void ExecutePlayerAction_WhenPhaseAdvancesAndInitialPlayerCannotAct_ShouldReselectActor()
    {
        var foldedInitial = new HumanPlayer("FoldedInitial", 100) { LastAction = PokerTypeAction.Fold };
        var bettor = new HumanPlayer("Bettor", 100);
        var caller = new HumanPlayer("Caller", 100);
        var round = new Round(
            [foldedInitial, bettor, caller],
            new FakeDeck(Enumerable.Repeat(new Card(CardRank.Huit, Suit.Spades), 20)), 0);
        var sut = new TurnManager(round, 0);
        PlayerTestHelper.SetCurrentBet(round, 0);

        sut.ExecutePlayerAction(bettor, new GameAction(PokerTypeAction.Bet, 10));
        sut.ExecutePlayerAction(caller, new GameAction(PokerTypeAction.Call));

        Assert.AreEqual(Phase.Flop, round.Phase);
        Assert.AreEqual(1, sut.CurrentPlayerIndex);
        Assert.AreEqual(bettor, sut.GetPlayerToAct());
    }

    [TestMethod]
    public void GetPlayerToAct_WhenNoAvailableActionsAfterPhaseReset_ShouldThrowInvalidOperationException()
    {
        var alice = new HumanPlayer("Alice", 100);
        var bob = new HumanPlayer("Bob", 100);
        var round = new Round(
            [alice, bob],
            new FakeDeck(Enumerable.Repeat(new Card(CardRank.Neuf, Suit.Diamonds), 20)), 0);
        round.PhaseState = new NoActionsAfterAdvancePhaseState();
        alice.LastAction = PokerTypeAction.Check;
        bob.LastAction = PokerTypeAction.Check;
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

    private sealed class NoActionsAfterAdvancePhaseState : IPhaseState
    {
        public void Avancer(Round context)
        {
            context.MoveToNextPhase(Phase.Flop, new EmptyActionsPhaseState());
        }

        public IEnumerable<PokerTypeAction> GetAvailableActions(Player player, Round context)
        {
            return [PokerTypeAction.Check];
        }

        public void ApplyAction(Player player, GameAction action, Round context)
        {
        }
    }

    private sealed class EmptyActionsPhaseState : IPhaseState
    {
        public void Avancer(Round context)
        {
            context.MoveToNextPhase(Phase.Showdown, new ShowdownState());
        }

        public IEnumerable<PokerTypeAction> GetAvailableActions(Player player, Round context)
        {
            return Enumerable.Empty<PokerTypeAction>();
        }

        public void ApplyAction(Player player, GameAction action, Round context)
        {
        }
    }
}
