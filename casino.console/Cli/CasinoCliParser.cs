using casino.core.Games.Poker;

namespace casino.console.Cli;

internal static class CasinoCliParser
{
    private const int MinimumPokerPlayers = 2;
    private const int MaximumPokerPlayers = 6;
    private const int MinimumInitialChips = 100;
    private const int MaximumInitialChips = 5000;
    private const PokerDifficulty DefaultPokerDifficulty = PokerDifficulty.Medium;

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
            var option = args[index];

            if (TryReadInlineOption(option, "-p", "--players", out var inlinePlayers))
            {
                if (!TryParseIntOption("players", inlinePlayers, MinimumPokerPlayers, MaximumPokerPlayers, out playerCount, out var error))
                    return CasinoCliParseResult.Failure(error);
                continue;
            }

            if (TryReadInlineOption(option, "-d", "--difficulty", out var inlineDifficulty))
            {
                if (!TryParseDifficulty(inlineDifficulty, out difficulty, out var error))
                    return CasinoCliParseResult.Failure(error);
                continue;
            }

            if (TryReadInlineOption(option, "-c", "--chips", out var inlineChips))
            {
                if (!TryParseIntOption("chips", inlineChips, MinimumInitialChips, MaximumInitialChips, out initialChips, out var error))
                    return CasinoCliParseResult.Failure(error);
                continue;
            }

            if (IsOption(option, "-p", "--players"))
            {
                if (!TryReadNextValue(args, ref index, option, out var value, out var error)
                    || !TryParseIntOption("players", value, MinimumPokerPlayers, MaximumPokerPlayers, out playerCount, out error))
                {
                    return CasinoCliParseResult.Failure(error);
                }

                continue;
            }

            if (IsOption(option, "-d", "--difficulty"))
            {
                if (!TryReadNextValue(args, ref index, option, out var value, out var error)
                    || !TryParseDifficulty(value, out difficulty, out error))
                {
                    return CasinoCliParseResult.Failure(error);
                }

                continue;
            }

            if (IsOption(option, "-c", "--chips"))
            {
                if (!TryReadNextValue(args, ref index, option, out var value, out var error)
                    || !TryParseIntOption("chips", value, MinimumInitialChips, MaximumInitialChips, out initialChips, out error))
                {
                    return CasinoCliParseResult.Failure(error);
                }

                continue;
            }

            return CasinoCliParseResult.Failure($"Unknown poker option '{option}'.");
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
}
