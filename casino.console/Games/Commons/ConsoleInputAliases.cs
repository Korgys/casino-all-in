namespace casino.console.Games.Commons;

internal static class ConsoleInputAliases
{
    private static readonly HashSet<string> YesChoices = new(StringComparer.OrdinalIgnoreCase)
    {
        "y",
        "yes",
        "o",
        "oui",
        "j",
        "ja",
        "s",
        "si",
        "sí",
        "da",
        "да",
        "hai",
        "はい",
        "shi",
        "是"
    };

    private static readonly HashSet<string> HitChoices = new(StringComparer.OrdinalIgnoreCase)
    {
        "1",
        "h",
        "hit",
        "tirer",
        "ziehen",
        "pedir",
        "vzyat",
        "взять",
        "hitto",
        "ヒット",
        "yaopai",
        "要牌"
    };

    private static readonly HashSet<string> StandChoices = new(StringComparer.OrdinalIgnoreCase)
    {
        "2",
        "s",
        "stand",
        "rester",
        "stehen",
        "plantarse",
        "stoyat",
        "стоять",
        "sutando",
        "スタンド",
        "tingpai",
        "停牌"
    };

    private static readonly HashSet<string> BackChoices = new(StringComparer.OrdinalIgnoreCase)
    {
        "b",
        "back",
        "cancel",
        "retour",
        "zuruck",
        "zurück",
        "volver",
        "atras",
        "atrás",
        "nazad",
        "назад",
        "modoru",
        "戻る",
        "fanhui",
        "返回"
    };

    public static bool IsYes(string? input) => IsAlias(input, YesChoices);

    public static bool IsHit(string? input) => IsAlias(input, HitChoices);

    public static bool IsStand(string? input) => IsAlias(input, StandChoices);

    public static bool IsBack(string? input) => IsAlias(input, BackChoices);

    private static bool IsAlias(string? input, HashSet<string> aliases)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return aliases.Contains(input.Trim());
    }
}
