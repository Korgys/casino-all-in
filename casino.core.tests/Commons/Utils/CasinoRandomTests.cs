using casino.core.Common.Utils;

namespace casino.core.tests.Commons.Utils;

[TestClass]
public class CasinoRandomTests
{
    [TestMethod]
    public void Shuffle_DoitCreer52CartesUniques()
    {
        // Arrange
        IRandom random = new CasinoRandom();
        int maxExclusive = 11;

        // Act
        int n = random.Next(maxExclusive);

        // Assert
        Assert.IsGreaterThanOrEqualTo(0, n);
        Assert.IsLessThan(maxExclusive, n);
    }

    [TestMethod]
    public void Next_WithMinInclusiveAndMaxExclusive_ShouldStayWithinBounds()
    {
        // Arrange
        IRandom random = new CasinoRandom();
        int minInclusive = 7;
        int maxExclusive = 19;
        const int iterations = 10000;

        // Act / Assert
        for (int i = 0; i < iterations; i++)
        {
            int value = random.Next(minInclusive, maxExclusive);
            Assert.IsGreaterThanOrEqualTo(minInclusive, value);
            Assert.IsLessThan(maxExclusive, value);
        }
    }
}
