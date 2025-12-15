using System.ComponentModel;

namespace casino.core.Jeux.Poker.Cartes;

public enum RangCarte
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
    public static string ToShortString(this RangCarte rang)
    {
        return rang switch
        {
            RangCarte.Deux => "2",
            RangCarte.Trois => "3",
            RangCarte.Quatre => "4",
            RangCarte.Cinq => "5",
            RangCarte.Six => "6",
            RangCarte.Sept => "7",
            RangCarte.Huit => "8",
            RangCarte.Neuf => "9",
            RangCarte.Dix => "10",
            RangCarte.Valet => "J",
            RangCarte.Dame => "Q",
            RangCarte.Roi => "K",
            RangCarte.As => "A",
            _ => throw new ArgumentOutOfRangeException(nameof(rang), rang, null)
        };
    }
}