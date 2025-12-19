using casino.core.Jeux.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Jeux.Poker.Cartes;

[TestClass]
public class CouleurTests
{
    [TestMethod]
    [DataRow(Couleur.Coeur, "♥")]
    [DataRow(Couleur.Carreau, "♦")]
    [DataRow(Couleur.Trefle, "♣")]
    [DataRow(Couleur.Pique, "♠")]
    public void ToSymbol_DoitRetournerLeBonSymbole(Couleur couleur, string attendu)
    {
        // Act
        var result = couleur.ToSymbol();

        // Assert
        Assert.AreEqual(attendu, result);
    }

    [TestMethod]
    public void ToSymbol_QuandCouleurInvalide_DoitLeverArgumentOutOfRangeException()
    {
        // Arrange
        var couleurInvalide = (Couleur)999;

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => couleurInvalide.ToSymbol());
    }
}
