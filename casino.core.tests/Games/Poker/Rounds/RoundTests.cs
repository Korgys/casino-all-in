using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.tests.Fakes;
using casino.core.tests.Games.Poker.Players;

namespace casino.core.tests.Games.Poker.Rounds;

[TestClass]
public class RoundTests
{
    [TestMethod]
    public void Round_ShouldDealCardsOnlyToEligiblePlayers()
    {
        // Arrange
        var deck = new FakeDeck(new[]
        {
            new Card(CardRank.As, Suit.Hearts),
            new Card(CardRank.Roi, Suit.Diamonds),
            new Card(CardRank.Dame, Suit.Clubs),
            new Card(CardRank.Valet, Suit.Spades)
        });
        var activePlayer = new HumanPlayer("Alice", 100);
        var playerWithoutChips = new HumanPlayer("Bob", 0);

        // Act
        var round = new Round(new List<Player> { activePlayer, playerWithoutChips }, deck);

        // Assert
        Assert.IsNotNull(round.Players.First(j => j.Name == "Alice").Hand, "players with chips must receive cards.");
        Assert.AreEqual(new Card(CardRank.As, Suit.Hearts), activePlayer.Hand.First, "The first card must come from the provided deck.");
        Assert.AreEqual(new Card(CardRank.Roi, Suit.Diamonds), activePlayer.Hand.Second, "The second card must come from the provided deck.");
        Assert.IsNull(round.Players.First(j => j.Name == "Bob").Hand, "players without chips must not receive a hand.");
    }

    [TestMethod]
    public void AdvancePhase_PreFlopToFlop_ShouldRevealThreeCardsAndUpdateState()
    {
        // Arrange
        var deck = new FakeDeck(new[]
        {
            new Card(CardRank.As, Suit.Hearts),
            new Card(CardRank.Roi, Suit.Hearts),
            new Card(CardRank.Dame, Suit.Hearts),
            new Card(CardRank.Valet, Suit.Hearts),
            new Card(CardRank.Dix, Suit.Hearts)
        });
        var player = new HumanPlayer("Alice", 100);
        var round = new Round(new List<Player> { player }, deck);

        // Act
        round.AdvancePhase();

        // Assert
        Assert.AreEqual(Phase.Flop, round.Phase, "Phase should move to flop.");
        Assert.IsInstanceOfType<FlopPhaseState>(round.PhaseState, "State should be updated to flop state.");
        Assert.AreEqual(new Card(CardRank.Dame, Suit.Hearts), round.CommunityCards.Flop1, "First flop card must come from the deck.");
        Assert.AreEqual(new Card(CardRank.Valet, Suit.Hearts), round.CommunityCards.Flop2, "Second flop card must come from the deck.");
        Assert.AreEqual(new Card(CardRank.Dix, Suit.Hearts), round.CommunityCards.Flop3, "Third flop card must come from the deck.");
    }

    [TestMethod]
    public void EndGame_LastActivePlayer_ShouldWinByFold_AndUpdateBookkeeping()
    {
        // Arrange
        var deck = new FakeDeck(Enumerable.Repeat(new Card(CardRank.Deux, Suit.Hearts), 6));
        var activePlayer = new HumanPlayer("Alice", 100);
        var foldedPlayer = new HumanPlayer("Bob", 50) { LastAction = PokerTypeAction.Fold };
        var round = new Round(new List<Player> { activePlayer, foldedPlayer }, deck);
        round.AddToPot(50);
        round.StartingBet = 10;
        round.NumberOfRoundsPlayed = 1;

        // Act
        round.EndGame();

        // Assert
        Assert.HasCount(1, round.Winners, "There must be a single winner.");
        Assert.AreEqual(activePlayer, round.Winners.First(), "The only active player must be declared winner.");
        Assert.AreEqual(150, activePlayer.Chips, "The pot must be added to the winner's chips.");
        Assert.AreEqual(Phase.Showdown, round.Phase, "Round must end in showdown phase.");
        Assert.AreEqual(2, round.NumberOfRoundsPlayed, "Played rounds counter must be incremented after resolution.");
        Assert.AreEqual(20, round.StartingBet, "Starting bet must double when the increase threshold is reached.");
    }

    [TestMethod]
    public void EndGame_Showdown_ShouldCompareHandsAndUpdateBookkeeping()
    {
        // Arrange
        var deck = new FakeDeck(Enumerable.Repeat(new Card(CardRank.Deux, Suit.Spades), 10));
        var alice = new HumanPlayer("Alice", 100);
        var bob = new HumanPlayer("Bob", 100);
        var round = new Round(new List<Player> { alice, bob }, deck);
        round.StartingBet = 10;
        round.NumberOfRoundsPlayed = 1;
        round.SetCommunityCards(PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Dame, Suit.Hearts),
            new Card(CardRank.Dix, Suit.Hearts),
            new Card(CardRank.Neuf, Suit.Hearts),
            new Card(CardRank.Deux, Suit.Diamonds),
            new Card(CardRank.Trois, Suit.Clubs)));
        round.AddToPot(60);

        // Important: assign hands after round initialization.
        alice.Hand = new HandCards(
            new Card(CardRank.As, Suit.Hearts),
            new Card(CardRank.Roi, Suit.Hearts));
        bob.Hand = new HandCards(
            new Card(CardRank.As, Suit.Spades),
            new Card(CardRank.As, Suit.Diamonds));

        // Act
        round.EndGame();

        // Assert
        Assert.HasCount(1, round.Winners, "There must be a single winner.");
        Assert.AreEqual(alice, round.Winners.First(), "Player with the best hand must be designated winner.");
        Assert.AreEqual(160, alice.Chips, "The pot must be added to the winner's chips.");
        Assert.AreEqual(100, bob.Chips, "Other players' chips must remain unchanged.");
        Assert.AreEqual(Phase.Showdown, round.Phase, "Round must be marked as finished.");
        Assert.AreEqual(2, round.NumberOfRoundsPlayed, "Played rounds counter must be incremented after resolution.");
        Assert.AreEqual(20, round.StartingBet, "Starting bet must double when the increase threshold is reached.");
    }
    [TestMethod]
    public void Round_PublicApi_ShouldExposeReadOnlyStateMutatedViaMethodsOnly()
    {
        // Arrange
        var deck = new FakeDeck(new[]
        {
            new Card(CardRank.As, Suit.Hearts),
            new Card(CardRank.Roi, Suit.Hearts),
            new Card(CardRank.Dame, Suit.Hearts),
            new Card(CardRank.Valet, Suit.Hearts),
            new Card(CardRank.Dix, Suit.Hearts),
            new Card(CardRank.Neuf, Suit.Hearts),
            new Card(CardRank.Huit, Suit.Hearts)
        });
        var alice = new HumanPlayer("Alice", 100);
        var bob = new HumanPlayer("Bob", 100);
        var round = new Round(new List<Player> { alice, bob }, deck);

        // Assert read-only exposure
        Assert.IsFalse(round.Players is List<Player>, "players must not be exposed as mutable list.");

        // Act through sanctioned methods
        round.SetCurrentBet(20);
        round.AddToPot(20);
        round.AdvancePhase();

        // Assert state was changed through explicit API
        Assert.AreEqual(0, round.CurrentBet, "Current bet must reset when advancing phase.");
        Assert.AreEqual(20, round.Pot);
        Assert.AreEqual(Phase.Flop, round.Phase);
        Assert.IsNotNull(round.CommunityCards.Flop1);
        Assert.IsNotNull(round.CommunityCards.Flop2);
        Assert.IsNotNull(round.CommunityCards.Flop3);
    }

}
