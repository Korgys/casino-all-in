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
            hand: new HandCards(new Card(CardRank.Ace, Suit.Spades), new Card(CardRank.King, Suit.Spades)));

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
            hand: new HandCards(new Card(CardRank.Two, Suit.Hearts), new Card(CardRank.Seven, Suit.Clubs)));

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
            hand: new HandCards(new Card(CardRank.Ten, Suit.Hearts), new Card(CardRank.Nine, Suit.Clubs)));

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Check, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_BluffsWithRaise_WhenBluffFrequencyIsOne()
    {
        var profile = PokerAiProfile.Medium with
        {
            PreferCheckWhenAvailable = false,
            BluffFrequency = 1.0,
            CanAllInWhenStrong = false
        };

        var context = CreateContext(new[] { PokerTypeAction.Raise, PokerTypeAction.Call });
        context.Round.SetCurrentBet(30);

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
        Assert.AreEqual(31, action.Amount);
    }

    [TestMethod]
    public void DecideAction_RaiseAmount_IsCappedByTargetContribution_WhenAggressive()
    {
        var profile = PokerAiProfile.Hard with
        {
            PreferCheckWhenAvailable = false,
            BluffFrequency = 0.0,
            FoldThresholdFactor = 0.0,
            CallThresholdFactor = 0.0,
            RaiseThresholdFactor = 0.000001,
            AllInThresholdFactor = 10.0,
            RaiseSizeMultiplier = 10.0
        };

        var context = CreateContext(
            new[] { PokerTypeAction.Raise, PokerTypeAction.Call },
            chips: 5,
            hand: new HandCards(new Card(CardRank.Ace, Suit.Hearts), new Card(CardRank.King, Suit.Hearts)));

        context.Round.SetCurrentBet(20);
        context.Round.SetBetFor(context.CurrentPlayer, 18);

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
        Assert.AreEqual(23, action.Amount);
    }

    [TestMethod]
    public void DecideAction_UsesFirstAvailableAction_WhenDefensiveOptionsAreUnavailable()
    {
        var profile = PokerAiProfile.Beginner with
        {
            PreferCheckWhenAvailable = false,
            BluffFrequency = 0.0,
            FoldThresholdFactor = 10.0,
            CallThresholdFactor = 10.0,
            RaiseThresholdFactor = 10.0,
            AllInThresholdFactor = 10.0
        };

        var context = CreateContext(new[] { PokerTypeAction.Raise, PokerTypeAction.Bet });

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_ChoosesCheck_WhenDefensiveAndFoldIsUnavailable()
    {
        var context = CreateContext(new[] { PokerTypeAction.Check, PokerTypeAction.Call });

        var action = new AdaptiveStrategy(CreateDefensiveProfile()).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Check, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_ChoosesCall_WhenDefensiveAndOnlyCallFallbackExists()
    {
        var context = CreateContext(new[] { PokerTypeAction.Call, PokerTypeAction.Raise });

        var action = new AdaptiveStrategy(CreateDefensiveProfile()).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Call, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_ChoosesCall_WhenCautiousAndCheckIsUnavailable()
    {
        var context = CreateContext(new[] { PokerTypeAction.Call, PokerTypeAction.Bet });

        var action = new AdaptiveStrategy(CreateCautiousProfile()).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Call, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_ChoosesBet_WhenCautiousAndOnlyBetFallbackExists()
    {
        var context = CreateContext(new[] { PokerTypeAction.Bet, PokerTypeAction.Raise });

        var action = new AdaptiveStrategy(CreateCautiousProfile()).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction);
        Assert.AreEqual(context.MinimumBet, action.Amount);
    }

    [TestMethod]
    public void DecideAction_ChoosesAggressiveFallback_WhenCautiousOptionsAreUnavailable()
    {
        var context = CreateContext(new[] { PokerTypeAction.Raise });

        var action = new AdaptiveStrategy(CreateCautiousProfile()).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
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
            hand: new HandCards(new Card(CardRank.Ace, Suit.Hearts), new Card(CardRank.King, Suit.Diamonds)));

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_BluffsWithBet_WhenRaiseIsUnavailable()
    {
        var profile = PokerAiProfile.Medium with
        {
            PreferCheckWhenAvailable = false,
            BluffFrequency = 1.0,
            CanAllInWhenStrong = false
        };

        var context = CreateContext(new[] { PokerTypeAction.Bet, PokerTypeAction.Call });

        var action = new AdaptiveStrategy(profile).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction);
        Assert.AreEqual(context.MinimumBet, action.Amount);
    }

    [TestMethod]
    public void DecideAction_ChoosesBet_WhenAggressiveAndRaiseIsUnavailable()
    {
        var context = CreateContext(new[] { PokerTypeAction.Bet, PokerTypeAction.Call });

        var action = new AdaptiveStrategy(CreateAggressiveProfile()).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction);
        Assert.AreEqual(context.MinimumBet, action.Amount);
    }

    [TestMethod]
    public void DecideAction_ChoosesCall_WhenAggressiveAndOnlyCallFallbackExists()
    {
        var context = CreateContext(new[] { PokerTypeAction.Call, PokerTypeAction.Check });

        var action = new AdaptiveStrategy(CreateAggressiveProfile(bluffFrequency: 1.0)).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Call, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_ChoosesCheck_WhenAggressiveAndOnlyCheckFallbackExists()
    {
        var context = CreateContext(new[] { PokerTypeAction.Check, PokerTypeAction.Fold });

        var action = new AdaptiveStrategy(CreateAggressiveProfile()).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.Check, action.TypeAction);
    }

    [TestMethod]
    public void DecideAction_UsesFirstAvailableAction_WhenAggressiveFallbacksAreUnavailable()
    {
        var context = CreateContext(new[] { PokerTypeAction.AllIn });

        var action = new AdaptiveStrategy(CreateAggressiveProfile(canAllInWhenStrong: false)).DecideAction(context);

        Assert.AreEqual(PokerTypeAction.AllIn, action.TypeAction);
    }

    private static PokerAiProfile CreateDefensiveProfile()
    {
        return PokerAiProfile.Beginner with
        {
            PreferCheckWhenAvailable = false,
            BluffFrequency = 0.0,
            FoldThresholdFactor = 10.0,
            CallThresholdFactor = 10.0,
            RaiseThresholdFactor = 10.0,
            AllInThresholdFactor = 10.0
        };
    }

    private static PokerAiProfile CreateCautiousProfile()
    {
        return PokerAiProfile.Beginner with
        {
            PreferCheckWhenAvailable = false,
            BluffFrequency = 0.0,
            FoldThresholdFactor = 0.0,
            CallThresholdFactor = 0.0,
            RaiseThresholdFactor = 10.0,
            AllInThresholdFactor = 10.0
        };
    }

    private static PokerAiProfile CreateAggressiveProfile(double bluffFrequency = 0.0, bool canAllInWhenStrong = true)
    {
        return PokerAiProfile.Beginner with
        {
            PreferCheckWhenAvailable = false,
            BluffFrequency = bluffFrequency,
            FoldThresholdFactor = 0.0,
            CallThresholdFactor = 0.0,
            RaiseThresholdFactor = 0.0,
            AllInThresholdFactor = 10.0,
            CanAllInWhenStrong = canAllInWhenStrong
        };
    }

    private static GameContext CreateContext(
        IReadOnlyList<PokerTypeAction> availableActions,
        HandCards? hand = null,
        int chips = 200)
    {
        var player = new ComputerPlayer("Bot", chips);
        var opponent = new ComputerPlayer("Opp", 200);
        var round = new Round(new List<Player> { player, opponent }, new FakeDeck(CreateDeckCards()), 0)
        {
            StartingBet = 10
        };

        player.Hand = hand ?? new HandCards(
            new Card(CardRank.Ace, Suit.Hearts),
            new Card(CardRank.King, Suit.Spades));

        return new GameContext(round, player, availableActions);
    }

    private static IEnumerable<Card> CreateDeckCards()
    {
        yield return new Card(CardRank.Two, Suit.Hearts);
        yield return new Card(CardRank.Three, Suit.Hearts);
        yield return new Card(CardRank.Four, Suit.Hearts);
        yield return new Card(CardRank.Five, Suit.Hearts);
        yield return new Card(CardRank.Six, Suit.Hearts);
        yield return new Card(CardRank.Seven, Suit.Hearts);
        yield return new Card(CardRank.Eight, Suit.Hearts);
        yield return new Card(CardRank.Nine, Suit.Hearts);
        yield return new Card(CardRank.Ten, Suit.Hearts);
        yield return new Card(CardRank.Jack, Suit.Hearts);
    }

    private sealed class FakeDeck(IEnumerable<Card> cards) : IDeck
    {
        private readonly Queue<Card> cards = new(cards);

        public Card DrawCard() => this.cards.Count > 0 ? this.cards.Dequeue() : new Card(CardRank.Two, Suit.Diamonds);

        public void Shuffle()
        {
        }
    }
}
