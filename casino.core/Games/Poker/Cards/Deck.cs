using casino.core.Common.Utils;
using System;

namespace casino.core.Games.Poker.Cards;

public class Deck : IDeck
{
    private readonly List<Card> _cartes = new();
    private readonly IRandom _random;

    public int CartesRestantes => _cartes.Count;

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
        _cartes.Clear();
        _cartes.AddRange(CreerPaquetStandard());

        // Mélanger le paquet (Fisher–Yates).
        for (int i = _cartes.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (_cartes[i], _cartes[j]) = (_cartes[j], _cartes[i]);
        }
    }

    /// <summary>
    /// Tire une carte du dessus (index 0) pour simplifier et rendre testable.
    /// </summary>
    public Card DrawCard()
    {
        if (_cartes.Count == 0)
            throw new InvalidOperationException("Le paquet est vide.");

        // Retirer la première carte : déterministe après mélange.
        var carte = _cartes[0];
        _cartes.RemoveAt(0);
        return carte;
    }

    /// <summary>
    /// Crée un jeu de cartes avec 52 cartes (4 couleurs, 13 rangs).
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<Card> CreerPaquetStandard()
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
