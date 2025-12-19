using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Jeux.Poker.Cartes;

public class CartesCommunes
{
    public Carte Flop1 { get; set; }
    public Carte Flop2 { get; set; }
    public Carte Flop3 { get; set; }
    public Carte Turn { get; set; }
    public Carte River { get; set; }

    public IEnumerable<Carte> AsEnumerable()
    {
        var cartes = new List<Carte>();

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
