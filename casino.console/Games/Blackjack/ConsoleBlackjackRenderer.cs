using casino.core.Games.Blackjack;

namespace casino.console.Games.Blackjack;

public static class ConsoleBlackjackRenderer
{
    public static void RenderTable(BlackjackGameState state)
    {
        Console.Clear();

        Console.WriteLine("=== Blackjack ===\n");

        var dealerCards = state.IsDealerHoleCardHidden && state.DealerCards.Count > 1
            ? $"{state.DealerCards[0]} ??"
            : string.Join(" ", state.DealerCards);

        Console.WriteLine($"Croupier : {dealerCards}");
        Console.WriteLine($"Vous     : {string.Join(" ", state.PlayerCards)} (Total: {BlackjackScoreCalculator.Calculate(state.PlayerCards)})");

        if (!state.IsDealerHoleCardHidden)
            Console.WriteLine($"Total croupier: {BlackjackScoreCalculator.Calculate(state.DealerCards)}");

        Console.WriteLine();
        Console.WriteLine(state.StatusMessage);
    }
}
