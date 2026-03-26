using casino.console.Games;
using casino.console.Games.Blackjack;
using casino.console.Games.Poker;
using casino.console.Games.Slots;
using casino.core.Games.Blackjack;
using casino.console.Localization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace casino.console;

/// <summary>
/// Provides the entry point for the Casino All-In console application and initializes the poker game experience.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Program
{
    private static readonly ConsolePokerRenderer PokerRenderer = new();

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        SetCultureFromSystem();

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

            if (e.State is casino.core.Games.Slots.SlotMachineGameState slotMachineState)
            {
                ConsoleSlotMachineRenderer.RenderTable(slotMachineState);
            }
        };

        game.Run();

        Console.WriteLine($"\n{ConsoleText.PressAnyKeyToQuit}");
        Console.ReadKey(intercept: true);
    }

    private static casino.core.IGame? BuildGame(ConsoleGameFactory factory)
    {
        Console.Clear();
        RenderMainMenu();
        Console.Write(ConsoleText.MainMenuChoice);

        var choice = (Console.ReadLine() ?? string.Empty).Trim();

        return choice.ToLowerInvariant() switch
        {
            "1" or "poker" => factory.CreatePoker(
                ConsolePokerInput.GetPlayerAction,
                ConsolePokerInput.AskContinueNewGame,
                ConsolePokerInput.PromptGameSetup()),
            "2" or "blackjack" => factory.CreateBlackjack(ConsoleBlackjackInput.GetPlayerAction, ConsoleBlackjackInput.AskContinueNewGame),
            "3" or "slot" or "slots" => factory.CreateSlotMachine(ConsoleSlotMachineInput.GetBet, ConsoleSlotMachineInput.AskContinueNewGame),
            _ => null
        };
    }


    private static void SetCultureFromSystem()
    {
        var systemCulture = CultureInfo.InstalledUICulture;
        var selected = systemCulture.TwoLetterISOLanguageName.Equals("fr", StringComparison.OrdinalIgnoreCase)
            ? new CultureInfo("fr-FR")
            : new CultureInfo("en");

        CultureInfo.CurrentCulture = selected;
        CultureInfo.CurrentUICulture = selected;
        CultureInfo.DefaultThreadCurrentCulture = selected;
        CultureInfo.DefaultThreadCurrentUICulture = selected;
    }

    private static void RenderMainMenu()
    {
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║               CASINO ALL-IN                  ║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine("║  1. Poker                                    ║");
        Console.WriteLine("║  2. BlackJack                                ║");
        Console.WriteLine("║  3. Slot Machine                             ║");
        Console.WriteLine($"║  4. {ConsoleText.MenuQuit,-40}║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();
    }
}
