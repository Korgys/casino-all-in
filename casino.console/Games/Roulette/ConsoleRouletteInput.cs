using casino.console.Games.Commons;
using casino.console.Localization;
using casino.core.Games.Roulette;

namespace casino.console.Games.Roulette;

public static class ConsoleRouletteInput
{
    private const int InvalidInputHintThreshold = 2;

    public static RouletteBet GetBet(RouletteGameState state)
    {
        RenderBetPrompt(state);
        var invalidAttempts = 0;

        while (true)
        {
            Console.Write(ConsoleText.RouletteBetTypePrompt);
            var typeInput = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(typeInput))
                return new RouletteBet(RouletteBetKind.Red, state.MinBet);

            if (ConsoleInputAliases.IsBack(typeInput))
            {
                Console.WriteLine(ConsoleText.InputCanceledUsingDefault(state.MinBet.ToString()));
                return new RouletteBet(RouletteBetKind.Red, state.MinBet);
            }

            if (!TryParseBetKind(typeInput, out var kind))
            {
                invalidAttempts++;
                Console.WriteLine(ConsoleText.InvalidNumberInput);
                WriteNumberMenuHintIfNeeded(invalidAttempts);
                continue;
            }

            var number = kind == RouletteBetKind.Number ? (int?)PromptNumber() : null;
            var amount = PromptAmount(state);
            return new RouletteBet(kind, amount, number);
        }
    }

    public static bool AskContinueNewGame()
    {
        Console.Write($"\n{ConsoleText.RouletteContinuePrompt}");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        return ConsoleInputAliases.IsYes(answer);
    }

    private static int PromptAmount(RouletteGameState state)
    {
        var invalidAttempts = 0;

        while (true)
        {
            Console.Write(ConsoleText.RouletteBetAmountPrompt(state.MinBet));
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input))
                return state.MinBet;

            if (ConsoleInputAliases.IsBack(input))
            {
                Console.WriteLine(ConsoleText.InputCanceledUsingDefault(state.MinBet.ToString()));
                return state.MinBet;
            }

            if (int.TryParse(input, out var amount) && amount >= state.MinBet && amount <= state.MaxBet)
                return amount;

            invalidAttempts++;
            Console.WriteLine(int.TryParse(input, out _) ? ConsoleText.RangeError(state.MinBet, state.MaxBet) : ConsoleText.InvalidNumberInput);
            WriteNumberMenuHintIfNeeded(invalidAttempts);
        }
    }

    private static int PromptNumber()
    {
        var invalidAttempts = 0;

        while (true)
        {
            Console.Write(ConsoleText.RouletteNumberPrompt);
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input))
                return 0;

            if (ConsoleInputAliases.IsBack(input))
            {
                Console.WriteLine(ConsoleText.InputCanceledUsingDefault("0"));
                return 0;
            }

            if (int.TryParse(input, out var number) && number is >= 0 and <= 36)
                return number;

            invalidAttempts++;
            Console.WriteLine(ConsoleText.RangeError(0, 36));
            WriteNumberMenuHintIfNeeded(invalidAttempts);
        }
    }

    private static bool TryParseBetKind(string input, out RouletteBetKind kind)
    {
        switch (input.Trim().ToLowerInvariant())
        {
            case "1":
            case "number":
            case "straight":
                kind = RouletteBetKind.Number;
                return true;
            case "2":
            case "red":
                kind = RouletteBetKind.Red;
                return true;
            case "3":
            case "black":
                kind = RouletteBetKind.Black;
                return true;
            case "4":
            case "even":
                kind = RouletteBetKind.Even;
                return true;
            case "5":
            case "odd":
                kind = RouletteBetKind.Odd;
                return true;
            default:
                kind = RouletteBetKind.Red;
                return false;
        }
    }

    private static void WriteNumberMenuHintIfNeeded(int invalidAttempts)
    {
        if (invalidAttempts < InvalidInputHintThreshold)
            return;

        Console.WriteLine(ConsoleText.TypeNumberMenuHelp);
    }

    private static void RenderBetPrompt(RouletteGameState state)
    {
        Console.WriteLine();
        var frameWidth = ConsoleLayout.ResolveContentWidth(56);
        ConsoleLayout.WriteTopBorder(frameWidth);
        ConsoleLayout.WriteFramedLine($" {ConsoleText.RoulettePanelTitle} ", frameWidth, '|', '|');
        ConsoleLayout.WriteFramedLine($" {ConsoleText.RouletteCredits}: {state.Credits} ", frameWidth, '|', '|');
        ConsoleLayout.WriteFramedLine($" {ConsoleText.RouletteMinMaxBet}: {state.MinBet} - {state.MaxBet} ", frameWidth, '|', '|');
        ConsoleLayout.WriteSeparator(frameWidth, '-');
        ConsoleLayout.WriteFramedLine($" 1. {ConsoleText.RouletteOptionStraight} ", frameWidth, '|', '|');
        ConsoleLayout.WriteFramedLine($" 2. {ConsoleText.RouletteOptionRed} ", frameWidth, '|', '|');
        ConsoleLayout.WriteFramedLine($" 3. {ConsoleText.RouletteOptionBlack} ", frameWidth, '|', '|');
        ConsoleLayout.WriteFramedLine($" 4. {ConsoleText.RouletteOptionEven} ", frameWidth, '|', '|');
        ConsoleLayout.WriteFramedLine($" 5. {ConsoleText.RouletteOptionOdd} ", frameWidth, '|', '|');
        ConsoleLayout.WriteBottomBorder(frameWidth);
    }
}
