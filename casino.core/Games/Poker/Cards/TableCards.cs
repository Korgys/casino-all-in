using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Games.Poker.Cards;

public class TableCards
{
    public Card? Flop1 { get; set; }
    public Card? Flop2 { get; set; }
    public Card? Flop3 { get; set; }
    public Card? Turn { get; set; }
    public Card? River { get; set; }

    public IEnumerable<Card> AsEnumerable()
    {
        var cards = new List<Card>();

        if (Flop1 != null) cards.Add(Flop1);
        if (Flop2 != null) cards.Add(Flop2);
        if (Flop3 != null) cards.Add(Flop3);
        if (Turn != null) cards.Add(Turn);
        if (River != null) cards.Add(River);

        return cards;
    }

    public override string ToString()
    {
        return AsEnumerable().Any() ? string.Join(", ", AsEnumerable()) : "";
    }
}
