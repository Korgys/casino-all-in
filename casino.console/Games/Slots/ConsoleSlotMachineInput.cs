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
        return ConsolePromptReader.ReadIntInRange(
            ConsoleText.SlotBetPrompt(state.MinBet),
            state.MinBet,
            state.MaxBet,
            state.MinBet);
    }

    /// <summary>
    /// Asks the player to continue with a new spin.
    /// </summary>
    /// <returns><see langword="true"/> when the player wants to continue; otherwise <see langword="false"/>.</returns>
    public static bool AskContinueNewGame()
    {
        Console.Write($"\n{ConsoleText.SlotContinuePrompt}");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        return ConsoleInputAliases.IsYes(answer);
    }

    /// <summary>
    /// Renders the bet prompt panel.
    /// </summary>
    /// <param name="state">The current slot machine state.</param>
    private static void RenderBetPrompt(SlotMachineGameState state)
    {
        Console.WriteLine();
        var frameWidth = ConsoleLayout.ResolveContentWidth(46);
        ConsoleLayout.WriteTopBorder(frameWidth);
        ConsoleLayout.WriteFramedLine($" {ConsoleText.SlotPanelTitle} ", frameWidth, '│', '│');
        ConsoleLayout.WriteFramedLine($" {ConsoleText.SlotCredits}: {state.Credits} ", frameWidth, '│', '│');
        ConsoleLayout.WriteFramedLine($" {ConsoleText.SlotMinMaxBet}: {state.MinBet} - {state.MaxBet} ", frameWidth, '│', '│');
        ConsoleLayout.WriteBottomBorder(frameWidth);
    }
}
