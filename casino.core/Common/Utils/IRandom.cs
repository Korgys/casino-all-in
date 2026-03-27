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
