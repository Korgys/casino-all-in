using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;

namespace casino.core.tests.Games.Poker.Players.StrategyTests.cs;

[TestClass]
public class RandomStrategyTests
{
    [TestMethod]
    public void DecideAction_WhenMinimumRaiseExceedsRemainingChips_ShouldUseAllChips()
    {
        var player = new HumanPlayer("Alice", 8)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.Roi, Suit.Spades))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand);
        PlayerTestHelper.SetCurrentBet(round, 5);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Raise });
        var strategy = new RandomStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
        Assert.AreEqual(player.Chips, action.Amount);
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
}
