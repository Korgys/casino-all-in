using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Common.Utils;

public interface IRandom
{
    int Next(int maxExclusive);
}

public class CasinoRandom : IRandom
{
    private readonly Random _random = Random.Shared;
    public int Next(int maxExclusive) => _random.Next(maxExclusive);
}
