using casino.core.Properties.Langages;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Scores;

public enum HandRank
{
    HighCard = 1,
    OnePair = 2,
    TwoPair = 3,
    ThreeOfAKind = 4,
    Straight = 5,
    Flush = 6,
    FullHouse = 7,
    FourOfAKind = 8,
    StraightFlush = 9,
    RoyalFlush = 10
}

public static class HandRankExtensions
{
    public static string ToDisplayString(this HandRank handRank)
    {
        return handRank switch
        {
            HandRank.HighCard => Resources.HighCard,
            HandRank.OnePair => Resources.OnePair,
            HandRank.TwoPair => Resources.TwoPair,
            HandRank.ThreeOfAKind => Resources.ThreeOfAKind,
            HandRank.Straight => Resources.Straight,
            HandRank.Flush => Resources.Flush,
            HandRank.FullHouse => Resources.FullHouse,
            HandRank.FourOfAKind => Resources.FourOfAKind,
            HandRank.StraightFlush => Resources.StraightFlush,
            HandRank.RoyalFlush => Resources.RoyalFlush,
            _ => Resources.None
        };
    }
}
