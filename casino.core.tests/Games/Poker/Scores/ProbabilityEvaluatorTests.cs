using casino.core.Common.Utils;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Scores;

[TestClass]
public class ProbabilityEvaluatorTests
{
    [TestMethod]
    public void EstimateWinProbability_Throws_WhenMainPlayerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ProbabilityEvaluator.EstimateWinProbability(null!, new TableCards(), numberOfOpponents: 1));
    }

    [TestMethod]
    public void EstimateWinProbability_Throws_WhenCommunityCardsIsNull()
    {
        var hand = new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts));

        Assert.Throws<ArgumentNullException>(() =>
            ProbabilityEvaluator.EstimateWinProbability(hand, null!, numberOfOpponents: 1));
    }

    [TestMethod]
    public void EstimateWinProbability_Throws_WhenNumberOfOpponentsIsNegative()
    {
        var hand = new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ProbabilityEvaluator.EstimateWinProbability(hand, new TableCards(), numberOfOpponents: -1));
    }

    [TestMethod]
    public void EstimateWinProbability_Throws_WhenSimulationsIsLessThanOne()
    {
        var hand = new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ProbabilityEvaluator.EstimateWinProbability(hand, new TableCards(), numberOfOpponents: 1, simulations: 0));
    }

    [TestMethod]
    public void EstimateWinProbability_Returns100_WhenNoOpponent()
    {
        var hand = new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts));

        var probability = ProbabilityEvaluator.EstimateWinProbability(hand, new TableCards(), numberOfOpponents: 0);

        Assert.AreEqual(100d, probability, 0.00001d);
    }

    [TestMethod]
    public void EstimateWinProbability_Throws_WhenKnownCardsContainDuplicates()
    {
        var duplicate = new Card(CardRank.As, Suit.Spades);
        var hand = new HandCards(duplicate, new Card(CardRank.Roi, Suit.Hearts));
        var board = new TableCards { Flop1 = duplicate };

        Assert.Throws<ArgumentException>(() =>
            ProbabilityEvaluator.EstimateWinProbability(hand, board, numberOfOpponents: 1));
    }

    [TestMethod]
    public void EstimateWinProbability_Throws_WhenNotEnoughCardsToDeal()
    {
        var hand = new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts));
        var board = new TableCards
        {
            Flop1 = new Card(CardRank.Deux, Suit.Clubs),
            Flop2 = new Card(CardRank.Trois, Suit.Clubs),
            Flop3 = new Card(CardRank.Quatre, Suit.Clubs),
            Turn = new Card(CardRank.Cinq, Suit.Clubs),
            River = new Card(CardRank.Six, Suit.Clubs)
        };

        Assert.Throws<ArgumentException>(() =>
            ProbabilityEvaluator.EstimateWinProbability(hand, board, numberOfOpponents: 23, simulations: 10));
    }

    [TestMethod]
    public void EstimateWinProbability_WithDeterministicRandom_CanReturnZero_WhenOpponentAlwaysBetter()
    {
        var hand = new HandCards(new Card(CardRank.Trois, Suit.Diamonds), new Card(CardRank.Quatre, Suit.Diamonds));
        var board = new TableCards
        {
            Flop1 = new Card(CardRank.As, Suit.Spades),
            Flop2 = new Card(CardRank.As, Suit.Diamonds),
            Flop3 = new Card(CardRank.Roi, Suit.Clubs),
            Turn = new Card(CardRank.Sept, Suit.Clubs),
            River = new Card(CardRank.Deux, Suit.Clubs)
        };

        var probability = ProbabilityEvaluator.EstimateWinProbability(
            hand,
            board,
            numberOfOpponents: 1,
            simulations: 20,
            random: new MinValueRandom());

        Assert.AreEqual(0d, probability, 0.00001d);
    }

    [TestMethod]
    public void EstimateWinProbability_WithDeterministicRandom_CanReturnHundred_WhenMainPlayerAlwaysBetter()
    {
        var hand = new HandCards(new Card(CardRank.As, Suit.Hearts), new Card(CardRank.As, Suit.Clubs));
        var board = new TableCards
        {
            Flop1 = new Card(CardRank.Roi, Suit.Diamonds),
            Flop2 = new Card(CardRank.Dame, Suit.Diamonds),
            Flop3 = new Card(CardRank.Neuf, Suit.Clubs),
            Turn = new Card(CardRank.Trois, Suit.Spades),
            River = new Card(CardRank.Cinq, Suit.Hearts)
        };

        var probability = ProbabilityEvaluator.EstimateWinProbability(
            hand,
            board,
            numberOfOpponents: 1,
            simulations: 20,
            random: new MinValueRandom());

        Assert.AreEqual(100d, probability, 0.00001d);
    }

    [TestMethod]
    public void EstimateWinProbability_ReturnsSplitShare_WhenBoardForcesTie()
    {
        var hand = new HandCards(new Card(CardRank.Deux, Suit.Hearts), new Card(CardRank.Trois, Suit.Hearts));
        var board = new TableCards
        {
            Flop1 = new Card(CardRank.Dix, Suit.Spades),
            Flop2 = new Card(CardRank.Valet, Suit.Spades),
            Flop3 = new Card(CardRank.Dame, Suit.Spades),
            Turn = new Card(CardRank.Roi, Suit.Spades),
            River = new Card(CardRank.As, Suit.Spades)
        };

        var probability = ProbabilityEvaluator.EstimateWinProbability(
            hand,
            board,
            numberOfOpponents: 1,
            simulations: 30,
            random: new MinValueRandom());

        Assert.AreEqual(50d, probability, 0.00001d);
    }

    [TestMethod]
    public void EstimateWinProbability_UsesProvidedRandom_InSequentialMode()
    {
        var hand = new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts));
        var board = new TableCards();
        var random = new CountingRandom();

        var probability = ProbabilityEvaluator.EstimateWinProbability(
            hand,
            board,
            numberOfOpponents: 2,
            simulations: 15,
            random: random);

        Assert.IsTrue(probability >= 0d && probability <= 100d);
        Assert.IsGreaterThan(0, random.NextCalls, "The provided randomizer should be consumed during simulation.");
    }

    [TestMethod]
    public void EstimateWinProbability_ParallelBranch_ReturnsValueInRange()
    {
        var hand = new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts));
        var board = new TableCards();

        var probability = ProbabilityEvaluator.EstimateWinProbability(
            hand,
            board,
            numberOfOpponents: 3,
            simulations: 1000);

        Assert.IsTrue(probability >= 0d && probability <= 100d);
    }

    private sealed class MinValueRandom : IRandom
    {
        public int Next(int maxExclusive) => 0;
        public int Next(int minInclusive, int maxExclusive) => minInclusive;
    }

    private sealed class CountingRandom : IRandom
    {
        public int NextCalls { get; private set; }

        public int Next(int maxExclusive)
        {
            NextCalls++;
            return 0;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            NextCalls++;
            return minInclusive;
        }
    }
}
