using casino.console.jeux.Poker;
using casino.console.Jeux;
using casino.core;
using casino.core.Common.Events;
using casino.core.Jeux.Poker;
using System;
using System.Text;

namespace casino.console;

public static class Program
{
    private static PokerGameState? _lastState;

    public static void Main(string[] args)
    {
        // Configurer la console pour supporter les caractères Unicode (pour les symboles des cartes)
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Casino All-In ===\n");

        IGameFactory factory = new ConsoleGameFactory();

        // Gestion du poker
        var renderer = new ConsolePokerRenderer();
        var input = new ConsolePokerInput(renderer);
        var game = factory.Create("poker", input.GetPlayerAction, ConsolePokerInput.AskContinueNewGame);

        if (game is null) return;

        game.StateUpdated += (_, e) =>
        {
            if (e.State is PokerGameState state)
            {
                _lastState = state;
                ConsolePokerRenderer.RenderTable(state);
            }
        };

        game.Run();

        Console.WriteLine("\nAppuyez sur une touche pour quitter...");
        Console.ReadKey(intercept: true);
    }
}
