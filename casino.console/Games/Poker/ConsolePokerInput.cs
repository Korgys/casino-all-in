using casino.console.Games.Commons;
using casino.console.Localization;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using ActionModel = casino.core.Games.Poker.Actions.GameAction;

namespace casino.console.Games.Poker;

/// <summary>
/// Class responsible for managing user input for the poker game in the console.
/// </summary>
public static class ConsolePokerInput
{
    private const int MinimumPlayers = 2;
    private const int MaximumPlayers = 6;
    private const int MinimumInitialChips = 100;
    private const int MaximumInitialChips = 5000;
    private const int InvalidInputHintThreshold = 2;

    public static ActionModel GetPlayerAction(ActionRequest request)
    {
        var state = (PokerGameState)request.TableState;
        var player = state.Players.First(j => j.Name == request.PlayerName);

        var choice = ReadActionChoice(state, request.AvailableActions, request.MinimumBet);

        return choice switch
        {
            PokerTypeAction.Bet => new ActionModel(choice, request.MinimumBet),
            PokerTypeAction.Raise => new ActionModel(choice, ReadRaiseAmount(player.Chips, state.CurrentBet, player.Contribution)),
            _ => new ActionModel(choice, 0),
        };
    }

    /// <summary>
    /// Prompts and builds the poker game setup using adaptive console panels.
    /// </summary>
    /// <returns>The configured poker game setup.</returns>
    public static PokerGameSetup PromptGameSetup()
    {
        RequestFullConsoleRefresh();
        var frameWidth = ConsoleLayout.ResolveContentWidth(46);
        ConsoleLayout.WriteTopBorder(frameWidth);
        ConsoleLayout.WriteFramedLine($" {ConsoleText.PokerSettingsTitle} ", frameWidth);
        ConsoleLayout.WriteBottomBorder(frameWidth);

        var initialChips = ReadIntInRange(
            ConsoleText.InitialChipsPrompt(MinimumInitialChips, MaximumInitialChips),
            MinimumInitialChips,
            MaximumInitialChips,
            defaultValue: 1000);

        var playerCount = ReadIntInRange(
            ConsoleText.PlayerCountPrompt(MinimumPlayers, MaximumPlayers),
            MinimumPlayers,
            MaximumPlayers,
            defaultValue: 5);

        Console.WriteLine();
        RenderDifficultyOptions();
        var difficulty = ReadDifficulty(ConsoleText.DifficultyPrompt(ConsoleText.PokerDifficultyLabel(PokerDifficulty.Medium)), PokerDifficulty.Medium);

        var opponents = new List<PokerOpponentSetup>();
        for (var index = 1; index < playerCount; index++)
            opponents.Add(new PokerOpponentSetup(difficulty));

        RequestFullConsoleRefresh(); // To avoid UI bugs in the poker table rendering

        return new PokerGameSetup(initialChips, playerCount, opponents);
    }

    public static bool AskContinueNewGame()
    {
        Console.Write($"\n{ConsoleText.ContinuePokerPrompt}");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        var wantsToContinue = ConsoleInputAliases.IsYes(answer);

        if (wantsToContinue)
            RequestFullConsoleRefresh();

        return wantsToContinue;
    }

    private static PokerTypeAction ReadActionChoice(PokerGameState state, IReadOnlyList<PokerTypeAction> availableActions, int minimumBet)
    {
        ConsolePokerRenderer.RenderAvailableActions(availableActions, minimumBet);
        var invalidAttempts = 0;

        while (true)
        {
            Console.Write(ConsoleText.ActionChoicePrompt);
            if (!int.TryParse(Console.ReadLine(), out var raw))
            {
                invalidAttempts++;
                Console.WriteLine(ConsoleText.InvalidNumberInput);
                WriteNumberMenuHintIfNeeded(invalidAttempts);
                continue;
            }

            if (availableActions.Any(a => (int)a == raw))
                return (PokerTypeAction)raw;

            invalidAttempts++;
            Console.WriteLine(ConsoleText.ActionUnavailable(raw));
            WriteNumberMenuHintIfNeeded(invalidAttempts);
        }
    }

    private static void RequestFullConsoleRefresh()
    {
        ConsolePokerRenderer.RequestFullRefresh();

        try
        {
            Console.Clear();
        }
        catch
        {
            // Ignore clear failures in redirected output environments.
        }
    }

    private static int ReadRaiseAmount(int maxChips, int actualBet, int currentContribution)
    {
        var maxAllowedTarget = currentContribution + maxChips;
        var invalidAttempts = 0;

        while (true)
        {
            Console.Write(ConsoleText.RaiseTargetPromptWithConstraints(actualBet, currentContribution, maxAllowedTarget));
            if (!int.TryParse(Console.ReadLine(), out var amount))
            {
                invalidAttempts++;
                Console.WriteLine(ConsoleText.InvalidNumberInput);
                WriteNumberMenuHintIfNeeded(invalidAttempts);
                continue;
            }

            if (amount > actualBet && amount - currentContribution <= maxChips)
                return amount;

            invalidAttempts++;
            Console.WriteLine(ConsoleText.RaiseAmountUnavailable(actualBet, maxAllowedTarget));
            WriteNumberMenuHintIfNeeded(invalidAttempts);
        }
    }

    private static int ReadIntInRange(string prompt, int minValue, int maxValue, int defaultValue)
    {
        var invalidAttempts = 0;

        while (true)
        {
            Console.Write(prompt);
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input) || IsCancelInput(input))
            {
                if (IsCancelInput(input))
                    Console.WriteLine(ConsoleText.InputCanceledUsingDefault(defaultValue.ToString()));
                return defaultValue;
            }

            if (int.TryParse(input, out var value) && value >= minValue && value <= maxValue)
                return value;

            invalidAttempts++;
            Console.WriteLine(ConsoleText.RangeError(minValue, maxValue));
            WriteNumberMenuHintIfNeeded(invalidAttempts);
        }
    }

    private static PokerDifficulty ReadDifficulty(string prompt, PokerDifficulty defaultDifficulty)
    {
        var invalidAttempts = 0;

        while (true)
        {
            Console.Write(prompt);
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input) || IsCancelInput(input))
            {
                if (IsCancelInput(input))
                    Console.WriteLine(ConsoleText.InputCanceledUsingDefault(ConsoleText.PokerDifficultyLabel(defaultDifficulty)));
                return defaultDifficulty;
            }

            if (int.TryParse(input, out var raw)
                && Enum.IsDefined(typeof(PokerDifficulty), raw))
            {
                return (PokerDifficulty)raw;
            }

            invalidAttempts++;
            Console.WriteLine(ConsoleText.InvalidDifficulty);
            WriteNumberMenuHintIfNeeded(invalidAttempts);
        }
    }

    private static bool IsCancelInput(string input)
    {
        return ConsoleInputAliases.IsBack(input);
    }

    private static void WriteNumberMenuHintIfNeeded(int invalidAttempts)
    {
        if (invalidAttempts < InvalidInputHintThreshold)
            return;

        Console.WriteLine(ConsoleText.TypeNumberMenuHelp);
    }

    /// <summary>
    /// Renders available poker difficulty options without fixed-width padding.
    /// </summary>
    private static void RenderDifficultyOptions()
    {
        foreach (var value in Enum.GetValues<PokerDifficulty>())
        {
            Console.WriteLine($"  {(int)value}. {ConsoleText.PokerDifficultyLabel(value)}");
        }
    }
}
