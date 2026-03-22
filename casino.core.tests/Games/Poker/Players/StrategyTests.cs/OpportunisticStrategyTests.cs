using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Poker.Rounds;
using casino.core.tests.Fakes;
using casino.core.tests.Games.Poker.Players;
using System.Linq;

namespace casino.core.tests.Games.Poker.Players.Strategies;

[TestClass]
public class OpportunisticStrategyTests
{
    [TestMethod]
    public void DecideAction_ShouldAllInWhenOddsAreGood()
    {
        var player = new Player("bob", 1000);
        var players = new List<Player>
        {
            player,
            new Player("alice", 1000)
        };
        var round = new Round(players, new FakeDeck(CreateSimpleCards()));
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.AllIn });
        var strategy = new OpportunisticStrategy();

        var action = strategy.DecideAction(context);

        Assert.IsNotNull(action);
        Assert.AreEqual(PokerTypeAction.AllIn, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_ShouldBetWithMinimumBet()
    {
        var player = new Player("bob", 1000);
        var players = new List<Player>
        {
            player,
            new Player("alice", 1000)
        };
        var round = new Round(players, new FakeDeck(CreateSimpleCards()));
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Bet });
        var strategy = new OpportunisticStrategy();

        var action = strategy.DecideAction(context);

        Assert.IsNotNull(action);
        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction);
        Assert.AreEqual(10, action.Amount);
    }

    [TestMethod]
    public void DecideAction_ShouldRaiseWhenMostOpponentsAreFolded()
    {
        var player = new Player("bob", 1000);
        var activeOpponent = new Player("alice", 1000);
        var foldedOne = new Player("charlie", 1000) { LastAction = PokerTypeAction.Fold };
        var foldedTwo = new Player("diana", 1000) { LastAction = PokerTypeAction.Fold };
        var foldedThree = new Player("eric", 1000) { LastAction = PokerTypeAction.Fold };

        var players = new List<Player>
        {
            player,
            activeOpponent,
            foldedOne,
            foldedTwo,
            foldedThree
        };

        var cards = new List<Card>
        {
            new(CardRank.As, Suit.Spades),
            new(CardRank.As, Suit.Hearts),
            new(CardRank.Deux, Suit.Clubs),
            new(CardRank.Trois, Suit.Diamonds),
            new(CardRank.Quatre, Suit.Spades),
            new(CardRank.Cinq, Suit.Hearts),
            new(CardRank.Six, Suit.Clubs),
            new(CardRank.Sept, Suit.Diamonds),
            new(CardRank.Huit, Suit.Spades),
            new(CardRank.Neuf, Suit.Hearts)
        };

        var round = new Round(players, new FakeDeck(cards));
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Fold, PokerTypeAction.Call, PokerTypeAction.Raise });
        var strategy = new OpportunisticStrategy();

        var action = strategy.DecideAction(context);

        Assert.IsNotNull(action);
        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
        Assert.AreEqual(context.MinimumBet, action.Amount);
    }

    [TestMethod]
    public void DecideAction_WithRaiseAvailableAndCurrentBet_ShouldRaiseAboveCurrentBet()
    {
        var player = new Player("bob", 1000);
        var players = new List<Player>
        {
            player,
            new Player("alice", 1000)
        };
        var round = new Round(players, new FakeDeck(CreateSimpleCards()));
        PlayerTestHelper.SetCurrentBet(round, 14);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Raise });
        var strategy = new OpportunisticStrategy();

        var action = strategy.DecideAction(context);

        Assert.IsNotNull(action);
        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
        Assert.AreEqual(15, action.Amount);
    }

    private static IEnumerable<Card> CreateSimpleCards() => Enumerable.Repeat(new Card(CardRank.Deux, Suit.Hearts), 10);
}
