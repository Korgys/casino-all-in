using casino.core.Games.Poker;

namespace casino.console.Cli;

internal static class CasinoCliParser
{
    private const int MinimumPokerPlayers = 2;
    private const int MaximumPokerPlayers = 6;
    private const int MinimumInitialChips = 100;
    private const int MaximumInitialChips = 5000;
    private const PokerDifficulty DefaultPokerDifficulty = PokerDifficulty.Medium;
    private static readonly PokerOptionDefinition[] PokerOptions =
    [
        new(PokerOptionKind.Players, "players", "-p", "--players"),
        new(PokerOptionKind.Difficulty, "difficulty", "-d", "--difficulty"),
        new(PokerOptionKind.Chips, "chips", "-c", "--chips")
    ];

    public const string Usage = """
        Usage:
          casino <game> [options]

        Games:
          poker
          blackjack
          slots
          roulette

        Poker options:
          -p, --players <2-6>       Total players, including you.
          -d, --difficulty <1-6>    AI difficulty for every opponent.
          -c, --chips <100-5000>    Initial chips for each player.

        Examples:
          casino blackjack
          casino poker -p 4 -d 4 -c 1000
        """;

    public static CasinoCliParseResult Parse(IReadOnlyList<string> args)
    {
        if (args.Count == 0)
            return CasinoCliParseResult.Failure("A game command is required.");

        if (args.Any(IsHelpOption))
            return CasinoCliParseResult.Help();

        if (!TryParseGame(args[0], out var game))
            return CasinoCliParseResult.Failure($"Unknown game '{args[0]}'.");

        var options = args.Skip(1).ToArray();

        if (game != CasinoGameKind.Poker)
        {
            return options.Length == 0
                ? CasinoCliParseResult.Success(new CasinoCliCommand(game, PokerSetup: null))
                : CasinoCliParseResult.Failure($"Game '{args[0]}' does not support options.");
        }

        return ParsePokerCommand(options);
    }

    private static CasinoCliParseResult ParsePokerCommand(IReadOnlyList<string> args)
    {
        if (args.Count == 0)
            return CasinoCliParseResult.Success(new CasinoCliCommand(CasinoGameKind.Poker, PokerSetup: null));

        int? playerCount = null;
        int? initialChips = null;
        PokerDifficulty? difficulty = null;

        for (var index = 0; index < args.Count; index++)
        {
            if (!TryReadPokerOption(args, ref index, out var option, out var value, out var error))
                return CasinoCliParseResult.Failure(error);

            if (!TryApplyPokerOption(option, value, ref playerCount, ref difficulty, ref initialChips, out error))
                return CasinoCliParseResult.Failure(error);
        }

        var defaultSetup = PokerGameSetup.CreateDefault();
        var resolvedPlayerCount = playerCount ?? defaultSetup.PlayerCount;
        var resolvedInitialChips = initialChips ?? defaultSetup.InitialChips;
        var resolvedDifficulty = difficulty ?? DefaultPokerDifficulty;
        var opponents = Enumerable
            .Range(0, resolvedPlayerCount - 1)
            .Select(_ => new PokerOpponentSetup(resolvedDifficulty))
            .ToArray();

        return CasinoCliParseResult.Success(new CasinoCliCommand(
            CasinoGameKind.Poker,
            new PokerGameSetup(resolvedInitialChips, resolvedPlayerCount, opponents)));
    }

    private static bool TryReadPokerOption(
        IReadOnlyList<string> args,
        ref int index,
        out PokerOptionDefinition option,
        out string value,
        out string error)
    {
        var token = args[index];

        foreach (var candidate in PokerOptions)
        {
            if (TryReadInlineOption(token, candidate.ShortName, candidate.LongName, out value))
            {
                option = candidate;
                error = string.Empty;
                return true;
            }

            if (IsOption(token, candidate.ShortName, candidate.LongName))
            {
                option = candidate;
                return TryReadNextValue(args, ref index, token, out value, out error);
            }
        }

        option = default;
        value = string.Empty;
        error = $"Unknown poker option '{token}'.";
        return false;
    }

