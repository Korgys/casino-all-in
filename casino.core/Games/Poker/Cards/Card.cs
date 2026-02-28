namespace casino.core.Games.Poker.Cartes;

public class Card
{
    public RangCarte Rang { get; }
    public Couleur Couleur { get; }
    public Card(RangCarte rang, Couleur couleur)
    {
        Rang = rang;
        Couleur = couleur;
    }
    public override string ToString()
    {
        return $"{Rang.ToShortString()}{Couleur.ToSymbol()}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Card other)
            return false;

        return Rang == other.Rang && Couleur == other.Couleur;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Rang, Couleur);
    }

}