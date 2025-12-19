using casino.core.Jeux.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Jeux.Poker.Cartes;

[TestClass]
public class RangCarteTests
{
    [TestMethod]
    [DataRow(RangCarte.Deux, "2")]
    [DataRow(RangCarte.Trois, "3")]
    [DataRow(RangCarte.Quatre, "4")]
    [DataRow(RangCarte.Cinq, "5")]
    [DataRow(RangCarte.Six, "6")]
    [DataRow(RangCarte.Sept, "7")]
    [DataRow(RangCarte.Huit, "8")]
    [DataRow(RangCarte.Neuf, "9")]
    [DataRow(RangCarte.Dix, "10")]
    [DataRow(RangCarte.Valet, "J")]
    [DataRow(RangCarte.Dame, "Q")]
    [DataRow(RangCarte.Roi, "K")]
    [DataRow(RangCarte.As, "A")]
    public void ToShortString_DoitRetournerLeBonFormat(RangCarte rang, string attendu)
    {
        // Act
        var result = rang.ToShortString();

        // Assert
        Assert.AreEqual(attendu, result);
    }

    [TestMethod]
    public void ToShortString_QuandRangInvalide_DoitLeverArgumentOutOfRangeException()
    {
        // Arrange
        var rangInvalide = (RangCarte)999;

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => rangInvalide.ToShortString());
    }
}
