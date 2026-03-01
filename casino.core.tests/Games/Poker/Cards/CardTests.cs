using casino.core.Games.Poker.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class CarteTests
{
    [TestMethod]
    public void Constructeur_DoitInitialiserRangEtSuit()
    {
        // Arrange
        var rang = CardRank.As;
        var couleur = Suit.Hearts;

        // Act
        var carte = new Card(rang, couleur);

        // Assert
        Assert.AreEqual(rang, carte.Rank, "Le rang devrait être celui passé au constructeur.");
        Assert.AreEqual(couleur, carte.Suit, "La couleur devrait être celle passée au constructeur.");
    }

    [TestMethod]
    public void ToString_DoitRetournerRangCourtEtSuit()
    {
        // Arrange
        var carte = new Card(CardRank.As, Suit.Spades);

        // Act
        var result = carte.ToString();

        // Assert
        Assert.AreEqual("A♠", result, "Le format ToString n'est pas conforme.");
    }

    [TestMethod]
    public void ToString_DoitEtreDeterministe()
    {
        // Arrange
        var carte = new Card(CardRank.Dix, Suit.Diamonds);

        // Act
        var first = carte.ToString();
        var second = carte.ToString();

        // Assert
        Assert.AreEqual(first, second, "ToString devrait toujours retourner la même valeur.");
    }

    [TestMethod]
    public void Equals_DeuxCartesIdentiques_DoitRetournerTrue()
    {
        // Arrange
        var carte1 = new Card(CardRank.Roi, Suit.Clubs);
        var carte2 = new Card(CardRank.Roi, Suit.Clubs);

        // Act
        var equals = carte1.Equals(carte2);

        // Assert
        Assert.IsTrue(equals, "Deux cartes avec même rang et couleur devraient être égales.");
    }

    [TestMethod]
    public void Equals_DeuxCartesDifferentes_DoitRetournerFalse()
    {
        // Arrange
        var carte1 = new Card(CardRank.Roi, Suit.Clubs);
        var carte2 = new Card(CardRank.Roi, Suit.Hearts);

        // Act
        var equals = carte1.Equals(carte2);

        // Assert
        Assert.IsFalse(equals, "Deux cartes avec une couleur différente ne devraient pas être égales.");
    }

    [TestMethod]
    public void GetHashCode_DeuxCartesIdentiques_DoitRetournerMemeValeur()
    {
        // Arrange
        var carte1 = new Card(CardRank.Dame, Suit.Hearts);
        var carte2 = new Card(CardRank.Dame, Suit.Hearts);

        // Act
        var hash1 = carte1.GetHashCode();
        var hash2 = carte2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2, "Deux cartes identiques doivent avoir le même hash.");
    }
}
