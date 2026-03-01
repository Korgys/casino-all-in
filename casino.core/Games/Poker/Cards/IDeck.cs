namespace casino.core.Games.Poker.Cards;

public interface IDeck
{
    Card DrawCard();
    void Shuffle();
}
