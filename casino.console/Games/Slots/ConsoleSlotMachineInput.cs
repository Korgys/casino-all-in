using casino.core.Games.Slots;

namespace casino.console.Games.Slots;

public static class ConsoleSlotMachineInput
{
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

    public static bool AskContinueNewGame()
    {
        Console.Write("\nAppuyez sur une touche pour relancer la machine : ");
        Console.ReadKey();
        return true;
    }

    private static void RenderBetPrompt(SlotMachineGameState state)
    {
        Console.WriteLine();
        Console.WriteLine("┌──────────── Machine à sous ─────────────┐");
        Console.WriteLine($"│ Crédits : {state.Credits,-29}│");
        Console.WriteLine($"│ Mise min/max : {state.MinBet} - {state.MaxBet,-21}│");
        Console.WriteLine("└─────────────────────────────────────────┘");
    }
}
