using casino.console.Games;
using casino.console.Games.Poker;
using System;
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

        Console.WriteLine("=== Casino All-In ===\n");

        var factory = new ConsoleGameFactory();
        var game = factory.Create("poker", ConsolePokerInput.GetPlayerAction, ConsolePokerInput.AskContinueNewGame);

        if (game is null) return;

        game.StateUpdated += (_, e) =>
        {
            if (e.State is casino.core.Games.Poker.PokerGameState state)
            {
                PokerRenderer.RenderTable(state);
            }
        };

        game.Run();

        Console.WriteLine("\nAppuyez sur une touche pour quitter...");
        Console.ReadKey(intercept: true);
    }
}
