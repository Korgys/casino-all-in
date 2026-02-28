using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Cartes;

public class TableCards
{
    public Card? Flop1 { get; set; }
    public Card? Flop2 { get; set; }
    public Card? Flop3 { get; set; }
    public Card? Turn { get; set; }
    public Card? River { get; set; }

    public IEnumerable<Card> AsEnumerable()
    {
        var cartes = new List<Card>();

        if (Flop1 != null) cartes.Add(Flop1);
        if (Flop2 != null) cartes.Add(Flop2);
        if (Flop3 != null) cartes.Add(Flop3);
        if (Turn != null) cartes.Add(Turn);
        if (River != null) cartes.Add(River);

        return cartes;
    }

    public override string ToString()
    {
        return AsEnumerable().Count() > 0 ? string.Join(", ", AsEnumerable()) : "";
    }
}
