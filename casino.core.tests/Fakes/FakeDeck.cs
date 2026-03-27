using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Fakes;

public class FakeDeck : IDeck
{
    private readonly Queue<Card> _cards;

    public FakeDeck(IEnumerable<Card> cards)
    {
        _cards = new Queue<Card>(cards);
    }

    public Card DrawCard()
    {
        return _cards.Count > 0 ? _cards.Dequeue() : new Card(CardRank.Deux, Suit.Diamonds);
    }

    public void Shuffle()
    {
        // No shuffle needed for tests.
    }
}
