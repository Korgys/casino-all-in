namespace casino.console.Games.Commons;

/// <summary>
/// Captures console output produced by a render action and returns normalized lines.
/// </summary>
public static class ConsoleOutputCapture
{
    /// <summary>
    /// Captures all text written to <see cref="Console.Out"/> while running <paramref name="render"/>.
    /// </summary>
    /// <param name="render">The render action to execute.</param>
    /// <returns>Rendered output split into lines without a trailing empty line.</returns>
    public static List<string> CaptureLines(Action render)
    {
        ArgumentNullException.ThrowIfNull(render);

        var originalOut = Console.Out;
        var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);
            render();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        return SplitLines(writer.ToString());
    }

    private static List<string> SplitLines(string text)
    {
        var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
        if (lines.Count > 0 && lines[^1].Length == 0)
            lines.RemoveAt(lines.Count - 1);

        return lines;
    }
}
