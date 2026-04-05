using casino.console.Games.Commons;

namespace casino.console.tests.Games.Commons;

internal sealed class FakeConsoleFrameTarget : IConsoleFrameTarget
{
    public bool SupportsCursorPositioning { get; set; } = true;
    public int? WindowWidth { get; set; } = 120;
    public int? WindowHeight { get; set; } = 40;

    public int CursorLeft { get; private set; }
    public int CursorTop { get; private set; }

    public int ClearCalls { get; private set; }
    public List<(int Left, int Top)> CursorMoves { get; } = [];
    public List<string> Lines { get; } = [];

    public void Clear()
    {
        ClearCalls++;
        Lines.Clear();
        CursorLeft = 0;
        CursorTop = 0;
    }

    public void SetCursorPosition(int left, int top)
    {
        CursorLeft = left;
        CursorTop = top;
        CursorMoves.Add((left, top));
    }

    public void Write(string value)
    {
        while (Lines.Count <= CursorTop)
            Lines.Add(string.Empty);

        var existing = Lines[CursorTop];
        var prefix = existing.Length >= CursorLeft
            ? existing[..CursorLeft]
            : existing.PadRight(CursorLeft);

        var merged = prefix + value;
        if (existing.Length > merged.Length)
            merged += existing[merged.Length..];

        Lines[CursorTop] = merged;
        CursorLeft += value.Length;
    }

    public void WriteLine()
    {
        CursorTop++;
        CursorLeft = 0;
    }
}
