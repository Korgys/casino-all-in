using casino.core.Games.Poker.Cards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Games.Poker.Scores;

public sealed class Score : IComparable<Score>
{
    public HandRank Rang { get; }
    public CardRank Valeur { get; }

    /// <summary>
    /// Stocker les valeurs de départage dans l'ordre décroissant (kickers).
    /// </summary>
    public IReadOnlyList<CardRank> Kickers { get; }

    public Score(HandRank rangMain, CardRank valeur, IEnumerable<CardRank>? kickers = null)
    {
        Rang = rangMain;
        Valeur = valeur;
        Kickers = (kickers ?? Array.Empty<CardRank>()).ToList().AsReadOnly();
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
            var a = i < Kickers.Count ? Kickers[i] : (CardRank)0;
            var b = i < other.Kickers.Count ? other.Kickers[i] : (CardRank)0;

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
