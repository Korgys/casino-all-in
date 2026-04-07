using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.tests.Fakes;
using casino.core.tests.Games.Poker.Players;

namespace casino.core.tests.Games.Poker.Rounds;

[TestClass]
public class SidePotResolutionTests
{
    [TestMethod]
    public void EndGame_SingleAllInAgainstDeeperStacks_ShouldResolveMainAndSidePotIndependently()
    {
        var deck = new FakeDeck(Enumerable.Repeat(new Card(CardRank.Deux, Suit.Spades), 10));
        var alice = new HumanPlayer("Alice", 0) { LastAction = PokerTypeAction.AllIn };
        var bob = new HumanPlayer("Bob", 100) { LastAction = PokerTypeAction.Call };
        var charlie = new HumanPlayer("Charlie", 100) { LastAction = PokerTypeAction.Call };
        var round = new Round(new List<Player> { alice, bob, charlie }, deck, 0);

        round.SetCommunityCards(PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Deux, Suit.Hearts),
            new Card(CardRank.Sept, Suit.Diamonds),
            new Card(CardRank.Neuf, Suit.Clubs),
            new Card(CardRank.Valet, Suit.Spades),
            new Card(CardRank.Dame, Suit.Hearts)));

        alice.Hand = new HandCards(new Card(CardRank.As, Suit.Hearts), new Card(CardRank.As, Suit.Diamonds));
        bob.Hand = new HandCards(new Card(CardRank.Roi, Suit.Clubs), new Card(CardRank.Dame, Suit.Diamonds));
        charlie.Hand = new HandCards(new Card(CardRank.Dix, Suit.Clubs), new Card(CardRank.Dix, Suit.Hearts));

        round.AddToPot(alice, 100);
        round.AddToPot(bob, 200);
        round.AddToPot(charlie, 200);

        round.EndGame();

        Assert.AreEqual(300, alice.Chips, "Alice should win the main pot only.");
        Assert.AreEqual(300, bob.Chips, "Bob should win the side pot.");
        Assert.AreEqual(100, charlie.Chips, "Charlie should not win any pot.");
    }

    [TestMethod]
    public void EndGame_MultipleAllInsAtDifferentAmounts_ShouldBuildThreePots()
    {
        var deck = new FakeDeck(Enumerable.Repeat(new Card(CardRank.Trois, Suit.Clubs), 10));
        var alice = new HumanPlayer("Alice", 0) { LastAction = PokerTypeAction.AllIn };
        var bob = new HumanPlayer("Bob", 0) { LastAction = PokerTypeAction.AllIn };
        var charlie = new HumanPlayer("Charlie", 0) { LastAction = PokerTypeAction.AllIn };
        var diana = new HumanPlayer("Diana", 0) { LastAction = PokerTypeAction.AllIn };
        var round = new Round(new List<Player> { alice, bob, charlie, diana }, deck, 0);

        round.SetCommunityCards(PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Deux, Suit.Hearts),
            new Card(CardRank.Cinq, Suit.Diamonds),
            new Card(CardRank.Neuf, Suit.Clubs),
            new Card(CardRank.Valet, Suit.Spades),
            new Card(CardRank.Dame, Suit.Hearts)));

        alice.Hand = new HandCards(new Card(CardRank.Trois, Suit.Spades), new Card(CardRank.Quatre, Suit.Spades));
        bob.Hand = new HandCards(new Card(CardRank.As, Suit.Hearts), new Card(CardRank.As, Suit.Diamonds));
        charlie.Hand = new HandCards(new Card(CardRank.Roi, Suit.Hearts), new Card(CardRank.Roi, Suit.Diamonds));
        diana.Hand = new HandCards(new Card(CardRank.Dix, Suit.Clubs), new Card(CardRank.Dix, Suit.Hearts));

        round.AddToPot(alice, 50);
        round.AddToPot(bob, 120);
        round.AddToPot(charlie, 200);
        round.AddToPot(diana, 200);

        round.EndGame();

        Assert.AreEqual(0, alice.Chips, "Alice should lose all pots.");
        Assert.AreEqual(410, bob.Chips, "Bob should win the main pot.");
        Assert.AreEqual(160, charlie.Chips, "Charlie should win both side pots.");
        Assert.AreEqual(0, diana.Chips, "Diana should lose all pots.");
    }

    [TestMethod]
    public void EndGame_TieInSingleSidePot_ShouldSplitThatPotWithDeterministicRemainder()
    {
        var deck = new FakeDeck(Enumerable.Repeat(new Card(CardRank.Quatre, Suit.Clubs), 10));
        var alice = new HumanPlayer("Alice", 0) { LastAction = PokerTypeAction.AllIn };
        var bob = new HumanPlayer("Bob", 0) { LastAction = PokerTypeAction.AllIn };
        var charlie = new HumanPlayer("Charlie", 0) { LastAction = PokerTypeAction.AllIn };
        var round = new Round(new List<Player> { alice, bob, charlie }, deck, 0);

        round.SetCommunityCards(PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Roi, Suit.Clubs),
            new Card(CardRank.Roi, Suit.Diamonds),
            new Card(CardRank.Huit, Suit.Hearts),
            new Card(CardRank.Huit, Suit.Clubs),
            new Card(CardRank.Deux, Suit.Spades)));

        alice.Hand = new HandCards(new Card(CardRank.Deux, Suit.Hearts), new Card(CardRank.Deux, Suit.Diamonds));
        bob.Hand = new HandCards(new Card(CardRank.As, Suit.Hearts), new Card(CardRank.Dame, Suit.Hearts));
        charlie.Hand = new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Valet, Suit.Spades));

        round.AddToPot(alice, 100);
        round.AddToPot(bob, 200);
        round.AddToPot(charlie, 201);

        round.EndGame();

        Assert.AreEqual(300, alice.Chips, "Alice should win the main pot with the best hand.");
        Assert.AreEqual(100, bob.Chips, "Bob should receive the deterministic remainder chip from the tied side pot.");
        Assert.AreEqual(101, charlie.Chips, "Charlie should receive the non-remainder part of the tied side pot.");
    }
}
