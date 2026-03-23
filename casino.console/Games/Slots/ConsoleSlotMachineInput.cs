using casino.core.Games.Slots;

namespace casino.console.Games.Slots;

public static class ConsoleSlotMachineInput
{
    public static int GetBet(SlotMachineGameState state)
    {
        while (true)
        {
            RenderBetPrompt(state);
            Console.Write("Votre mise : ");
            var rawInput = (Console.ReadLine() ?? string.Empty).Trim();

            if (int.TryParse(rawInput, out var bet) && bet >= state.MinBet && bet <= state.MaxBet)
                return bet;
        }
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
