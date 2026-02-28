using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Cartes;

public class HandCards
{
    public Card Premiere { get; }
    public Card Seconde { get; }

    public HandCards(Card premiere, Card seconde)
    {
        Premiere = premiere;
        Seconde = seconde;
    }

    public IEnumerable<Card> AsEnumerable()
    {
        var cartes = new List<Card>();

        if (Premiere != null) cartes.Add(Premiere);
        if (Seconde != null) cartes.Add(Seconde);

        return cartes;
    }

    public override string ToString()
    {
        return $"{Premiere}, {Seconde}";
    }
}
