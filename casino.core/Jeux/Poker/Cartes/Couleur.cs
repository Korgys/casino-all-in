namespace casino.core.Jeux.Poker.Cartes;

public enum Couleur
{
    Coeur,
    Carreau,
    Trèfle,
    Pique
}

public static class CouleurExtensions
{
    public static string ToSymbol(this Couleur couleur)
    {
        return couleur switch
        {
            Couleur.Coeur => "♥",
            Couleur.Carreau => "♦",
            Couleur.Trèfle => "♣",
            Couleur.Pique => "♠",
            _ => throw new ArgumentOutOfRangeException(nameof(couleur), couleur, null)
        };
    }
}