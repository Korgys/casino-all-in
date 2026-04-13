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
    private const int InvalidInputHintThreshold = 2;

    /// <summary>
    /// Gets the current bet from user input.
    /// </summary>
    /// <param name="state">The current slot machine state.</param>
    /// <returns>The selected bet amount.</returns>
    public static int GetBet(SlotMachineGameState state)
    {
        RenderBetPrompt(state);
        var invalidAttempts = 0;

        while (true)
        {
            Console.Write(ConsoleText.SlotBetPrompt(state.MinBet));
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input))
                return state.MinBet;

            if (ConsoleInputAliases.IsBack(input))
            {
                Console.WriteLine(ConsoleText.InputCanceledUsingDefault(state.MinBet.ToString()));
                return state.MinBet;
            }

            if (int.TryParse(input, out var bet) && bet >= state.MinBet && bet <= state.MaxBet)
                return bet;

            invalidAttempts++;
            Console.WriteLine(int.TryParse(input, out _) ? ConsoleText.RangeError(state.MinBet, state.MaxBet) : ConsoleText.InvalidNumberInput);
            WriteNumberMenuHintIfNeeded(invalidAttempts);
        }
    }

    private static void WriteNumberMenuHintIfNeeded(int invalidAttempts)
    {
        if (invalidAttempts < InvalidInputHintThreshold)
            return;

        Console.WriteLine(ConsoleText.TypeNumberMenuHelp);
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