    private static bool TryApplyPokerOption(
        PokerOptionDefinition option,
        string value,
        ref int? playerCount,
        ref PokerDifficulty? difficulty,
        ref int? initialChips,
        out string error)
    {
        return option.Kind switch
        {
            PokerOptionKind.Players => TryParseIntOption(
                option.Name,
                value,
                MinimumPokerPlayers,
                MaximumPokerPlayers,
                out playerCount,
                out error),
            PokerOptionKind.Difficulty => TryParseDifficulty(value, out difficulty, out error),
            PokerOptionKind.Chips => TryParseIntOption(
                option.Name,
                value,
                MinimumInitialChips,
                MaximumInitialChips,
                out initialChips,
                out error),
            _ => FailUnknownPokerOption(out error)
        };
    }

    private static bool TryParseGame(string value, out CasinoGameKind game)
    {
        switch (value.Trim().ToLowerInvariant())
        {
            case "poker":
                game = CasinoGameKind.Poker;
                return true;

            case "blackjack":
            case "black-jack":
            case "black":
                game = CasinoGameKind.Blackjack;
                return true;

            case "slot":
            case "slots":
            case "slot-machine":
            case "slotmachine":
                game = CasinoGameKind.SlotMachine;
                return true;

            case "roulette":
                game = CasinoGameKind.Roulette;
                return true;

            default:
                game = default;
                return false;
        }
    }

    private static bool TryParseIntOption(
        string name,
        string value,
        int minimum,
        int maximum,
        out int? parsed,
        out string error)
    {
        parsed = null;
        error = string.Empty;

        if (!int.TryParse(value, out var number))
        {
            error = $"Option '{name}' must be a number.";
            return false;
        }

        if (number < minimum || number > maximum)
        {
            error = $"Option '{name}' must be between {minimum} and {maximum}.";
            return false;
        }

        parsed = number;
        return true;
    }

    private static bool TryParseDifficulty(string value, out PokerDifficulty? difficulty, out string error)
    {
        difficulty = null;
        error = string.Empty;

        if (!int.TryParse(value, out var raw))
        {
            error = "Option 'difficulty' must be a number.";
            return false;
        }

        if (!Enum.IsDefined(typeof(PokerDifficulty), raw))
        {
            error = "Option 'difficulty' must be between 1 and 6.";
            return false;
        }

        difficulty = (PokerDifficulty)raw;
        return true;
    }

    private static bool TryReadNextValue(
        IReadOnlyList<string> args,
        ref int index,
        string option,
        out string value,
        out string error)
    {
        value = string.Empty;
        error = string.Empty;

        if (index + 1 >= args.Count || args[index + 1].StartsWith("-", StringComparison.Ordinal))
        {
            error = $"Option '{option}' requires a value.";
            return false;
        }

        value = args[++index];
        return true;
    }

    private static bool IsHelpOption(string value)
    {
        return IsOption(value, "-h", "--help") || string.Equals(value, "help", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsOption(string value, string shortName, string longName)
    {
        return string.Equals(value, shortName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, longName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryReadInlineOption(string option, string shortName, string longName, out string value)
    {
        value = string.Empty;
        var shortPrefix = shortName + "=";
        var longPrefix = longName + "=";

        if (option.StartsWith(shortPrefix, StringComparison.OrdinalIgnoreCase))
        {
            value = option[shortPrefix.Length..];
            return true;
        }

        if (option.StartsWith(longPrefix, StringComparison.OrdinalIgnoreCase))
        {
            value = option[longPrefix.Length..];
            return true;
        }

        return false;
    }

    private static bool FailUnknownPokerOption(out string error)
    {
        error = "Unknown poker option.";
        return false;
    }

    private readonly record struct PokerOptionDefinition(PokerOptionKind Kind, string Name, string ShortName, string LongName);

    private enum PokerOptionKind
    {
        Players,
        Difficulty,
        Chips
    }
}
