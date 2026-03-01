using System;
using System.Collections.Generic;
using System.Text;
using casino.core.Games.Poker.Cards;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using casino.core.Common.Utils;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class DeckTests
{
    [TestMethod]
    public void Shuffle_DoitCreer52CartesUniques()
    {
        // Arrange
        var deck = new Deck(new FakeRandomToujoursZero());

        // Act
        deck.Shuffle();

        // Assert
        Assert.AreEqual(52, deck.RemainingCards);

        var cartes = TirerTout(deck);
        Assert.HasCount(52, cartes);

        // Vérifier unicité (rang + couleur)
        var uniques = cartes
            .Select(c => (c.Rang, c.Suit))
            .Distinct()
            .Count();

        Assert.AreEqual(52, uniques, "Le paquet devrait contenir 52 cartes uniques.");
    }

    [TestMethod]
    public void DrawCard_DoitDiminuerLeNombreDeCartes()
    {
        // Arrange
        var deck = new Deck(new FakeRandomToujoursZero());
        deck.Shuffle();
        int avant = deck.RemainingCards;

        // Act
        var carte = deck.DrawCard();

        // Assert
        Assert.IsNotNull(carte);
        Assert.AreEqual(avant - 1, deck.RemainingCards);
    }

    [TestMethod]
    public void DrawCard_QuandPaquetVide_DoitLeverException()
    {
        // Arrange
        var deck = new Deck(new FakeRandomToujoursZero());

        // Vider le paquet
        TirerTout(deck);

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => deck.DrawCard());
    }

    [TestMethod]
    public void Shuffle_AvecRandomDeterministe_DoitProduireUnOrdreDeterministe()
    {
        // Arrange
        // Ce fake renvoie une séquence contrôlée : on obtient un ordre stable de run en run.
        var fakeRandom = new FakeRandomSequence(new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        var deck1 = new Deck(fakeRandom);
        var deck2 = new Deck(new FakeRandomSequence(Enumerable.Repeat(0, 51).ToArray()));

        // Act
        var seq1 = TirerTout(deck1).Select(c => (c.Rang, c.Suit)).ToList();
        var seq2 = TirerTout(deck2).Select(c => (c.Rang, c.Suit)).ToList();

        // Assert
        CollectionAssert.AreEqual(seq1, seq2, "Avec le même random déterministe, l'ordre doit être identique.");
    }

    // --------------------
    // Helpers
    // --------------------

    private static List<Card> TirerTout(Deck deck)
    {
        var cartes = new List<Card>();
        while (deck.RemainingCards > 0)
        {
            cartes.Add(deck.DrawCard());
        }
        return cartes;
    }

    private sealed class FakeRandomToujoursZero : IRandom
    {
        public int Next(int maxExclusive)
        {
            // Retourner 0 pour rendre le mélange déterministe.
            return 0;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            // Retourner minInclusive pour rendre le mélange déterministe.
            return minInclusive;
        }
    }

    private sealed class FakeRandomSequence : IRandom
    {
        private readonly int[] _values;
        private int _index;

        public FakeRandomSequence(int[] values)
        {
            _values = values;
            _index = 0;
        }

        public int Next(int maxExclusive)
        {
            // Retourner une valeur de séquence, bornée, pour éviter les sorties de plage.
            if (_values.Length == 0) return 0;

            int v = _values[_index % _values.Length];
            _index++;

            // Ramener dans [0; maxExclusive[
            return maxExclusive == 0 ? 0 : Math.Abs(v) % maxExclusive;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            int v = _values[_index % _values.Length];
            _index++;

            return Math.Min(minInclusive, maxExclusive == 0 ? 0 : Math.Abs(v) % maxExclusive);
        }
    }
}

