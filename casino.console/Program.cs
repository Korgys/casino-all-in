using casino.console.Games;
using casino.console.Games.Blackjack;
using casino.console.Games.Poker;
using casino.core.Games.Blackjack;
using System.Text;

namespace casino.console;

/// <summary>
/// Provides the entry point for the Casino All-In console application and initializes the poker game experience.
/// </summary>
public static class Program
{
    private static readonly ConsolePokerRenderer PokerRenderer = new();

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        var factory = new ConsoleGameFactory();
        var game = BuildGame(factory);

        if (game is null) return;

        game.StateUpdated += (_, e) =>
        {
            if (e.State is casino.core.Games.Poker.PokerGameState state)
            {
                PokerRenderer.RenderTable(state);
            }

            if (e.State is BlackjackGameState blackjackState)
            {
                ConsoleBlackjackRenderer.RenderTable(blackjackState);
            }
        };

        game.Run();

        Console.WriteLine("\nAppuyez sur une touche pour quitter...");
        Console.ReadKey(intercept: true);
    }

    private static casino.core.IGame? BuildGame(ConsoleGameFactory factory)
    {
        Console.Clear();
        RenderMainMenu();
        Console.Write("Votre choix: ");

        var choice = (Console.ReadLine() ?? string.Empty).Trim();

        return choice.ToLowerInvariant() switch
        {
            "1" or "poker" => factory.CreatePoker(
                ConsolePokerInput.GetPlayerAction,
                ConsolePokerInput.AskContinueNewGame,
                ConsolePokerInput.PromptGameSetup()),
            "2" or "blackjack" => factory.CreateBlackjack(ConsoleBlackjackInput.GetPlayerAction, ConsoleBlackjackInput.AskContinueNewGame),
            _ => null
        };
    }

    private static void RenderMainMenu()
    {
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║               CASINO ALL-IN                  ║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine("║  1. Poker                                    ║");
        Console.WriteLine("║  2. BlackJack                                ║");
        Console.WriteLine("║  3. Quitter                                  ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();
    }
}
