namespace casino.core.Jeux.Poker.Cartes;

public interface IDeck
{
    Carte TirerCarte();
    void Melanger();
}
