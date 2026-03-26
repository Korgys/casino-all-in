using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace casino.console.Games.Commons;

/// <summary>
/// Provides shared helpers to render framed console content with safe width handling.
/// </summary>
public static class ConsoleLayout
{
    private const int BorderOverhead = 2;
    private const int MinContentWidth = 20;
    private const string Ellipsis = "…";

    /// <summary>
    /// Resolves the effective content width for a frame from a preferred width and the current console size.
    /// </summary>
    /// <param name="preferredContentWidth">The target content width before clamping.</param>
    /// <returns>A width clamped to safe console limits.</returns>
    public static int ResolveContentWidth(int preferredContentWidth)
    {
        var consoleWidth = TryGetConsoleWindowWidth();
        var maxContentWidth = Math.Max(MinContentWidth, consoleWidth - BorderOverhead);
        return Math.Clamp(preferredContentWidth, MinContentWidth, maxContentWidth);
    }

    /// <summary>
    /// Writes a top border line for a frame.
    /// </summary>
    /// <param name="contentWidth">The inner content width.</param>
    /// <param name="horizontal">The horizontal border character.</param>
    public static void WriteTopBorder(int contentWidth, char horizontal = '═')
        => Console.WriteLine($"╔{new string(horizontal, contentWidth)}╗");

    /// <summary>
    /// Writes a separator line for a frame.
    /// </summary>
    /// <param name="contentWidth">The inner content width.</param>
    /// <param name="horizontal">The horizontal border character.</param>
    public static void WriteSeparator(int contentWidth, char horizontal = '═')
        => Console.WriteLine($"╠{new string(horizontal, contentWidth)}╣");

    /// <summary>
    /// Writes a bottom border line for a frame.
    /// </summary>
    /// <param name="contentWidth">The inner content width.</param>
    /// <param name="horizontal">The horizontal border character.</param>
    public static void WriteBottomBorder(int contentWidth, char horizontal = '═')
        => Console.WriteLine($"╚{new string(horizontal, contentWidth)}╝");

    /// <summary>
    /// Writes one framed content line with truncation and right-side padding when needed.
    /// </summary>
    /// <param name="content">The content to render inside the frame.</param>
    /// <param name="contentWidth">The frame content width.</param>
    /// <param name="left">The left border character.</param>
    /// <param name="right">The right border character.</param>
    public static void WriteFramedLine(string content, int contentWidth, char left = '║', char right = '║')
    {
        var fitted = FitToWidth(content, contentWidth);
        var padding = Math.Max(0, contentWidth - GetVisibleLength(fitted));
        Console.Write(left);
        Console.Write(fitted);
        Console.Write(new string(' ', padding));
        Console.WriteLine(right);
    }

    /// <summary>
    /// Truncates a string to a visible width while preserving ANSI escape sequences.
    /// </summary>
    /// <param name="text">The source text.</param>
    /// <param name="width">The maximum visible width.</param>
    /// <returns>The original text or a truncated version with ellipsis.</returns>
    public static string FitToWidth(string text, int width)
    {
        if (width <= 0)
            return string.Empty;

        if (GetVisibleLength(text) <= width)
            return text;

        var visibleBudget = Math.Max(0, width - GetVisibleLength(Ellipsis));
        var builder = new StringBuilder();
        var index = 0;

        while (index < text.Length && visibleBudget > 0)
        {
            if (IsAnsiEscapeSequence(text, index, out var ansiLength))
            {
                builder.Append(text, index, ansiLength);
                index += ansiLength;
                continue;
            }

            var rune = Rune.GetRuneAt(text, index);
            var runeWidth = GetRuneDisplayWidth(rune);

            if (runeWidth > visibleBudget)
                break;

            builder.Append(text, index, rune.Utf16SequenceLength);
            index += rune.Utf16SequenceLength;
            visibleBudget -= runeWidth;
        }

        builder.Append(Ellipsis);
        return builder.ToString();
    }

    /// <summary>
    /// Computes the visible text length by ignoring ANSI color sequences and accounting for double-width glyphs.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <returns>The visible display width.</returns>
    public static int GetVisibleLength(string text)
    {
        var plain = Regex.Replace(text, @"\x1B\[[0-9;]*m", string.Empty);
        var width = 0;

        foreach (var rune in plain.EnumerateRunes())
            width += GetRuneDisplayWidth(rune);

        return width;
    }

    /// <summary>
    /// Returns the current console window width with a safe fallback when unavailable.
    /// </summary>
    /// <returns>The detected or fallback console width.</returns>
    private static int TryGetConsoleWindowWidth()
    {
        try
        {
            return Console.WindowWidth > 0 ? Console.WindowWidth : 120;
        }
        catch
        {
            return 120;
        }
    }

    /// <summary>
    /// Checks whether an ANSI escape sequence starts at a specific index.
    /// </summary>
    /// <param name="text">The source text.</param>
    /// <param name="startIndex">The index to inspect.</param>
    /// <param name="length">The detected ANSI sequence length when found.</param>
    /// <returns><c>true</c> when a complete ANSI sequence is found; otherwise <c>false</c>.</returns>
    private static bool IsAnsiEscapeSequence(string text, int startIndex, out int length)
    {
        length = 0;

        if (startIndex >= text.Length || text[startIndex] != '\u001b')
            return false;

        var next = startIndex + 1;
        if (next >= text.Length || text[next] != '[')
            return false;

        var end = next + 1;
        while (end < text.Length && text[end] != 'm')
            end++;

        if (end >= text.Length)
            return false;

        length = end - startIndex + 1;
        return true;
    }

    /// <summary>
    /// Estimates the display width of a rune for monospace console rendering.
    /// </summary>
    /// <param name="rune">The rune to evaluate.</param>
    /// <returns>0 for combining/control, 1 for narrow glyphs, and 2 for wide glyphs (emoji/CJK).</returns>
    private static int GetRuneDisplayWidth(Rune rune)
    {
        if (Rune.IsControl(rune))
            return 0;

        var category = Rune.GetUnicodeCategory(rune);
        if (category is UnicodeCategory.NonSpacingMark or UnicodeCategory.EnclosingMark or UnicodeCategory.Format)
            return 0;

        var value = rune.Value;

        // Heuristic wide ranges (CJK + emoji blocks commonly rendered as double-width in terminals)
        if (
            (value >= 0x1100 && value <= 0x115F) ||
            (value >= 0x2E80 && value <= 0xA4CF) ||
            (value >= 0xAC00 && value <= 0xD7A3) ||
            (value >= 0xF900 && value <= 0xFAFF) ||
            (value >= 0xFE10 && value <= 0xFE19) ||
            (value >= 0xFE30 && value <= 0xFE6F) ||
            (value >= 0xFF00 && value <= 0xFF60) ||
            (value >= 0xFFE0 && value <= 0xFFE6) ||
            (value >= 0x1F300 && value <= 0x1FAFF))
        {
            return 2;
        }

        return 1;
    }
}
