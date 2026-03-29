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
        while (true)
        {
            Console.Clear();
            RenderMainMenu();
            Console.Write(ConsoleText.MainMenuChoice);

            var choice = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

            switch (choice)
            {
                case "1":
                case "poker":
                    return factory.CreatePoker(
                        ConsolePokerInput.GetPlayerAction,
                        ConsolePokerInput.AskContinueNewGame,
                        ConsolePokerInput.PromptGameSetup());

                case "2":
                case "blackjack":
                    return factory.CreateBlackjack(ConsoleBlackjackInput.GetPlayerAction, ConsoleBlackjackInput.AskContinueNewGame);

                case "3":
                case "slot":
                case "slots":
                    return ConsoleGameFactory.CreateSlotMachine(ConsoleSlotMachineInput.GetBet, ConsoleSlotMachineInput.AskContinueNewGame);

                case "4":
                case "language":
                case "languages":
                case "lange":
                case "langage":
                case "sprache":
                case "sprachen":
                    ShowLanguageMenu();
                    break;

                case "5":
                case "quit":
                case "quitter":
                case "beenden":
                    return null;
            }
        }
    }


    private static void SetCultureFromSystem()
    {
        var systemCulture = CultureInfo.InstalledUICulture;
        var selected = systemCulture.TwoLetterISOLanguageName.ToLowerInvariant() switch
        {
            "fr" => new CultureInfo("fr-FR"),
            "de" => new CultureInfo("de-DE"),
            _ => new CultureInfo("en")
        };

        SetCulture(selected);
    }

    private static void SetCulture(CultureInfo selected)
    {
        CultureInfo.CurrentCulture = selected;
        CultureInfo.CurrentUICulture = selected;
        CultureInfo.DefaultThreadCurrentCulture = selected;
        CultureInfo.DefaultThreadCurrentUICulture = selected;
    }

    private static void ShowLanguageMenu()
    {
        while (true)
        {
            Console.Clear();
            RenderLanguageMenu();
            Console.Write(ConsoleText.LanguageMenuChoice);

            var choice = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

            switch (choice)
            {
                case "1":
                case "fr":
                case "francais":
                case "français":
                    SetCulture(new CultureInfo("fr-FR"));
                    return;

                case "2":
                case "en":
                case "english":
                    SetCulture(new CultureInfo("en"));
                    return;

                case "3":
                case "de":
                case "deutsch":
                case "german":
                case "allemand":
                    SetCulture(new CultureInfo("de-DE"));
                    return;

                case "4":
                case "back":
                case "retour":
                case "zuruck":
                case "zurück":
                    return;
            }
        }
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
        Games.Commons.ConsoleLayout.WriteFramedLine($" 1. {ConsoleText.MenuPoker} ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" 2. {ConsoleText.MenuBlackjack} ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" 3. {ConsoleText.MenuSlotMachine} ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" 4. {ConsoleText.MenuLanguages} ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" 5. {ConsoleText.MenuQuit} ", width);
        Games.Commons.ConsoleLayout.WriteBottomBorder(width);
        Console.WriteLine();
    }

    private static void RenderLanguageMenu()
    {
        const int preferredWidth = 46;
        var width = Games.Commons.ConsoleLayout.ResolveContentWidth(preferredWidth);

        Games.Commons.ConsoleLayout.WriteTopBorder(width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" {ConsoleText.LanguageMenuTitle} ", width);
        Games.Commons.ConsoleLayout.WriteSeparator(width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" {ConsoleText.CurrentLanguageLabel}: {GetCurrentLanguageName()} ", width);
        Games.Commons.ConsoleLayout.WriteSeparator(width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" 1. {ConsoleText.LanguageFrench} ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" 2. {ConsoleText.LanguageEnglish} ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" 3. {ConsoleText.LanguageGerman} ", width);
        Games.Commons.ConsoleLayout.WriteFramedLine($" 4. {ConsoleText.MenuBack} ", width);
        Games.Commons.ConsoleLayout.WriteBottomBorder(width);
        Console.WriteLine();
    }

    private static string GetCurrentLanguageName()
    {
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant() switch
        {
            "fr" => ConsoleText.LanguageFrench,
            "de" => ConsoleText.LanguageGerman,
            _ => ConsoleText.LanguageEnglish
        };
    }
}
