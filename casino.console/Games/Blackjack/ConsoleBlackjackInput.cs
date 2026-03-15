using casino.core.Games.Blackjack;

namespace casino.console.Games.Blackjack;

public static class ConsoleBlackjackInput
{
    public static BlackjackAction GetPlayerAction(BlackjackGameState _)
    {
        while (true)
        {
            Console.Write("Action (h=hit / s=stand): ");
            var choice = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

            if (choice is "h" or "hit")
                return BlackjackAction.Hit;

            if (choice is "s" or "stand")
                return BlackjackAction.Stand;
        }
    }

    public static bool AskContinueNewGame()
    {
        Console.Write("\nRejouer une manche de blackjack ? (o/n) : ");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        return answer is "o" or "oui" or "y" or "yes";
    }
}
