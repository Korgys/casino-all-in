namespace casino.console.Games.Commons;

/// <summary>
/// Stores previously rendered lines and updates only changed rows when cursor positioning is available.
/// </summary>
public sealed class ConsoleFrameBuffer
{
    private readonly IConsoleFrameTarget target;
    private List<string> previousLines = [];
    private bool hasRendered;
    private int? lastWindowWidth;
    private int? lastWindowHeight;

    public ConsoleFrameBuffer(IConsoleFrameTarget target)
    {
        this.target = target ?? throw new ArgumentNullException(nameof(target));
    }

    public void Render(IReadOnlyList<string> nextLines)
    {
        ArgumentNullException.ThrowIfNull(nextLines);

        if (!hasRendered)
        {
            DrawFullFrame(nextLines, clearBeforeDraw: true);
            return;
        }

        if (ShouldFallbackToFullRedraw())
        {
            DrawFullFrame(nextLines, clearBeforeDraw: true);
            return;
        }

        if (!target.SupportsCursorPositioning)
        {
            DrawFullFrame(nextLines, clearBeforeDraw: false);
            return;
        }

        try
        {
            DrawChangedLines(nextLines);
        }
        catch
        {
            DrawFullFrame(nextLines, clearBeforeDraw: false);
        }
    }

    private bool ShouldFallbackToFullRedraw()
    {
        if (!target.SupportsCursorPositioning)
            return false;

        var width = target.WindowWidth;
        var height = target.WindowHeight;

        if (lastWindowWidth is null || lastWindowHeight is null)
            return false;

        return width != lastWindowWidth || height != lastWindowHeight;
    }

    private void DrawChangedLines(IReadOnlyList<string> nextLines)
    {
        var maxLineCount = Math.Max(previousLines.Count, nextLines.Count);

        for (var index = 0; index < maxLineCount; index++)
        {
            var previous = index < previousLines.Count ? previousLines[index] : string.Empty;
            var next = index < nextLines.Count ? nextLines[index] : string.Empty;

            if (string.Equals(previous, next, StringComparison.Ordinal))
                continue;

            target.SetCursorPosition(0, index);
            target.Write(next.PadRight(Math.Max(previous.Length, next.Length)));
        }

        target.SetCursorPosition(0, nextLines.Count);
        SaveSnapshot(nextLines);
    }

    private void DrawFullFrame(IReadOnlyList<string> lines, bool clearBeforeDraw)
    {
        if (clearBeforeDraw)
            TryClear();

        if (target.SupportsCursorPositioning)
            target.SetCursorPosition(0, 0);

        for (var index = 0; index < lines.Count; index++)
        {
            target.Write(lines[index]);
            target.WriteLine();
        }

        SaveSnapshot(lines);
    }

    private void SaveSnapshot(IReadOnlyList<string> lines)
    {
        hasRendered = true;
        previousLines = [.. lines];
        lastWindowWidth = target.WindowWidth;
        lastWindowHeight = target.WindowHeight;
    }

    private void TryClear()
    {
        try
        {
            target.Clear();
        }
        catch
        {
            // Ignore clear failures in redirected output environments.
        }
    }
}
