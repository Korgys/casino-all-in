using casino.console.Games.Poker;
using casino.console.Games;
using casino.core;
using casino.core.Common.Events;
using casino.core.Games.Poker;
using System;
using System.Text;

namespace casino.console;

/// <summary>
/// Provides the entry point for the Casino All-In console application and initializes the poker game experience.
/// </summary>
public static class Program
{
    private static PokerGameState? _lastState;

    public static void Main(string[] args)
    {
        // Configure console encoding to support UTF-8 characters (e.g., card suits)
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Casino All-In ===\n");

        var factory = new ConsoleGameFactory();

        // Create a poker game instance, passing the input handlers for player actions and game continuation
        var game = factory.Create("poker", ConsolePokerInput.GetPlayerAction, ConsolePokerInput.AskContinueNewGame);

        if (game is null) return;

        // Subscribe to the StateUpdated event to render the game state whenever it changes
        game.StateUpdated += (_, e) =>
        {
            if (e.State is PokerGameState state)
            {
                _lastState = state;
                ConsolePokerRenderer.RenderTable(state);
            }
        };

        // Run the game loop
        game.Run();

        Console.WriteLine("\nAppuyez sur une touche pour quitter...");
        Console.ReadKey(intercept: true);
    }
}
