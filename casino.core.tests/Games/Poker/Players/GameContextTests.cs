using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Players;

[TestClass]
public class GameContextTests
{
    [TestMethod]
    public void Constructor_ShouldCalculateScoreAndKeepAvailableActions()
    {
        var player = new HumanPlayer("Alice", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Ace, Suit.Hearts),
                new Card(CardRank.King, Suit.Diamonds))
        };
        var communityCards = PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Ace, Suit.Spades),
            new Card(CardRank.Queen, Suit.Clubs),
            new Card(CardRank.Nine, Suit.Diamonds));
        var availableActions = new List<PokerTypeAction> { PokerTypeAction.Check, PokerTypeAction.Bet };

        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand, communityCards);

        var context = new GameContext(round, player, availableActions);

        Assert.AreEqual(HandRank.OnePair, context.PlayerScore.Rank);
        Assert.AreEqual(CardRank.Ace, context.PlayerScore.CardValue);
        CollectionAssert.AreEquivalent(availableActions, context.AvailableActions.ToList());
        Assert.AreEqual(round.StartingBet, context.MinimumBet);
    }

    [TestMethod]
    public void MinimumBet_ShouldUseCurrentBetWhenItIsHigherThanStartingBet()
    {
        var player = new HumanPlayer("Bob", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Ten, Suit.Hearts),
                new Card(CardRank.Nine, Suit.Diamonds))
        };
        var round = PlayerTestHelper.CreateRoundWithPlayer(player, player.Hand);
        PlayerTestHelper.SetCurrentBet(round, 45);

        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Bet });

        Assert.AreEqual(45, context.MinimumBet);
    }
}
