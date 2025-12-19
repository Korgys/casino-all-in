using casino.core.Jeux.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Jeux.Poker.Cartes;

[TestClass]
public class CartesMainTests
{
    [TestMethod]
    public void Constructeur_DoitInitialiserPremiereEtSeconde()
    {
        // Arrange
        var c1 = new Carte(RangCarte.As, Couleur.Pique);
        var c2 = new Carte(RangCarte.Roi, Couleur.Coeur);

        // Act
        var main = new CartesMain(c1, c2);

        // Assert
        Assert.AreSame(c1, main.Premiere);
        Assert.AreSame(c2, main.Seconde);
    }

    [TestMethod]
    public void AsEnumerable_DoitRetournerDeuxCartes_DansLeBonOrdre()
    {
        // Arrange
        var c1 = new Carte(RangCarte.As, Couleur.Pique);
        var c2 = new Carte(RangCarte.Roi, Couleur.Coeur);
        var main = new CartesMain(c1, c2);

        // Act
        var cartes = main.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(2, cartes);
        Assert.AreSame(c1, cartes[0]);
        Assert.AreSame(c2, cartes[1]);
    }

    [TestMethod]
    public void AsEnumerable_QuandSecondeEstNull_DoitRetournerSeulementPremiere()
    {
        // Arrange
        var c1 = new Carte(RangCarte.As, Couleur.Pique);
        var main = new CartesMain(c1, seconde: null!);

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
        var c1 = new Carte(RangCarte.As, Couleur.Pique);     // "A Pique"
        var c2 = new Carte(RangCarte.Roi, Couleur.Coeur);    // "K Coeur"
        var main = new CartesMain(c1, c2);

        // Act
        var s = main.ToString();

        // Assert
        Assert.AreEqual("A Pique, K Coeur", s);
    }
}
