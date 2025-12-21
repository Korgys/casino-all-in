using System;
using System.Collections.Generic;
using System.Text;
using casino.core.Jeux.Poker.Cartes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using casino.core.Common.Utils;

namespace casino.core.tests.Jeux.Poker.Cartes;

[TestClass]
public class JeuDeCartesTests
{
    [TestMethod]
    public void Melanger_DoitCreer52CartesUniques()
    {
        // Arrange
        var deck = new JeuDeCartes(new FakeRandomToujoursZero());

        // Act
        deck.Melanger();

        // Assert
        Assert.AreEqual(52, deck.CartesRestantes);

        var cartes = TirerTout(deck);
        Assert.HasCount(52, cartes);

        // Vérifier unicité (rang + couleur)
        var uniques = cartes
            .Select(c => (c.Rang, c.Couleur))
            .Distinct()
            .Count();

        Assert.AreEqual(52, uniques, "Le paquet devrait contenir 52 cartes uniques.");
    }

    [TestMethod]
    public void TirerCarte_DoitDiminuerLeNombreDeCartes()
    {
        // Arrange
        var deck = new JeuDeCartes(new FakeRandomToujoursZero());
        deck.Melanger();
        int avant = deck.CartesRestantes;

        // Act
        var carte = deck.TirerCarte();

        // Assert
        Assert.IsNotNull(carte);
        Assert.AreEqual(avant - 1, deck.CartesRestantes);
    }

    [TestMethod]
    public void TirerCarte_QuandPaquetVide_DoitLeverException()
    {
        // Arrange
        var deck = new JeuDeCartes(new FakeRandomToujoursZero());

        // Vider le paquet
        TirerTout(deck);

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => deck.TirerCarte());
    }

    [TestMethod]
    public void Melanger_AvecRandomDeterministe_DoitProduireUnOrdreDeterministe()
    {
        // Arrange
        // Ce fake renvoie une séquence contrôlée : on obtient un ordre stable de run en run.
        var fakeRandom = new FakeRandomSequence(new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        var deck1 = new JeuDeCartes(fakeRandom);
        var deck2 = new JeuDeCartes(new FakeRandomSequence(Enumerable.Repeat(0, 51).ToArray()));

        // Act
        var seq1 = TirerTout(deck1).Select(c => (c.Rang, c.Couleur)).ToList();
        var seq2 = TirerTout(deck2).Select(c => (c.Rang, c.Couleur)).ToList();

        // Assert
        CollectionAssert.AreEqual(seq1, seq2, "Avec le même random déterministe, l'ordre doit être identique.");
    }

    // --------------------
    // Helpers
    // --------------------

    private static List<Carte> TirerTout(JeuDeCartes deck)
    {
        var cartes = new List<Carte>();
        while (deck.CartesRestantes > 0)
        {
            cartes.Add(deck.TirerCarte());
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
    }
}

