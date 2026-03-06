using System.Threading;
using casino.core.Games.Poker;

namespace casino.console.Games.Poker;

public class ConsoleWaitStrategy : IWaitStrategy
{
    public void Wait()
    {
        Thread.Sleep(Random.Shared.Next(500, 1500));
    }
}
