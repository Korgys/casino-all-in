using System.Diagnostics.CodeAnalysis;
using casino.console.Games.Commons;
using casino.console.Localization;
using casino.core.Games.Slots;

namespace casino.console.Games.Slots;

/// <summary>
/// Provides console input helpers for the slot machine game.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ConsoleSlotMachineInput
{
    /// <summary>
    /// Gets the current bet from user input.
    /// </summary>
    /// <param name="state">The current slot machine state.</param>
    /// <returns>The selected bet amount.</returns>
    public static int GetBet(SlotMachineGameState state)
    {
        RenderBetPrompt(state);
        int bet = 1;
        Console.Write(ConsoleText.SlotBetPrompt(bet));

        return bet;
    }

    /// <summary>
    /// Asks the player to continue with a new spin.
    /// </summary>
    /// <returns>Always <see langword="true"/>.</returns>
    public static bool AskContinueNewGame()
    {
        Console.Write($"\n{ConsoleText.SlotContinuePrompt}");
        Console.ReadKey();
        return true;
    }

    /// <summary>
    /// Renders the bet prompt panel.
    /// </summary>
    /// <param name="state">The current slot machine state.</param>
    private static void RenderBetPrompt(SlotMachineGameState state)
    {
        Console.WriteLine();
        var frameWidth = ConsoleLayout.ResolveContentWidth(46);
        Console.WriteLine("┌" + new string('─', frameWidth) + "┐");
        ConsoleLayout.WriteFramedLine($" {ConsoleText.SlotPanelTitle} ", frameWidth, '│', '│');
        ConsoleLayout.WriteFramedLine($" {ConsoleText.SlotCredits}: {state.Credits} ", frameWidth, '│', '│');
        ConsoleLayout.WriteFramedLine($" {ConsoleText.SlotMinMaxBet}: {state.MinBet} - {state.MaxBet} ", frameWidth, '│', '│');
        Console.WriteLine("└" + new string('─', frameWidth) + "┘");
    }
}
