using casino.core.Jeux.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Jeux.Poker.Cartes;

[TestClass]
public class CartesCommunesTests
{
    [TestMethod]
    public void AsEnumerable_QuandAucuneCarte_DoitRetournerListeVide()
    {
        // Arrange
        var communes = new CartesCommunes();

        // Act
        var cartes = communes.AsEnumerable().ToList();

        // Assert
        Assert.IsEmpty(cartes);
    }

    [TestMethod]
    public void AsEnumerable_DoitRetournerCartesNonNull_DansLOrdreFlopTurnRiver()
    {
        // Arrange
        var flop1 = new Carte(RangCarte.Deux, Couleur.Carreau);
        var flop2 = new Carte(RangCarte.Trois, Couleur.Coeur);
        var flop3 = new Carte(RangCarte.Quatre, Couleur.Pique);
        var turn = new Carte(RangCarte.As, Couleur.Trefle);
        var river = new Carte(RangCarte.Roi, Couleur.Coeur);

        var communes = new CartesCommunes
        {
            Flop1 = flop1,
            Flop2 = flop2,
            Flop3 = flop3,
            Turn = turn,
            River = river
        };

        // Act
        var cartes = communes.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(5, cartes);
        Assert.AreSame(flop1, cartes[0]);
        Assert.AreSame(flop2, cartes[1]);
        Assert.AreSame(flop3, cartes[2]);
        Assert.AreSame(turn, cartes[3]);
        Assert.AreSame(river, cartes[4]);
    }

    [TestMethod]
    public void ToString_QuandAucuneCarte_DoitRetournerChaineVide()
    {
        // Arrange
        var communes = new CartesCommunes();

        // Act
        var s = communes.ToString();

        // Assert
        Assert.AreEqual(string.Empty, s);
    }

    [TestMethod]
    public void ToString_QuandCartesPresentes_DoitFaireJoinAvecVirgules()
    {
        // Arrange
        var communes = new CartesCommunes
        {
            Flop1 = new Carte(RangCarte.As, Couleur.Pique),   // "A Pique"
            Turn = new Carte(RangCarte.Dix, Couleur.Coeur)    // "10 Coeur"
        };

        // Act
        var s = communes.ToString();

        // Assert
        Assert.AreEqual("A Pique, 10 Coeur", s);
    }

    [TestMethod]
    public void AsEnumerable_QuandCartesIntermediairesNull_DoitIgnorerLesNulls()
    {
        // Arrange
        var flop1 = new Carte(RangCarte.As, Couleur.Pique);
        var river = new Carte(RangCarte.Dame, Couleur.Carreau);

        var communes = new CartesCommunes
        {
            Flop1 = flop1,
            // Flop2 null
            // Flop3 null
            // Turn null
            River = river
        };

        // Act
        var cartes = communes.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(2, cartes);
        Assert.AreSame(flop1, cartes[0]);
        Assert.AreSame(river, cartes[1]);
    }
}
