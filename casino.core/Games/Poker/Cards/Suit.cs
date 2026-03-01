namespace casino.core.Games.Poker.Cards;

public enum Suit
{
    Coeur,
    Carreau,
    Trefle,
    Pique
}

public static class SuitExtensions
{
    public static string ToSymbol(this Suit couleur)
    {
        return couleur switch
        {
            Suit.Coeur => "♥",
            Suit.Carreau => "♦",
            Suit.Trefle => "♣",
            Suit.Pique => "♠",
            _ => throw new ArgumentOutOfRangeException(nameof(couleur), couleur, null)
        };
    }
}