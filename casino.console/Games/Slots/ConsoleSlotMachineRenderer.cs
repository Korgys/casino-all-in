using System.Diagnostics.CodeAnalysis;
using casino.console.Games.Commons;
using casino.console.Localization;
using casino.core.Games.Slots;

namespace casino.console.Games.Slots;


/// <summary>
/// Renders slot machine game state in the console.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ConsoleSlotMachineRenderer
{
    private const int PreferredWidth = 46;
    private static ConsoleFrameBuffer FrameBuffer = new(new SystemConsoleFrameTarget());
    private static Action<int> _pause = Thread.Sleep;

    internal static void SetPauseForTests(Action<int> pause)
    {
        ArgumentNullException.ThrowIfNull(pause);
        _pause = pause;
    }

    internal static void ResetPause()
    {
        _pause = Thread.Sleep;
    }

    /// <summary>
    /// Renders the full slot machine table.
    /// </summary>
    /// <param name="state">The slot machine game state.</param>
    public static void RenderTable(SlotMachineGameState state)
    {
        var lines = BuildFrameLines(state);
        FrameBuffer.Render(lines);
    }

    private static IReadOnlyList<string> BuildFrameLines(SlotMachineGameState state)
    {
        return ConsoleOutputCapture.CaptureLines(() =>
        {
            var frameWidth = ConsoleLayout.ResolveContentWidth(PreferredWidth);
            RenderHeader(frameWidth);
            Console.WriteLine();
            RenderMachine(state, frameWidth);
            Console.WriteLine();
            RenderStatus(state);
            Console.WriteLine();
            RenderStats(state);

            if (state.IsRoundOver && state.LastPayout > 0)
                RenderWinAnimation(state.IsJackpot);
        });
    }

    /// <summary>
    /// Renders the slot machine header.
    /// </summary>
    /// <param name="frameWidth">The header frame content width.</param>
    private static void RenderHeader(int frameWidth)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Magenta))
            ConsoleLayout.WriteTopBorder(frameWidth);

        using (ConsoleColorScope.Foreground(ConsoleColor.Cyan))
            ConsoleLayout.WriteFramedLine($" {ConsoleText.SlotMachineHeader} ", frameWidth);

        using (ConsoleColorScope.Foreground(ConsoleColor.Magenta))
            ConsoleLayout.WriteBottomBorder(frameWidth);
    }

    /// <summary>
    /// Renders reels and current values.
    /// </summary>
    /// <param name="state">The slot machine game state.</param>
    /// <param name="frameWidth">The machine frame content width.</param>
    private static void RenderMachine(SlotMachineGameState state, int frameWidth)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.DarkYellow))
            ConsoleLayout.WriteTopBorder(frameWidth, '═');

        var reels = string.Join("  ", state.Reels.Select(symbol => FormatSymbol(symbol)));
        ConsoleLayout.WriteFramedLine($" {reels} ", frameWidth);

        using (ConsoleColorScope.Foreground(ConsoleColor.DarkYellow))
            ConsoleLayout.WriteBottomBorder(frameWidth, '═');

        using (ConsoleColorScope.Foreground(ConsoleColor.Yellow))
            Console.WriteLine($"{ConsoleText.SlotCredits}: {state.Credits}   {ConsoleText.CurrentBetLabel}: {state.CurrentBet}   {ConsoleText.SlotLastWin}: {state.LastPayout}");
    }

    /// <summary>
    /// Renders the round status message.
    /// </summary>
    /// <param name="state">The slot machine game state.</param>
    private static void RenderStatus(SlotMachineGameState state)
    {
        ConsoleColor color;
        if (state.IsJackpot)
            color = ConsoleColor.Yellow;
        else if (state.LastPayout > 0)
            color = ConsoleColor.Green;
        else if (state.IsSpinning)
            color = ConsoleColor.Cyan;
        else
            color = ConsoleColor.White;

        using (ConsoleColorScope.Foreground(color))
            Console.WriteLine(state.StatusMessage);
    }

    /// <summary>
    /// Renders cumulative slot statistics.
    /// </summary>
    /// <param name="state">The slot machine game state.</param>
    private static void RenderStats(SlotMachineGameState state)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Gray))
            Console.WriteLine($"{ConsoleText.SlotSpins}: {state.TotalSpins}  |  {ConsoleText.SlotWinningSpins}: {state.WinningSpins}  |  {ConsoleText.SlotBestWin}: {state.BiggestPayout}");
    }

    /// <summary>
    /// Formats one slot symbol for inline machine rendering.
    /// </summary>
    /// <param name="symbol">The symbol to format.</param>
    /// <returns>The formatted symbol string wrapped in brackets.</returns>
    private static string FormatSymbol(SlotSymbol symbol)
    {
        return symbol switch
        {
            SlotSymbol.Cherry => "[🍒]",
            SlotSymbol.Lemon => "[🍋]",
            SlotSymbol.Bell => "[🔔]",
            SlotSymbol.Diamond => "[💎]",
            SlotSymbol.Star => "[⭐]",
            SlotSymbol.Seven => "[7]",
            SlotSymbol.Bar => "[💲]",
            _ => "[?]"
        };
    }

    /// <summary>
    /// Renders a short win animation.
    /// </summary>
    /// <param name="jackpot">Indicates whether the win is a jackpot.</param>
    private static void RenderWinAnimation(bool jackpot)
    {
        ConsoleColor[] frames;
        string message;
        if (jackpot)
        {
            frames = [ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Cyan];
            message = ConsoleText.SlotJackpotAnimation;
        }
        else
        {
            frames = [ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Cyan];
            message = ConsoleText.SlotWinAnimation;
        }

        foreach (var color in frames)
        {
            using (ConsoleColorScope.Foreground(color))
                Console.WriteLine(message);

            _pause(50);
        }
    }
}
