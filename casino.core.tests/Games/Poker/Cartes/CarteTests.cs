using casino.core.Games.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Games.Poker.Cartes;

[TestClass]
public class CarteTests
{
    [TestMethod]
    public void Constructeur_DoitInitialiserRangEtCouleur()
    {
        // Arrange
        var rang = RangCarte.As;
        var couleur = Couleur.Coeur;

        // Act
        var carte = new Card(rang, couleur);

        // Assert
        Assert.AreEqual(rang, carte.Rang, "Le rang devrait être celui passé au constructeur.");
        Assert.AreEqual(couleur, carte.Couleur, "La couleur devrait être celle passée au constructeur.");
    }

    [TestMethod]
    public void ToString_DoitRetournerRangCourtEtCouleur()
    {
        // Arrange
        var carte = new Card(RangCarte.As, Couleur.Pique);

        // Act
        var result = carte.ToString();

        // Assert
        Assert.AreEqual("A♠", result, "Le format ToString n'est pas conforme.");
    }

    [TestMethod]
    public void ToString_DoitEtreDeterministe()
    {
        // Arrange
        var carte = new Card(RangCarte.Dix, Couleur.Carreau);

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
        var carte1 = new Card(RangCarte.Roi, Couleur.Trefle);
        var carte2 = new Card(RangCarte.Roi, Couleur.Trefle);

        // Act
        var equals = carte1.Equals(carte2);

        // Assert
        Assert.IsTrue(equals, "Deux cartes avec même rang et couleur devraient être égales.");
    }

    [TestMethod]
    public void Equals_DeuxCartesDifferentes_DoitRetournerFalse()
    {
        // Arrange
        var carte1 = new Card(RangCarte.Roi, Couleur.Trefle);
        var carte2 = new Card(RangCarte.Roi, Couleur.Coeur);

        // Act
        var equals = carte1.Equals(carte2);

        // Assert
        Assert.IsFalse(equals, "Deux cartes avec une couleur différente ne devraient pas être égales.");
    }

    [TestMethod]
    public void GetHashCode_DeuxCartesIdentiques_DoitRetournerMemeValeur()
    {
        // Arrange
        var carte1 = new Card(RangCarte.Dame, Couleur.Coeur);
        var carte2 = new Card(RangCarte.Dame, Couleur.Coeur);

        // Act
        var hash1 = carte1.GetHashCode();
        var hash2 = carte2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2, "Deux cartes identiques doivent avoir le même hash.");
    }
}
