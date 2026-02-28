using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Common.Utils;

/// <summary>
/// Defines methods for generating random integers within specified ranges.
/// </summary>
/// <remarks>Implementations of this interface should ensure that the generated values are uniformly distributed
/// across the specified range. The methods allow callers to specify either an upper bound or both lower and upper
/// bounds for the random number generation. This interface is intended for scenarios where deterministic or
/// customizable random number generation is required, such as in testing or gaming applications.</remarks>
public interface IRandom
{
    int Next(int maxExclusive);
    int Next(int minInclusive, int maxExclusive);
}

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
    public int Next(int minInclusive, int maxExclusive) => _random.Next(maxExclusive);
}
