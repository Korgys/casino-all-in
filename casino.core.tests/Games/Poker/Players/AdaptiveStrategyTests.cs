using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Poker.Rounds;
using casino.core.tests.Fakes;

namespace casino.core.tests.Games.Poker.Players;

[TestClass]
public class AdaptiveStrategyTests
{
    [TestMethod]
    public void DecideAction_PrefersCheck_WhenProfileRequestsIt()
    {
        var context = CreateContext(
            new[] { PokerTypeAction.Check, PokerTypeAction.Call, PokerTypeAction.Raise });

        var action = new AdaptiveStrategy(PokerAiProfile.Beginner).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Check, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_CanReturnAllIn_WhenStrongEnoughAndAllowed()
    {
        var profile = PokerAiProfile.Medium with
        {
            FoldThresholdFactor = 0.0,
            CallThresholdFactor = 0.0,
            RaiseThresholdFactor = 0.0,
            AllInThresholdFactor = 0.0,
            PreferCheckWhenAvailable = false,
            CanAllInWhenStrong = true,
            BluffFrequency = 0.0
        };

        var context = CreateContext(
            new[] { PokerTypeAction.AllIn, PokerTypeAction.Raise, PokerTypeAction.Call },
            hand: new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Spades)));

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.AllIn, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_FallsBackToDefensiveAction_WhenProbabilityIsVeryLow()
    {
        var profile = PokerAiProfile.Beginner with
        {
            FoldThresholdFactor = 10.0,
            CallThresholdFactor = 10.0,
            RaiseThresholdFactor = 10.0,
            AllInThresholdFactor = 10.0,
            PreferCheckWhenAvailable = false,
            BluffFrequency = 0.0
        };

        var context = CreateContext(
            new[] { PokerTypeAction.Fold, PokerTypeAction.Call, PokerTypeAction.Raise },
            hand: new HandCards(new Card(CardRank.Deux, Suit.Hearts), new Card(CardRank.Sept, Suit.Clubs)));

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Fold, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_ChoosesCautiousAction_WhenProbabilityIsModerate()
    {
        var profile = PokerAiProfile.Beginner with
        {
            FoldThresholdFactor = 0.0,
            CallThresholdFactor = 0.0,
            RaiseThresholdFactor = 10.0,
            AllInThresholdFactor = 10.0,
            PreferCheckWhenAvailable = false,
            BluffFrequency = 0.0
        };

        var context = CreateContext(
            new[] { PokerTypeAction.Check, PokerTypeAction.Call, PokerTypeAction.Bet },
            hand: new HandCards(new Card(CardRank.Dix, Suit.Hearts), new Card(CardRank.Neuf, Suit.Clubs)));

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Check, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_ChoosesAggressiveAction_WhenProbabilityIsAboveRaiseThreshold()
    {
        var profile = PokerAiProfile.Beginner with
        {
            FoldThresholdFactor = 0.0,
            CallThresholdFactor = 0.0,
            RaiseThresholdFactor = 0.000001,
            AllInThresholdFactor = 10.0,
            PreferCheckWhenAvailable = false,
            BluffFrequency = 0.0,
            RaiseSizeMultiplier = 2.0
        };

        var context = CreateContext(
            new[] { PokerTypeAction.Raise, PokerTypeAction.Call },
            hand: new HandCards(new Card(CardRank.As, Suit.Hearts), new Card(CardRank.Roi, Suit.Diamonds)));

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
    }

    private static GameContext CreateContext(
        IReadOnlyList<PokerTypeAction> availableActions,
        HandCards? hand = null)
    {
        var player = new ComputerPlayer("Bot", 200);
        var opponent = new ComputerPlayer("Opp", 200);
        var round = new Round(new List<Player> { player, opponent }, new FakeDeck(CreateDeckCards()), 0)
        {
            StartingBet = 10
        };

        player.Hand = hand ?? new HandCards(
            new Card(CardRank.As, Suit.Hearts),
            new Card(CardRank.Roi, Suit.Spades));

        return new GameContext(round, player, availableActions);
    }

    private static IEnumerable<Card> CreateDeckCards()
    {
        yield return new Card(CardRank.Deux, Suit.Hearts);
        yield return new Card(CardRank.Trois, Suit.Hearts);
        yield return new Card(CardRank.Quatre, Suit.Hearts);
        yield return new Card(CardRank.Cinq, Suit.Hearts);
        yield return new Card(CardRank.Six, Suit.Hearts);
        yield return new Card(CardRank.Sept, Suit.Hearts);
        yield return new Card(CardRank.Huit, Suit.Hearts);
        yield return new Card(CardRank.Neuf, Suit.Hearts);
        yield return new Card(CardRank.Dix, Suit.Hearts);
        yield return new Card(CardRank.Valet, Suit.Hearts);
    }

    private sealed class FakeDeck(IEnumerable<Card> cards) : IDeck
    {
        private readonly Queue<Card> cards = new(cards);

        public Card DrawCard() => this.cards.Count > 0 ? this.cards.Dequeue() : new Card(CardRank.Deux, Suit.Diamonds);

        public void Shuffle()
        {
        }
    }
}
