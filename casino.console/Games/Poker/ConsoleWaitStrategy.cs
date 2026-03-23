using casino.core.Games.Poker;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace casino.console.Games.Poker;

[ExcludeFromCodeCoverage]
public class ConsoleWaitStrategy : IWaitStrategy
{
    public void Wait()
    {
        Thread.Sleep(Random.Shared.Next(500, 1500));
    }
}
