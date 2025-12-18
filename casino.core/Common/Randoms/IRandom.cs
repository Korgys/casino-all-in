using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Common.Randoms;

public interface IRandom
{
    int Next(int maxExclusive);
}
