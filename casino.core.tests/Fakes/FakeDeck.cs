using casino.core.Games.Poker.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Fakes;

public class FakeDeck : IDeck
{
    private readonly Queue<Card> _cartes;

    public FakeDeck(IEnumerable<Card> cartes)
    {
        _cartes = new Queue<Card>(cartes);
    }

    public Card DrawCard()
    {
        return _cartes.Count > 0 ? _cartes.Dequeue() : new Card(CardRank.Deux, Suit.Carreau);
    }

    public void Shuffle()
    {
        // Aucun mélange nécessaire pour les tests.
    }
}