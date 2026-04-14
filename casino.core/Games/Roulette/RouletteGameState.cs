namespace casino.core.Games.Roulette;

public class RouletteGameState
{
    public int Credits { get; init; }
    public int CurrentBet { get; init; }
    public int LastPayout { get; init; }
    public int MinBet { get; init; }
    public int MaxBet { get; init; }
    public int TotalSpins { get; init; }
    public int WinningSpins { get; init; }
    public int BiggestPayout { get; init; }
    public int? CurrentPocket { get; init; }
    public RouletteBetKind BetKind { get; init; }
    public int? SelectedNumber { get; init; }
    public string BetSummary { get; init; } = string.Empty;
    public bool IsSpinning { get; init; }
    public bool IsRoundOver { get; init; }
    public bool IsWinningBet { get; init; }
    public string StatusMessage { get; init; } = string.Empty;
}
