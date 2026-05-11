using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;

namespace casino.core.tests.Games.Poker.Players.StrategyTests.cs;

[TestClass]
public class RandomStrategyTests
{
    [TestMethod]
    public void DecideAction_WhenMinimumRaiseExceedsRemainingChips_ShouldUseAllChipsAsTargetContribution()
    {
        var player = new HumanPlayer("Alice", 8)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.Roi, Suit.Spades))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand);
        PlayerTestHelper.SetCurrentBet(round, 11);
        round.SetBetFor(player, 4);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Raise });
        var strategy = new RandomStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
        Assert.AreEqual(12, action.Amount);
    }

    [TestMethod]
    public void DecideAction_WhenBetIsChosen_ShouldUseMinimumBet()
    {
        var player = new HumanPlayer("Bob", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Dame, Suit.Diamonds),
                new Card(CardRank.Neuf, Suit.Clubs))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, startingBet: 25);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Bet });
        var strategy = new RandomStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction);
        Assert.AreEqual(context.MinimumBet, action.Amount);
    }

    [TestMethod]
    public void DecideAction_WhenRaiseIsChosen_ShouldStayWithinRaiseBounds()
    {
        var player = new HumanPlayer("Cara", 120)
        {
            Hand = new HandCards(
                new Card(CardRank.Dix, Suit.Hearts),
                new Card(CardRank.Huit, Suit.Spades))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, startingBet: 10);
        PlayerTestHelper.SetCurrentBet(round, 15);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Raise });
        var strategy = new RandomStrategy();

        var minimum = Math.Max(context.Round.CurrentBet + 1, context.MinimumBet);
        var maximum = Math.Max(minimum, Math.Min(round.GetBetFor(player) + context.CurrentPlayer.Chips, context.Round.CurrentBet + context.Round.StartingBet * 3));

        for (var i = 0; i < 50; i++)
        {
            var action = strategy.DecideAction(context);
            Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
            Assert.IsGreaterThanOrEqualTo(minimum, action.Amount);
            Assert.IsLessThanOrEqualTo(maximum, action.Amount);
        }
    }

    [TestMethod]
    public void DecideAction_WhenMinimumRaiseEqualsAllChips_ShouldReturnTargetContribution()
    {
        var player = new HumanPlayer("Dana", 6)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.Roi, Suit.Spades))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, startingBet: 1);
        PlayerTestHelper.SetCurrentBet(round, 5);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Raise });
        var strategy = new RandomStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
        Assert.AreEqual(player.Chips, action.Amount);
    }
}
