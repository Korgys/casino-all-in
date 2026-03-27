using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using casino.console.Games;
using casino.console.Games.Blackjack;
using casino.console.Games.Poker;
using casino.console.Games.Slots;
using casino.console.Localization;
using casino.core.Games.Blackjack;

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
            "3" or "slot" or "slots" => ConsoleGameFactory.CreateSlotMachine(ConsoleSlotMachineInput.GetBet, ConsoleSlotMachineInput.AskContinueNewGame),
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

    /// <summary>
    /// Renders the main menu in an adaptive framed layout.
    /// </summary>
    private static void RenderMainMenu()
    {
        const int preferredWidth = 46;
        var width = Games.Commons.ConsoleLayout.ResolveContentWidth(preferredWidth);

        Games.Commons.ConsoleLayout.WriteTopBorder(width);
        Games.Commons.ConsoleLayout.WriteFramedLine(" CASINO ALL-IN ", width);
        Games.Commons.ConsoleLayout.WriteSeparator(width);
        Games.Commons.ConsoleLayout.WriteFramedLine(" 1. Poker ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine(" 2. BlackJack ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine(" 3. Slot Machine ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" 4. {ConsoleText.MenuQuit} ", width);
        Games.Commons.ConsoleLayout.WriteBottomBorder(width);
        Console.WriteLine();
    }
}
