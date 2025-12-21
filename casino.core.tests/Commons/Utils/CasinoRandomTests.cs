using casino.core.Common.Utils;
using casino.core.Jeux.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Commons.Utils;

[TestClass]
public class CasinoRandomTests
{
    [TestMethod]
    public void Melanger_DoitCreer52CartesUniques()
    {
        // Arrange
        var random = new CasinoRandom();
        int maxExclusive = 11;

        // Act
        int n = random.Next(maxExclusive);

        // Assert
        Assert.IsGreaterThanOrEqualTo(0, n);
        Assert.IsLessThan(maxExclusive, n);
    }
}
