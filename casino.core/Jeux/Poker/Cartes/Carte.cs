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
}