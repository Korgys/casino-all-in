using casino.core.Common.Utils;

namespace casino.core.Games.Poker.Cards;

public class Deck : IDeck
{
    private readonly List<Card> _cards = new();
    private readonly IRandom _random;

    public int RemainingCards => _cards.Count;

    public Deck(IRandom? random = null)
    {
        _random = random ?? new CasinoRandom();
        Shuffle();
    }

    /// <summary>
    /// Rebuilds a standard 52-card deck and shuffles it.
    /// </summary>
    public void Shuffle()
    {
        _cards.Clear();
        _cards.AddRange(CreateDeck());

        // Fisher-Yates shuffle.
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    /// <summary>
    /// Draws the top card to keep deck behavior deterministic in tests.
    /// </summary>
    public Card DrawCard()
    {
        if (_cards.Count == 0)
            throw new InvalidOperationException("The deck is empty.");

        var card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    /// <summary>
    /// Creates a 52-card deck with four suits and thirteen ranks.
    /// </summary>
    private static IEnumerable<Card> CreateDeck()
    {
        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (CardRank rank in Enum.GetValues<CardRank>())
            {
                yield return new Card(rank, suit);
            }
        }
    }
}
