using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Poker.Rounds;
using casino.core.tests.Fakes;

namespace casino.core.tests.Games.Poker.Players;

[TestClass]
public class DifficultyStrategyFactoryTests
{
    [TestMethod]
    public void Create_ShouldReturnAdaptiveStrategy_ForAllConfiguredDifficulties()
    {
        var levels = Enum.GetValues<PokerDifficulty>();

        foreach (var level in levels)
        {
            var strategy = DifficultyStrategyFactory.Create(level);
            Assert.IsInstanceOfType<AdaptiveStrategy>(strategy);
        }
    }

    [TestMethod]
    public void VeryHardProfile_ShouldRaiseMoreThanBeginner_WhenOnlyRaiseIsAvailable()
    {
        var player = new Player("bot", 500);
        var players = new List<Player> { player, new Player("opp", 500) };
        var round = new Round(players, new FakeDeck(CreateCards()), 0)
        {
            StartingBet = 20
        };

        // Set the controlled hand after Round dealt cards to avoid FakeDeck overwriting it
        player.Hand = new HandCards(new Card(CardRank.As, Suit.Hearts), new Card(CardRank.Roi, Suit.Spades));

        var context = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Raise });

        var beginnerStrategy = new AdaptiveStrategy(PokerAiProfile.Beginner);
        var veryHardStrategy = new AdaptiveStrategy(PokerAiProfile.VeryHard with { BluffFrequency = 0.0 });

        var beginnerAction = beginnerStrategy.DecideAction(context);
        var veryHardAction = veryHardStrategy.DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, beginnerAction.TypeAction);
        Assert.AreEqual(PokerTypeAction.Raise, veryHardAction.TypeAction);
    }

    private static IEnumerable<Card> CreateCards()
    {
        var ranks = Enum.GetValues<CardRank>();
        var suits = Enum.GetValues<Suit>();
        int count = 0;
        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                yield return new Card(rank, suit);
                if (++count >= 20)
                    yield break;
            }
        }
    }
}
