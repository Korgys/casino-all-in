using casino.core.Games.Poker.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class CartesMainTests
{
    [TestMethod]
    public void Constructeur_DoitInitialiserFirstEtSecond()
    {
        // Arrange
        var c1 = new Card(CardRank.As, Suit.Spades);
        var c2 = new Card(CardRank.Roi, Suit.Hearts);

        // Act
        var main = new HandCards(c1, c2);

        // Assert
        Assert.AreSame(c1, main.First);
        Assert.AreSame(c2, main.Second);
    }

    [TestMethod]
    public void AsEnumerable_DoitRetournerDeuxCartes_DansLeBonOrdre()
    {
        // Arrange
        var c1 = new Card(CardRank.As, Suit.Spades);
        var c2 = new Card(CardRank.Roi, Suit.Hearts);
        var main = new HandCards(c1, c2);

        // Act
        var cartes = main.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(2, cartes);
        Assert.AreSame(c1, cartes[0]);
        Assert.AreSame(c2, cartes[1]);
    }

    [TestMethod]
    public void AsEnumerable_QuandSecondEstNull_DoitRetournerSeulementFirst()
    {
        // Arrange
        var c1 = new Card(CardRank.As, Suit.Spades);
        var main = new HandCards(c1, second: null!);

        // Act
        var cartes = main.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(1, cartes);
        Assert.AreSame(c1, cartes[0]);
    }

    [TestMethod]
    public void ToString_DoitConcatenerLesDeuxCartes()
    {
        // Arrange
        var c1 = new Card(CardRank.As, Suit.Spades);     // "A Pique"
        var c2 = new Card(CardRank.Roi, Suit.Hearts);    // "K Coeur"
        var main = new HandCards(c1, c2);

        // Act
        var s = main.ToString();

        // Assert
        Assert.AreEqual("A♠, K♥", s);
    }
}
