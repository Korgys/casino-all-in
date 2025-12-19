using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Jeux.Poker.Cartes;

public class CartesMain
{
    public Carte Premiere { get; }
    public Carte Seconde { get; }

    public CartesMain(Carte premiere, Carte seconde)
    {
        Premiere = premiere;
        Seconde = seconde;
    }

    public IEnumerable<Carte> AsEnumerable()
    {
        var cartes = new List<Carte>();

        if (Premiere != null) cartes.Add(Premiere);
        if (Seconde != null) cartes.Add(Seconde);

        return cartes;
    }

    public override string ToString()
    {
        return $"{Premiere}, {Seconde}";
    }
}
