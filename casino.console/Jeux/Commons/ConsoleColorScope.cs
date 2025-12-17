using System;
using System.Collections.Generic;
using System.Text;

namespace casino.console.Jeux.Commons;

public readonly struct ConsoleColorScope : IDisposable
{
    private readonly ConsoleColor _old;

    private ConsoleColorScope(ConsoleColor old) => _old = old;

    public static ConsoleColorScope Foreground(ConsoleColor color)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        return new ConsoleColorScope(old);
    }

    public void Dispose() => Console.ForegroundColor = _old;
}
