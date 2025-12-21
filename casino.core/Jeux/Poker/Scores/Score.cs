using casino.core.Jeux.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Jeux.Poker.Scores;

public sealed class Score : IComparable<Score>
{
    public RangMain Rang { get; }
    public RangCarte Valeur { get; }

    /// <summary>
    /// Stocker les valeurs de départage dans l'ordre décroissant (kickers).
    /// </summary>
    public IReadOnlyList<RangCarte> Kickers { get; }

    public Score(RangMain rangMain, RangCarte valeur, IEnumerable<RangCarte>? kickers = null)
    {
        Rang = rangMain;
        Valeur = valeur;
        Kickers = (kickers ?? Array.Empty<RangCarte>()).ToList().AsReadOnly();
    }

    public int CompareTo(Score? other)
    {
        if (other is null) return 1;

        // Comparer d'abord le type de main.
        int cmp = Rang.CompareTo(other.Rang);
        if (cmp != 0) return cmp;

        // Comparer ensuite la valeur principale (ex: valeur de la paire/brelan/...).
        cmp = Valeur.CompareTo(other.Valeur);
        if (cmp != 0) return cmp;

        // Comparer enfin les kickers (lexicographique).
        int len = Math.Max(Kickers.Count, other.Kickers.Count);
        for (int i = 0; i < len; i++)
        {
            var a = i < Kickers.Count ? Kickers[i] : (RangCarte)0;
            var b = i < other.Kickers.Count ? other.Kickers[i] : (RangCarte)0;

            cmp = a.CompareTo(b);
            if (cmp != 0) return cmp;
        }

        return 0;
    }

    public override string ToString()
    {
        return $"{Rang} de {Valeur}";
    }
}
