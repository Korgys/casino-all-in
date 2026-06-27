using casino.console.Localization;

namespace casino.console.Games.Commons;

internal static class ConsolePromptReader
{
    private const int InvalidInputHintThreshold = 2;

    public static int ReadIntInRange(
        string prompt,
        int minValue,
        int maxValue,
        int defaultValue,
        string? invalidNumberMessage = null)
    {
        var invalidAttempts = 0;

        while (true)
        {
            Console.Write(prompt);
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input))
                return defaultValue;

            if (ConsoleInputAliases.IsBack(input))
            {
                Console.WriteLine(ConsoleText.InputCanceledUsingDefault(defaultValue.ToString()));
                return defaultValue;
            }

            if (int.TryParse(input, out var value))
            {
                if (value >= minValue && value <= maxValue)
                    return value;

                Console.WriteLine(ConsoleText.RangeError(minValue, maxValue));
            }
            else
            {
                Console.WriteLine(invalidNumberMessage ?? ConsoleText.InvalidNumberInput);
            }

            invalidAttempts++;
            WriteNumberMenuHintIfNeeded(invalidAttempts);
        }
    }

    public static void WriteNumberMenuHintIfNeeded(int invalidAttempts)
    {
        if (invalidAttempts >= InvalidInputHintThreshold)
            Console.WriteLine(ConsoleText.TypeNumberMenuHelp);
    }
}
