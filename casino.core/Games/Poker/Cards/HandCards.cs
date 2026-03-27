namespace casino.core.Games.Poker.Cards;

public class HandCards
{
    public Card First { get; }
    public Card Second { get; }

    public HandCards(Card first, Card second)
    {
        First = first;
        Second = second;
    }

    public IEnumerable<Card> AsEnumerable()
    {
        var cards = new List<Card>();

        if (First != null) cards.Add(First);
        if (Second != null) cards.Add(Second);

        return cards;
    }

    public override string ToString()
    {
        return $"{First}, {Second}";
    }
}
