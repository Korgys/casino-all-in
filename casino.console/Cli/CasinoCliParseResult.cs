namespace casino.console.Cli;

internal sealed record CasinoCliParseResult(CasinoCliCommand? Command, bool IsHelp, string? Error)
{
    public bool IsSuccess => Command is not null && Error is null;

    public static CasinoCliParseResult Success(CasinoCliCommand command) => new(command, IsHelp: false, Error: null);

    public static CasinoCliParseResult Help() => new(Command: null, IsHelp: true, Error: null);

    public static CasinoCliParseResult Failure(string error) => new(Command: null, IsHelp: false, Error: error);
}
