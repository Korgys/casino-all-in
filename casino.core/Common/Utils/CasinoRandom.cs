using System;

namespace casino.core.Common.Utils;

/// <summary>
/// Provides a thread-safe random number generator suitable for use in gaming and casino applications.
/// </summary>
/// <remarks>This class uses the shared instance of the .NET <see cref="System.Random"/> class to generate random
/// numbers. It is designed to be efficient and safe for concurrent use across multiple threads. Use this class when you
/// require random integer values for scenarios such as games of chance or simulations where fairness and
/// unpredictability are important.</remarks>
public class CasinoRandom : IRandom
{
    private readonly Random _random = Random.Shared;

    public int Next(int maxExclusive) => _random.Next(maxExclusive);

    public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
}
