using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Poker.Scores;

public sealed class Score : IComparable<Score>, IEquatable<Score>
{
    public HandRank Rank { get; }
    public CardRank CardValue { get; }

    /// <summary>
    /// Stocker les valeurs de départage dans l'ordre décroissant (kickers).
    /// </summary>
    public IReadOnlyList<CardRank> Kickers { get; }

    public Score(HandRank rangMain, CardRank valeur, IEnumerable<CardRank>? kickers = null)
    {
        Rank = rangMain;
        CardValue = valeur;
        Kickers = (kickers ?? Array.Empty<CardRank>()).ToList().AsReadOnly();
    }

    public int CompareTo(Score? other)
    {
        if (other is null) return 1;

        // Comparer d'abord le type de main.
        int cmp = Rank.CompareTo(other.Rank);
        if (cmp != 0) return cmp;

        // Comparer ensuite la valeur principale (ex: valeur de la paire/brelan/...).
        cmp = CardValue.CompareTo(other.CardValue);
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

    public bool Equals(Score? other)
    {
        if (other is null) return false;
        return CompareTo(other) == 0;
    }

    public override bool Equals(object? obj) => obj is Score other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Rank, CardValue, Kickers.Aggregate(0, (hash, rank) => HashCode.Combine(hash, rank)));

    public static bool operator ==(Score? left, Score? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Score? left, Score? right) => !(left == right);

    public static bool operator <(Score? left, Score? right)
    {
        if (left is null) return right is not null;
        if (right is null) return false;
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Score? left, Score? right)
    {
        if (left is null) return true;
        if (right is null) return false;
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Score? left, Score? right)
    {
        if (left is null) return false;
        if (right is null) return true;
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Score? left, Score? right)
    {
        if (left is null) return right is null;
        if (right is null) return true;
        return left.CompareTo(right) >= 0;
    }

    public override string ToString()
    {
        return $"{Rank.ToDisplayString()} de {CardValue}";
    }
}
