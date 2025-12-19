namespace casino.core.Jeux.Poker.Cartes;

public enum Couleur
{
    Coeur,
    Carreau,
    Trefle,
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
            Couleur.Trefle => "♣",
            Couleur.Pique => "♠",
            _ => throw new ArgumentOutOfRangeException(nameof(couleur), couleur, null)
        };
    }
}