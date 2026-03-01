using System.ComponentModel;

namespace casino.core.Games.Poker.Cards;

public enum CardRank
{
    Deux = 2,
    Trois = 3,
    Quatre = 4,
    Cinq = 5,
    Six = 6,
    Sept = 7,
    Huit = 8,
    Neuf = 9,
    Dix = 10,
    Valet = 11,
    Dame = 12,
    Roi = 13,
    As = 14
}

public static class RangExtensions
{
    public static string ToShortString(this CardRank rang)
    {
        return rang switch
        {
            CardRank.Deux => "2",
            CardRank.Trois => "3",
            CardRank.Quatre => "4",
            CardRank.Cinq => "5",
            CardRank.Six => "6",
            CardRank.Sept => "7",
            CardRank.Huit => "8",
            CardRank.Neuf => "9",
            CardRank.Dix => "10",
            CardRank.Valet => "J",
            CardRank.Dame => "Q",
            CardRank.Roi => "K",
            CardRank.As => "A",
            _ => throw new ArgumentOutOfRangeException(nameof(rang), rang, null)
        };
    }
}