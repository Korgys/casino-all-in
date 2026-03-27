using casino.core.Common.Utils;

namespace casino.core.Games.Poker.Cards;

public class Deck : IDeck
{
    private readonly List<Card> _cards = new();
    private readonly IRandom _random;

    public int RemainingCards => _cards.Count;

    public Deck(IRandom? random = null)
    {
        _random = random ?? new CasinoRandom();
        Shuffle();
    }

    /// <summary>
    /// Reconstruire un paquet standard (52 cartes) puis mélanger réellement.
    /// </summary>
    public void Shuffle()
    {
        // Reconstruire le paquet.
        _cards.Clear();
        _cards.AddRange(CreateDeck());

        // Mélanger le paquet (Fisher–Yates).
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    /// <summary>
    /// Tire une carte du dessus (index 0) pour simplifier et rendre testable.
    /// </summary>
    public Card DrawCard()
    {
        if (_cards.Count == 0)
            throw new InvalidOperationException("Le paquet est vide.");

        // Retirer la première carte : déterministe après mélange.
        var card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    /// <summary>
    /// Crée un jeu de cartes avec 52 cartes (4 couleurs, 13 rangs).
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<Card> CreateDeck()
    {
        foreach (Suit couleur in Enum.GetValues<Suit>())
        {
            foreach (CardRank rang in Enum.GetValues<CardRank>())
            {
                yield return new Card(rang, couleur);
            }
        }
    }
}
