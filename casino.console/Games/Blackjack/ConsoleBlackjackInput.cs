using casino.core.Games.Blackjack;

namespace casino.console.Games.Blackjack;

public static class ConsoleBlackjackInput
{
    public static BlackjackAction GetPlayerAction(BlackjackGameState _)
    {
        while (true)
        {
            RenderAvailableActions();
            Console.Write("Quel est votre choix ? ");
            var choice = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

            if (choice is "1" or "h" or "hit")
                return BlackjackAction.Hit;

            if (choice is "2" or "s" or "stand")
                return BlackjackAction.Stand;
        }
    }

    public static bool AskContinueNewGame()
    {
        Console.Write("\nRejouer une manche de blackjack ? (o/n) : ");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        return answer is "o" or "oui" or "y" or "yes";
    }

    private static void RenderAvailableActions()
    {
        Console.WriteLine();
        Console.WriteLine("┌──────────── Actions disponibles ────────────┐");
        Console.WriteLine("│ 1. Tirer une carte (Hit)                    │");
        Console.WriteLine("│ 2. Rester (Stand)                           │");
        Console.WriteLine("└─────────────────────────────────────────────┘");
    }
}