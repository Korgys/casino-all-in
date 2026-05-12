namespace casino.core.Games.Poker.Cards;

public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public static class SuitExtensions
{
    public static string ToSymbol(this Suit suit)
    {
        return suit switch
        {
            Suit.Hearts => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs => "♣",
            Suit.Spades => "♠",
            _ => throw new ArgumentOutOfRangeException(nameof(suit), suit, null)
        };
    }
}