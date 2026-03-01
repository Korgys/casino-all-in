using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Cards;

public class HandCards
{
    public Card First { get; }
    public Card Second { get; }

    public HandCards(Card premiere, Card seconde)
    {
        First = premiere;
        Second = seconde;
    }

    public IEnumerable<Card> AsEnumerable()
    {
        var cartes = new List<Card>();

        if (First != null) cartes.Add(First);
        if (Second != null) cartes.Add(Second);

        return cartes;
    }

    public override string ToString()
    {
        return $"{First}, {Second}";
    }
}
