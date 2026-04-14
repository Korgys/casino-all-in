using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using casino.console.Games;
using casino.console.Games.Blackjack;
using casino.console.Games.Commons;
using casino.console.Games.Poker;
using casino.console.Games.Roulette;
using casino.console.Games.Slots;
using casino.console.Localization;
using casino.core;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker;
using casino.core.Games.Roulette;
using casino.core.Games.Slots;

namespace casino.console;

/// <summary>
/// Provides the entry point for the Casino All-In console application and initializes the poker game experience.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Program
{
    private static readonly ConsolePokerRenderer PokerRenderer = new();
    private static readonly LanguageOption[] LanguageOptions =
    [
        new(1, new CultureInfo("fr-FR"), () => "🇫🇷 " + ConsoleText.LanguageFrench, ["1", "fr", "francais", "français"]),
        new(2, new CultureInfo("en"), () => "🇬🇧 " + ConsoleText.LanguageEnglish, ["2", "en", "english"]),
        new(3, new CultureInfo("de-DE"), () => "🇩🇪 " + ConsoleText.LanguageGerman, ["3", "de", "deutsch", "german", "allemand"]),
        new(4, new CultureInfo("es-ES"), () => "🇪🇸 " + ConsoleText.LanguageSpanish, ["4", "es", "spanish", "espanol", "español"]),
        new(5, new CultureInfo("ja-JP"), () => "🇯🇵 " + ConsoleText.LanguageJapanese, ["5", "ja", "jp", "japanese", "nihongo", "日本語"]),
        new(6, new CultureInfo("zh-Hans"), () => "🇨🇳 " + ConsoleText.LanguageSimplifiedChinese, ["6", "zh", "zh-cn", "zh-hans", "chinese", "中文", "简体中文"]),
        new(7, new CultureInfo("ru-RU"), () => "🇷🇺 " + ConsoleText.LanguageRussian, ["7", "ru", "russian", "russkiy", "russkij", "русский"])
    ];

    private static readonly Dictionary<string, LanguageOption> LanguageOptionsByAlias =
        LanguageOptions
            .SelectMany(option => option.Aliases.Select(alias => new KeyValuePair<string, LanguageOption>(alias, option)))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, LanguageOption> LanguageOptionsByIsoCode =
        LanguageOptions.ToDictionary(option => option.Culture.TwoLetterISOLanguageName, StringComparer.OrdinalIgnoreCase);

    private static readonly LanguageOption DefaultLanguageOption =
        LanguageOptions.Single(option => string.Equals(option.Culture.Name, "en", StringComparison.OrdinalIgnoreCase));

    private static readonly HashSet<string> BackLanguageMenuChoices =
    [
        "8",
        "back",
        "retour",
        "volver",
        "zuruck",
        "zurück",
        "назад",
        "戻る",
        "返回"
    ];

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        SetCultureFromSystem();

        var factory = new ConsoleGameFactory();
        var game = BuildGame(factory);

        if (game is null) return;

        var refreshPokerTableAfterRoundEnd = false;

        game.GameEnded += (_, _) => refreshPokerTableAfterRoundEnd = true;

        game.StateUpdated += (_, e) =>
        {
            if (e.State is PokerGameState state)
            {
                if (refreshPokerTableAfterRoundEnd)
                {
                    ConsolePokerRenderer.RequestFullRefresh();
                    refreshPokerTableAfterRoundEnd = false;
                }

                PokerRenderer.RenderTable(state);
            }
            if (e.State is BlackjackGameState blackjackState)
            {
                ConsoleBlackjackRenderer.RenderTable(blackjackState);
            }
            if (e.State is SlotMachineGameState slotMachineState)
            {
                ConsoleSlotMachineRenderer.RenderTable(slotMachineState);
            }
            if (e.State is RouletteGameState rouletteState)
            {
                ConsoleRouletteRenderer.RenderTable(rouletteState);
            }
        };

        game.Run();

        Console.WriteLine($"\n{ConsoleText.PressAnyKeyToQuit}");
        Console.ReadKey(intercept: true);
    }

    private static IGame? BuildGame(ConsoleGameFactory factory)
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
                case "póker":
                case "ポーカー":
                case "扑克":
                case "покер":
                    return factory.CreatePoker(
                        ConsolePokerInput.GetPlayerAction,
                        ConsolePokerInput.AskContinueNewGame,
                        ConsolePokerInput.PromptGameSetup());

                case "2":
                case "blackjack":
                case "black-jack":
                case "black jack":
                case "二十一点":
                case "блэкджек":
                    return factory.CreateBlackjack(ConsoleBlackjackInput.GetPlayerAction, ConsoleBlackjackInput.AskContinueNewGame);

                case "3":
                case "slot":
                case "slots":
                case "slot machine":
                case "spielautomat":
                case "tragamonedas":
                case "スロット":
                case "老虎机":
                case "игровой автомат":
                    return factory.CreateSlotMachine(ConsoleSlotMachineInput.GetBet, ConsoleSlotMachineInput.AskContinueNewGame);

                case "4":
                case "roulette":
                case "roulette wheel":
                case "ruleta":
                case "ruletka":
                    return factory.CreateRoulette(ConsoleRouletteInput.GetBet, ConsoleRouletteInput.AskContinueNewGame);

                case "5":
                case "language":
                case "languages":
                case "langages":
                case "lange":
                case "langage":
                case "idioma":
                case "idiomas":
                case "sprache":
                case "sprachen":
                case "言語":
                case "语言":
                case "語言":
                case "язык":
                case "языки":
                    ShowLanguageMenu();
                    break;

                case "6":
                case "quit":
                case "quitter":
                case "beenden":
                case "salir":
                case "sortir":
                case "終了":
                case "退出":
                case "выход":
                    return null;
            }
        }
    }


    private static void SetCultureFromSystem()
    {
        var systemIsoCode = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
        var selectedOption = LanguageOptionsByIsoCode.GetValueOrDefault(systemIsoCode, DefaultLanguageOption);
        SetCulture(selectedOption.Culture);
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

            if (BackLanguageMenuChoices.Contains(choice))
            {
                return;
            }

            if (!LanguageOptionsByAlias.TryGetValue(choice, out var selectedLanguage))
            {
                continue;
            }

            SetCulture(selectedLanguage.Culture);
            return;
        }
    }

    /// <summary>
    /// Renders the main menu in an adaptive framed layout.
    /// </summary>
    private static void RenderMainMenu()
    {
        const int preferredWidth = 46;
        var width = ConsoleLayout.ResolveContentWidth(preferredWidth);

        ConsoleLayout.WriteTopBorder(width);
        ConsoleLayout.WriteFramedLine(" CASINO ALL-IN ", width);
        ConsoleLayout.WriteSeparator(width);
        ConsoleLayout.WriteFramedLine($" 1. {ConsoleText.MenuPoker} ", width);
        ConsoleLayout.WriteFramedLine($" 2. {ConsoleText.MenuBlackjack} ", width);
        ConsoleLayout.WriteFramedLine($" 3. {ConsoleText.MenuSlotMachine} ", width);
        ConsoleLayout.WriteFramedLine($" 4. {ConsoleText.MenuRoulette} ", width);
        ConsoleLayout.WriteFramedLine($" 5. {ConsoleText.MenuLanguages} ", width);
        ConsoleLayout.WriteFramedLine($" 6. {ConsoleText.MenuQuit} ", width);
        ConsoleLayout.WriteBottomBorder(width);
        Console.WriteLine();
    }

    private static void RenderLanguageMenu()
    {
        const int preferredWidth = 46;
        var width = ConsoleLayout.ResolveContentWidth(preferredWidth);

        ConsoleLayout.WriteTopBorder(width);
        ConsoleLayout.WriteFramedLine($" {ConsoleText.LanguageMenuTitle} ", width);
        ConsoleLayout.WriteSeparator(width);
        ConsoleLayout.WriteFramedLine($" {ConsoleText.CurrentLanguageLabel}: {GetCurrentLanguageName()} ", width);
        ConsoleLayout.WriteSeparator(width);
        foreach (var option in LanguageOptions)
        {
            ConsoleLayout.WriteFramedLine($" {option.MenuNumber}. {option.DisplayName()} ", width);
        }

        ConsoleLayout.WriteFramedLine($" 8. {ConsoleText.MenuBack} ", width);
        ConsoleLayout.WriteBottomBorder(width);
        Console.WriteLine();
    }

    private static string GetCurrentLanguageName()
    {
        var currentIsoCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        return LanguageOptionsByIsoCode.GetValueOrDefault(currentIsoCode, DefaultLanguageOption).DisplayName();
    }

    private sealed record LanguageOption(int MenuNumber, CultureInfo Culture, Func<string> DisplayName, IReadOnlyCollection<string> Aliases);
}
