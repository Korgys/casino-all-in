using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;

namespace casino.core.tests.Games.Poker.Players.StrategyTests.cs;

[TestClass]
public class AggressiveStrategyTests
{
    [TestMethod]
    public void DecideAction_ShouldPreferRaiseWhenPossible()
    {
        var player = new ComputerPlayer("Bot", 50, new AggressiveStrategy())
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.Roi, Suit.Spades))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, startingBet: 15);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Raise, PokerTypeAction.Bet, PokerTypeAction.Call });
        var strategy = new AggressiveStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
        Assert.AreEqual(context.MinimumBet, action.Amount);
    }

    [TestMethod]
    public void DecideAction_WhenRaiseIsNotPossibleButBetIsAvailable_ShouldBet()
    {
        var player = new ComputerPlayer("Bot", 10, new AggressiveStrategy())
        {
            Hand = new HandCards(
                new Card(CardRank.Valet, Suit.Diamonds),
                new Card(CardRank.Dix, Suit.Clubs))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand);
        PlayerTestHelper.SetCurrentBet(round, 20);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Bet, PokerTypeAction.Call });
        var strategy = new AggressiveStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction);
        Assert.AreEqual(context.MinimumBet, action.Amount);
    }

    [TestMethod]
    public void DecideAction_WhenOnlyCheckIsAvailable_ShouldCheck()
    {
        var player = new ComputerPlayer("Bot", 10, new AggressiveStrategy())
        {
            Hand = new HandCards(
                new Card(CardRank.Quatre, Suit.Diamonds),
                new Card(CardRank.Cinq, Suit.Clubs))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand);
        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Check });
        var strategy = new AggressiveStrategy();

        var action = strategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Check, action.TypeAction);
    }
}
