using casino.console.Games.Commons;
using casino.core.Games.Slots;
using System.Diagnostics.CodeAnalysis;
using casino.console.Localization;

namespace casino.console.Games.Slots;

[ExcludeFromCodeCoverage]
/// <summary>
/// Renders slot machine game state in the console.
/// </summary>
public static class ConsoleSlotMachineRenderer
{
    private static Action<int> _pause = Thread.Sleep;

    /// <summary>
    /// Renders the full slot machine table.
    /// </summary>
    /// <param name="state">The slot machine game state.</param>
    public static void RenderTable(SlotMachineGameState state)
    {
        try
        {
            Console.Clear();
        }
        catch
        {
            // Ignore console clearing issues in redirected test environments.
        }
        RenderHeader();
        Console.WriteLine();
        RenderMachine(state);
        Console.WriteLine();
        RenderStatus(state);
        Console.WriteLine();
        RenderStats(state);

        if (state.IsRoundOver && state.LastPayout > 0)
            RenderWinAnimation(state.IsJackpot);
    }

    /// <summary>
    /// Renders the slot machine header.
    /// </summary>
    private static void RenderHeader()
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Magenta))
            Console.WriteLine("╔══════════════════════════════════════════════╗");

        using (ConsoleColorScope.Foreground(ConsoleColor.Cyan))
            Console.WriteLine("║          SLOT MACHINE ✨ NEON JACKPOT        ║");

        using (ConsoleColorScope.Foreground(ConsoleColor.Magenta))
            Console.WriteLine("╚══════════════════════════════════════════════╝");
    }

    /// <summary>
    /// Renders reels and current values.
    /// </summary>
    /// <param name="state">The slot machine game state.</param>
    private static void RenderMachine(SlotMachineGameState state)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.DarkYellow))
            Console.WriteLine("╔═══════════════[ PAYLINE ]═══════════════════╗");

        Console.Write("║              ");
        for (var index = 0; index < state.Reels.Count; index++)
        {
            WriteSymbol(state.Reels[index], state.IsSpinning);
            if (index < state.Reels.Count - 1)
                Console.Write("  ");
        }

        Console.WriteLine("               ║");

        using (ConsoleColorScope.Foreground(ConsoleColor.DarkYellow))
            Console.WriteLine("╚═════════════════════════════════════════════╝");

        using (ConsoleColorScope.Foreground(ConsoleColor.Yellow))
            Console.WriteLine($"{ConsoleText.SlotCredits}: {state.Credits}   {ConsoleText.BetLabel}: {state.CurrentBet}   {ConsoleText.SlotLastWin}: {state.LastPayout}");
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
    /// Writes one symbol with style based on state.
    /// </summary>
    /// <param name="symbol">The symbol to write.</param>
    /// <param name="isSpinning">Indicates whether the reels are spinning.</param>
    private static void WriteSymbol(SlotSymbol symbol, bool isSpinning)
    {
        var (glyph, color) = symbol switch
        {
            SlotSymbol.Cherry => ("🍒", ConsoleColor.Red),
            SlotSymbol.Lemon => ("🍋", ConsoleColor.Yellow),
            SlotSymbol.Bell => ("🔔", ConsoleColor.DarkYellow),
            SlotSymbol.Diamond => ("💎", ConsoleColor.Cyan),
            SlotSymbol.Star => ("⭐", ConsoleColor.Magenta),
            SlotSymbol.Seven => ("7", ConsoleColor.Green),
            SlotSymbol.Bar => ("💲", ConsoleColor.Blue),
            _ => ("?", ConsoleColor.White)
        };

        using (ConsoleColorScope.Foreground(isSpinning ? ConsoleColor.White : color))
            Console.Write($"[{glyph}]");
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
