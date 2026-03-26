using casino.console.Games.Commons;
using casino.core.Games.Slots;
using System.Diagnostics.CodeAnalysis;

namespace casino.console.Games.Slots;

[ExcludeFromCodeCoverage]
public static class ConsoleSlotMachineRenderer
{
    private static Action<int> _pause = Thread.Sleep;

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

    private static void RenderHeader()
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Magenta))
            Console.WriteLine("╔══════════════════════════════════════════════╗");

        using (ConsoleColorScope.Foreground(ConsoleColor.Cyan))
            Console.WriteLine("║          SLOT MACHINE ✨ NEON JACKPOT        ║");

        using (ConsoleColorScope.Foreground(ConsoleColor.Magenta))
            Console.WriteLine("╚══════════════════════════════════════════════╝");
    }

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
            Console.WriteLine($"Crédits: {state.Credits}   Mise: {state.CurrentBet}   Dernier gain: {state.LastPayout}");
    }

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

    private static void RenderStats(SlotMachineGameState state)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Gray))
            Console.WriteLine($"Tours: {state.TotalSpins}  |  Tours gagnants: {state.WinningSpins}  |  Meilleur gain: {state.BiggestPayout}");
    }

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

    private static void RenderWinAnimation(bool jackpot)
    {
        ConsoleColor[] frames;
        string message;
        if (jackpot)
        {
            frames = [ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Cyan];
            message = "💥 JACKPOT NEON 💥";
        }
        else
        {
            frames = [ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Cyan];
            message = "✨ GAIN ! ✨";
        }

        foreach (var color in frames)
        {
            using (ConsoleColorScope.Foreground(color))
                Console.WriteLine(message);

            _pause(50);
        }
    }
}
