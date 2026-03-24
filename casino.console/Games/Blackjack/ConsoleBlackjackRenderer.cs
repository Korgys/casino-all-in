using casino.console.Games.Commons;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker.Cards;

namespace casino.console.Games.Blackjack;

public static class ConsoleBlackjackRenderer
{
    private static Action<int> _pause = Thread.Sleep;

    internal static void SetPauseForTests(Action<int> pause)
    {
        ArgumentNullException.ThrowIfNull(pause);
        _pause = pause;
    }

    internal static void ResetPause()
    {
        _pause = Thread.Sleep;
    }

    public static void RenderTable(BlackjackGameState state)
    {
        try
        {
            Console.Clear();
        }
        catch
        {
            // Ignore any exceptions from Console.Clear (e.g., if the console is not available)
        }


        RenderHeader();
        Console.WriteLine();

        RenderHand("Croupier", state.DealerCards, state.IsDealerHoleCardHidden);
        RenderHand("Vous", state.PlayerCards, hideHoleCard: false);

        Console.WriteLine();
        RenderStatus(state);
        Console.WriteLine();
        RenderStats(state);

        if (state.IsRoundOver && state.RoundOutcome == BlackjackRoundOutcome.PlayerWin)
            RenderWinAnimation();
    }

    private static void RenderHeader()
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Yellow))
            Console.WriteLine("╔══════════════════════════════════════════════╗");

        using (ConsoleColorScope.Foreground(ConsoleColor.Green))
            Console.WriteLine("║                BLACKJACK ♠♥                  ║");

        using (ConsoleColorScope.Foreground(ConsoleColor.Yellow))
            Console.WriteLine("╚══════════════════════════════════════════════╝");
    }

    private static void RenderHand(string label, IReadOnlyList<Card> cards, bool hideHoleCard)
    {
        using (ConsoleColorScope.Foreground(label == "Vous" ? ConsoleColor.Cyan : ConsoleColor.Magenta))
            Console.Write($"{label,-9}: ");

        for (var index = 0; index < cards.Count; index++)
        {
            if (index > 0)
                Console.Write(' ');

            WriteCard(cards[index]);
        }

        var total = hideHoleCard && cards.Count > 0
            ? BlackjackScoreCalculator.Calculate([cards[0]])
            : BlackjackScoreCalculator.Calculate(cards);

        using (ConsoleColorScope.Foreground(ConsoleColor.White))
            Console.Write($"  (Total: {total})");

        Console.WriteLine();
    }

    private static void RenderStatus(BlackjackGameState state)
    {
        var color = state.RoundOutcome switch
        {
            BlackjackRoundOutcome.PlayerWin => ConsoleColor.Green,
            BlackjackRoundOutcome.DealerWin => ConsoleColor.Red,
            BlackjackRoundOutcome.Push => ConsoleColor.Yellow,
            _ => ConsoleColor.White
        };

        using (ConsoleColorScope.Foreground(color))
            Console.WriteLine(state.StatusMessage);
    }

    private static void RenderStats(BlackjackGameState state)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Yellow))
            Console.Write("Stats");

        Console.Write("  |  ");

        using (ConsoleColorScope.Foreground(ConsoleColor.Green))
            Console.Write($"Victoires: {state.PlayerWins}");

        Console.Write("  |  ");

        using (ConsoleColorScope.Foreground(ConsoleColor.Red))
            Console.Write($"Défaites: {state.DealerWins}");

        Console.Write("  |  ");

        using (ConsoleColorScope.Foreground(ConsoleColor.DarkYellow))
            Console.Write($"Égalités: {state.Pushes}");

        Console.WriteLine();
    }

    private static void WriteCard(Card card)
    {
        var color = card.Suit is Suit.Hearts or Suit.Diamonds
            ? ConsoleColor.Red
            : ConsoleColor.Cyan;

        using (ConsoleColorScope.Foreground(color))
            Console.Write(card);
    }

    private static void RenderWinAnimation()
    {
        var colors = new[] { ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Cyan };
        const string message = "✨ BLACKJACK ! Vous gagnez ! ✨";

        foreach (var color in colors)
        {
            using (ConsoleColorScope.Foreground(color))
                Console.WriteLine(message);

            _pause(60);
        }
    }
}
