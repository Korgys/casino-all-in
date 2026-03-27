using System.Diagnostics.CodeAnalysis;
using casino.core.Games.Poker;

namespace casino.console.Games.Poker;

[ExcludeFromCodeCoverage]
public class ConsoleWaitStrategy : IWaitStrategy
{
    public void Wait()
    {
        Thread.Sleep(Random.Shared.Next(500, 1500));
    }
}
