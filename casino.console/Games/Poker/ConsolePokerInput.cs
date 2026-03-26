using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using ActionModel = casino.core.Games.Poker.Actions.GameAction;
using casino.console.Localization;

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

    public static ActionModel GetPlayerAction(ActionRequest request)
    {
        var state = (PokerGameState)request.TableState;
        var player = state.Players.First(j => j.Name == request.PlayerName);

        var choice = ReadActionChoice(request.AvailableActions, request.MinimumBet);

        return choice switch
        {
            PokerTypeAction.Bet => new ActionModel(choice, request.MinimumBet),
            PokerTypeAction.Raise => new ActionModel(choice, ReadRaiseAmount(player.Chips, state.CurrentBet, player.Contribution)),
            _ => new ActionModel(choice, 0),
        };
    }

    public static PokerGameSetup PromptGameSetup()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine($"║           {ConsoleText.PokerSettingsTitle,-31}║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");

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

        Console.Clear(); // To avoid UI bugs in the poker table rendering
        
        return new PokerGameSetup(initialChips, playerCount, opponents);
    }

    public static bool AskContinueNewGame()
    {
        Console.Write($"\n{ConsoleText.ContinuePokerPrompt}");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        return answer is "o" or "oui" or "y" or "yes";
    }

    private static PokerTypeAction ReadActionChoice(IReadOnlyList<PokerTypeAction> availableActions, int minimumBet)
    {
        while (true)
        {
            ConsolePokerRenderer.RenderAvailableActions(availableActions, minimumBet);

            Console.Write(ConsoleText.ActionChoicePrompt);
            if (!int.TryParse(Console.ReadLine(), out var raw))
                continue;

            if (availableActions.Any(a => (int)a == raw))
                return (PokerTypeAction)raw;
        }
    }

    private static int ReadRaiseAmount(int maxChips, int actualBet, int currentContribution)
    {
        while (true)
        {
            Console.Write(ConsoleText.RaiseTargetPrompt);
            if (!int.TryParse(Console.ReadLine(), out var amount))
                continue;

            if (amount > actualBet && amount - currentContribution <= maxChips)
                return amount;
        }
    }

    private static int ReadIntInRange(string prompt, int minValue, int maxValue, int defaultValue)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input))
                return defaultValue;

            if (int.TryParse(input, out var value) && value >= minValue && value <= maxValue)
                return value;

            Console.WriteLine(ConsoleText.RangeError(minValue, maxValue));
        }
    }

    private static PokerDifficulty ReadDifficulty(string prompt, PokerDifficulty defaultDifficulty)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input))
                return defaultDifficulty;

            if (int.TryParse(input, out var raw)
                && Enum.IsDefined(typeof(PokerDifficulty), raw))
            {
                return (PokerDifficulty)raw;
            }

            Console.WriteLine(ConsoleText.InvalidDifficulty);
        }
    }

    private static void RenderDifficultyOptions()
    {
        foreach (var value in Enum.GetValues<PokerDifficulty>())
        {
            Console.WriteLine($"  {(int)value}. {ConsoleText.PokerDifficultyLabel(value),-10}");
        }
    }
}
