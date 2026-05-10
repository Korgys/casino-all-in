using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using casino.console.Cli;
using casino.console.Games;

namespace casino.console;

/// <summary>
/// Provides the entry point for the Casino All-In console application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Program
{
    private static readonly IReadOnlyDictionary<string, CultureInfo> SupportedCulturesByIsoCode =
        new Dictionary<string, CultureInfo>(StringComparer.OrdinalIgnoreCase)
        {
            ["fr"] = CultureInfo.GetCultureInfo("fr-FR"),
            ["en"] = CultureInfo.GetCultureInfo("en"),
            ["de"] = CultureInfo.GetCultureInfo("de-DE"),
            ["es"] = CultureInfo.GetCultureInfo("es-ES"),
            ["ja"] = CultureInfo.GetCultureInfo("ja-JP"),
            ["zh"] = CultureInfo.GetCultureInfo("zh-Hans"),
            ["ru"] = CultureInfo.GetCultureInfo("ru-RU")
        };

    private static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en");

    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        SetCultureFromSystem();

        var parseResult = CasinoCliParser.Parse(args);

        if (parseResult.IsHelp)
        {
            Console.WriteLine(CasinoCliParser.Usage);
            return 0;
        }

        if (!parseResult.IsSuccess || parseResult.Command is null)
        {
            Console.Error.WriteLine(parseResult.Error);
            Console.Error.WriteLine();
            Console.Error.WriteLine(CasinoCliParser.Usage);
            return 1;
        }

        var factory = new ConsoleGameFactory();
        var game = ConsoleGameBuilder.Create(factory, parseResult.Command);
        var runner = new ConsoleGameRunner();

        runner.Run(game);

        return 0;
    }

    private static void SetCultureFromSystem()
    {
        var systemIsoCode = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
        var selected = SupportedCulturesByIsoCode.GetValueOrDefault(systemIsoCode, DefaultCulture);
        SetCulture(selected);
    }

    private static void SetCulture(CultureInfo selected)
    {
        CultureInfo.CurrentCulture = selected;
        CultureInfo.CurrentUICulture = selected;
        CultureInfo.DefaultThreadCurrentCulture = selected;
        CultureInfo.DefaultThreadCurrentUICulture = selected;
    }
}
