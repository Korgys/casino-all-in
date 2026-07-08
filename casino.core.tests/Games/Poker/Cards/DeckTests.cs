using casino.core.Common.Utils;
using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class DeckTests
{
    [TestMethod]
    public void Shuffle_ShouldCreate52UniqueCards()
    {
        // Arrange
        var deck = new Deck(new FakeRandomAlwaysZero());

        // Act
        deck.Shuffle();

        // Assert
        Assert.AreEqual(52, deck.RemainingCards);

        var cards = DrawAll(deck);
        Assert.HasCount(52, cards);

        // Verify uniqueness by rank and suit.
        var uniques = cards
            .Select(c => (c.Rank, c.Suit))
            .Distinct()
            .Count();

        Assert.AreEqual(52, uniques, "The deck should contain 52 unique cards.");
    }

    [TestMethod]
    public void DrawCard_ShouldDecreaseRemainingCardCount()
    {
        // Arrange
        var deck = new Deck(new FakeRandomAlwaysZero());
        deck.Shuffle();
        int before = deck.RemainingCards;

        // Act
        var card = deck.DrawCard();

        // Assert
        Assert.IsNotNull(card);
        Assert.AreEqual(before - 1, deck.RemainingCards);
    }

    [TestMethod]
    public void DrawCard_WhenDeckIsEmpty_ShouldThrowException()
    {
        // Arrange
        var deck = new Deck(new FakeRandomAlwaysZero());

        // Empty the deck.
        DrawAll(deck);

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => deck.DrawCard());
    }

    [TestMethod]
    public void Shuffle_WithDeterministicRandom_ShouldProduceDeterministicOrder()
    {
        // Arrange
        // This fake returns a controlled sequence, yielding a stable order across runs.
        var fakeRandom = new FakeRandomSequence(new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        var deck1 = new Deck(fakeRandom);
        var deck2 = new Deck(new FakeRandomSequence(Enumerable.Repeat(0, 51).ToArray()));

        // Act
        var seq1 = DrawAll(deck1).Select(c => (c.Rank, c.Suit)).ToList();
        var seq2 = DrawAll(deck2).Select(c => (c.Rank, c.Suit)).ToList();

        // Assert
        CollectionAssert.AreEqual(seq1, seq2, "The same deterministic random sequence should produce the same order.");
    }

    // --------------------
    // Helpers
    // --------------------

    private static List<Card> DrawAll(Deck deck)
    {
        var cards = new List<Card>();
        while (deck.RemainingCards > 0)
        {
            cards.Add(deck.DrawCard());
        }
        return cards;
    }

    private sealed class FakeRandomAlwaysZero : IRandom
    {
        public int Next(int maxExclusive)
        {
            // Return 0 to make shuffling deterministic.
            return 0;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            // Return minInclusive to make shuffling deterministic.
            return minInclusive;
        }
    }

    private sealed class FakeRandomSequence : IRandom
    {
        private readonly int[] _values;
        private int _index;

        public FakeRandomSequence(int[] values)
        {
            _values = values;
            _index = 0;
        }

        public int Next(int maxExclusive)
        {
            // Return a bounded sequence value to avoid out-of-range values.
            if (_values.Length == 0) return 0;

            int v = _values[_index % _values.Length];
            _index++;

            // Clamp to [0, maxExclusive).
            return maxExclusive == 0 ? 0 : Math.Abs(v) % maxExclusive;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            int v = _values[_index % _values.Length];
            _index++;

            return Math.Min(minInclusive, maxExclusive == 0 ? 0 : Math.Abs(v) % maxExclusive);
        }
    }
}

