using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Common.Randoms;

public class CasinoRandom : IRandom
{
    private readonly Random _random;

    public CasinoRandom(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    public int Next(int maxExclusive) => _random.Next(maxExclusive);
}
