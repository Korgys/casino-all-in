using casino.core.Games.Slots;
using System.Diagnostics.CodeAnalysis;

namespace casino.console.Games.Slots;

[ExcludeFromCodeCoverage]
/// <summary>
/// Provides console input helpers for the slot machine game.
/// </summary>
public static class ConsoleSlotMachineInput
{
    /// <summary>
    /// Gets the current bet from user input.
    /// </summary>
    /// <param name="state">The current slot machine state.</param>
    /// <returns>The selected bet amount.</returns>
    public static int GetBet(SlotMachineGameState state)
    {
        while (true)
        {
            RenderBetPrompt(state);
            int bet = 1;
            Console.Write($"Votre mise : {bet}");

            return bet;
        }
    }

    /// <summary>
    /// Asks the player to continue with a new spin.
    /// </summary>
    /// <returns>Always <see langword="true"/>.</returns>
    public static bool AskContinueNewGame()
    {
        Console.Write("\nAppuyez sur une touche pour relancer la machine : ");
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
        Console.WriteLine("┌──────────── Machine à sous ─────────────┐");
        Console.WriteLine($"│ Crédits : {state.Credits,-29}│");
        Console.WriteLine($"│ Mise min/max : {state.MinBet} - {state.MaxBet,-21}│");
        Console.WriteLine("└─────────────────────────────────────────┘");
    }
}
