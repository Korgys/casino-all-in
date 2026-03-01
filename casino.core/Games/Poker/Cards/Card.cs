namespace casino.core.Games.Poker.Cards;

public class Card
{
    public CardRank Rank { get; }
    public Suit Suit { get; }
    public Card(CardRank rank, Suit suit)
    {
        Rank = rank;
        Suit = suit;
    }
    public override string ToString()
    {
        return $"{Rank.ToShortString()}{Suit.ToSymbol()}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Card other)
            return false;

        return Rank == other.Rank && Suit == other.Suit;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Rank, Suit);
    }
}