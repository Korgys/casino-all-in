using System.Diagnostics.CodeAnalysis;

namespace casino.console.Games.Commons;

/// <summary>
/// Adapts <see cref="Console"/> operations for incremental frame rendering.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class SystemConsoleFrameTarget : IConsoleFrameTarget
{
    public bool SupportsCursorPositioning => !Console.IsOutputRedirected;

    public int? WindowWidth => TryGetDimension(static () => Console.WindowWidth);

    public int? WindowHeight => TryGetDimension(static () => Console.WindowHeight);

    public void Clear()
    {
        Console.Clear();
    }

    public void SetCursorPosition(int left, int top)
    {
        Console.SetCursorPosition(left, top);
    }

    public void Write(string value)
    {
        Console.Write(value);
    }

    public void WriteLine()
    {
        Console.WriteLine();
    }

    private static int? TryGetDimension(Func<int> getter)
    {
        try
        {
            return getter();
        }
        catch
        {
            return null;
        }
    }
}
