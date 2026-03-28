using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;

namespace casino.core.tests.Games.Poker.Players.StrategyTests.cs;

[TestClass]
public class ConservativeStrategyTests
{
    [TestMethod]
    public void DecideAction_WhenCheckIsAvailable_ShouldReturnCheck()
    {
        var player = new HumanPlayer("Alice", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.Dix, Suit.Diamonds))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Check, PokerTypeAction.Call });
        var strategy = new ConservativeStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Check, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_WhenCallIsAvailableWithStrongEnoughHand_ShouldCall()
    {
        var player = new HumanPlayer("Bob", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.As, Suit.Diamonds))
        };
        var communityCards = PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Roi, Suit.Spades),
            new Card(CardRank.Roi, Suit.Clubs),
            new Card(CardRank.Dame, Suit.Hearts));
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, communityCards);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Call, PokerTypeAction.Bet });
        var strategy = new ConservativeStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Call, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_WhenBetIsAvailableWithPair_ShouldBetMinimum()
    {
        var player = new HumanPlayer("Cara", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.As, Suit.Spades))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, startingBet: 15);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Bet, PokerTypeAction.Fold });
        var strategy = new ConservativeStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction);
        Assert.AreEqual(15, action.Amount);
    }

    [TestMethod]
    public void DecideAction_WhenRaiseIsAvailableWithFullHouse_ShouldRaiseMinimum()
    {
        var player = new HumanPlayer("Dana", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.As, Suit.Spades))
        };
        var communityCards = PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Roi, Suit.Hearts),
            new Card(CardRank.Roi, Suit.Diamonds),
            new Card(CardRank.Roi, Suit.Clubs));
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, communityCards, startingBet: 20);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Raise, PokerTypeAction.Call, PokerTypeAction.Fold });
        var strategy = new ConservativeStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Call, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_WhenNoOtherActionFits_ShouldFold()
    {
        var player = new HumanPlayer("Claire", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Deux, Suit.Diamonds),
                new Card(CardRank.Sept, Suit.Clubs))
        };
        var communityCards = PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Dix, Suit.Spades),
            new Card(CardRank.Neuf, Suit.Clubs),
            new Card(CardRank.Huit, Suit.Hearts));
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, communityCards);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Fold });
        var strategy = new ConservativeStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Fold, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_WhenBetIsFirstActionAndNoPriorityRuleMatches_ShouldUseBetFallback()
    {
        var player = new HumanPlayer("Eve", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Deux, Suit.Diamonds),
                new Card(CardRank.Sept, Suit.Clubs))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, startingBet: 30);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Bet, PokerTypeAction.AllIn });
        var strategy = new ConservativeStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction);
        Assert.AreEqual(context.MinimumBet, action.Amount);
    }

    [TestMethod]
    public void DecideAction_WhenNoPriorityRuleMatchesAndFirstActionIsNotBet_ShouldUseFinalFallback()
    {
        var player = new HumanPlayer("Finn", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Deux, Suit.Hearts),
                new Card(CardRank.Neuf, Suit.Spades))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.AllIn, PokerTypeAction.Raise });
        var strategy = new ConservativeStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.AllIn, action.TypeAction);
        Assert.AreEqual(0, action.Amount);
    }
}
