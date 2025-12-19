namespace casino.core.Jeux.Poker.Cartes;

public class Carte
{
    public RangCarte Rang { get; }
    public Couleur Couleur { get; }
    public Carte(RangCarte rang, Couleur couleur)
    {
        Rang = rang;
        Couleur = couleur;
    }
    public override string ToString()
    {
        return $"{Rang.ToShortString()} {Couleur}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Carte other)
            return false;

        return Rang == other.Rang && Couleur == other.Couleur;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Rang, Couleur);
    }

}