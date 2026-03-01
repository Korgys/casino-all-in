namespace casino.core.Games.Poker.Cards;

public class Card
{
    public CardRank Rang { get; }
    public Suit Suit { get; }
    public Card(CardRank rang, Suit suit)
    {
        Rang = rang;
        Suit = suit;
    }
    public override string ToString()
    {
        return $"{Rang.ToShortString()}{Suit.ToSymbol()}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Card other)
            return false;

        return Rang == other.Rang && Suit == other.Suit;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Rang, Suit);
    }

}