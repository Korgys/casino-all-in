using casino.console.Games.Commons;
using casino.console.Localization;
using casino.core.Games.Roulette;

namespace casino.console.Games.Roulette;

public static class ConsoleRouletteRenderer
{
    private const int PreferredWidth = 56;
    private static readonly HashSet<int> RedNumbers =
    [
        1, 3, 5, 7, 9,
        12, 14, 16, 18,
        19, 21, 23, 25, 27,
        30, 32, 34, 36
    ];

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

    public static void RenderTable(RouletteGameState state)
    {
        var lines = BuildFrameLines(state);
        FrameBuffer.Render(lines);
    }

    private static IReadOnlyList<string> BuildFrameLines(RouletteGameState state)
    {
        return ConsoleOutputCapture.CaptureLines(() =>
        {
            var frameWidth = ConsoleLayout.ResolveContentWidth(PreferredWidth);
            RenderHeader(frameWidth);
            Console.WriteLine();
            RenderWheel(state, frameWidth);
            Console.WriteLine();
            RenderStatus(state);
            Console.WriteLine();
            RenderStats(state);

            if (state.IsRoundOver && state.LastPayout > 0)
                RenderWinAnimation();
        });
    }

    private static void RenderHeader(int frameWidth)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.DarkYellow))
            ConsoleLayout.WriteTopBorder(frameWidth);

        using (ConsoleColorScope.Foreground(ConsoleColor.White))
            ConsoleLayout.WriteFramedLine($" {ConsoleText.RouletteHeader} ", frameWidth);

        using (ConsoleColorScope.Foreground(ConsoleColor.DarkYellow))
            ConsoleLayout.WriteBottomBorder(frameWidth);
    }

    private static void RenderWheel(RouletteGameState state, int frameWidth)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.DarkGreen))
            ConsoleLayout.WriteTopBorder(frameWidth, '=');

        using (ConsoleColorScope.Foreground(GetPocketColor(state.CurrentPocket)))
            ConsoleLayout.WriteFramedLine($" {ConsoleText.RoulettePocketLabel}: {FormatPocket(state.CurrentPocket)} ", frameWidth);

        ConsoleLayout.WriteFramedLine($" {ConsoleText.BetLabel}: {state.BetSummary} ", frameWidth);

        using (ConsoleColorScope.Foreground(ConsoleColor.DarkGreen))
            ConsoleLayout.WriteBottomBorder(frameWidth, '=');

        using (ConsoleColorScope.Foreground(ConsoleColor.Yellow))
            Console.WriteLine($"{ConsoleText.RouletteCredits}: {state.Credits}   {ConsoleText.CurrentBetLabel}: {state.CurrentBet}   {ConsoleText.RouletteLastWin}: {state.LastPayout}");
    }

    private static void RenderStatus(RouletteGameState state)
    {
        var color = ConsoleColor.White;

        if (state.IsWinningBet)
            color = ConsoleColor.Green;
        else if (state.IsSpinning)
            color = ConsoleColor.Cyan;

        using (ConsoleColorScope.Foreground(color))
            Console.WriteLine(state.StatusMessage);
    }

    private static void RenderStats(RouletteGameState state)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Gray))
            Console.WriteLine($"{ConsoleText.RouletteSpins}: {state.TotalSpins}  |  {ConsoleText.RouletteWinningSpins}: {state.WinningSpins}  |  {ConsoleText.RouletteBestWin}: {state.BiggestPayout}");
    }

    private static string FormatPocket(int? pocket)
    {
        if (!pocket.HasValue)
            return "--";

        if (pocket.Value == 0)
            return "0 GREEN";

        return RedNumbers.Contains(pocket.Value)
            ? $"{pocket.Value} RED"
            : $"{pocket.Value} BLACK";
    }

    private static ConsoleColor GetPocketColor(int? pocket)
    {
        if (!pocket.HasValue)
            return ConsoleColor.White;

        if (pocket.Value == 0)
            return ConsoleColor.Green;

        return RedNumbers.Contains(pocket.Value) ? ConsoleColor.Red : ConsoleColor.Gray;
    }

    private static void RenderWinAnimation()
    {
        foreach (var color in new[] { ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan })
        {
            using (ConsoleColorScope.Foreground(color))
                Console.WriteLine(ConsoleText.RouletteWinAnimation);

            _pause(50);
        }
    }
}


