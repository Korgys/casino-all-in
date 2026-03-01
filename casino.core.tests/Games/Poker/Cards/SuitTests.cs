using casino.core.Games.Poker.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class SuitTests
{
    [TestMethod]
    [DataRow(Suit.Hearts, "♥")]
    [DataRow(Suit.Diamonds, "♦")]
    [DataRow(Suit.Clubs, "♣")]
    [DataRow(Suit.Spades, "♠")]
    public void ToSymbol_DoitRetournerLeBonSymbole(Suit couleur, string attendu)
    {
        // Act
        var result = couleur.ToSymbol();

        // Assert
        Assert.AreEqual(attendu, result);
    }

    [TestMethod]
    public void ToSymbol_QuandSuitInvalide_DoitLeverArgumentOutOfRangeException()
    {
        // Arrange
        var couleurInvalide = (Suit)999;

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => couleurInvalide.ToSymbol());
    }
}
