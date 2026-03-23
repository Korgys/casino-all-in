namespace casino.core.Games.Slots;

public class SlotMachineGameState
{
    public IReadOnlyList<SlotSymbol> Reels { get; init; } = [];
    public int Credits { get; init; }
    public int CurrentBet { get; init; }
    public int LastPayout { get; init; }
    public int MinBet { get; init; }
    public int MaxBet { get; init; }
    public int TotalSpins { get; init; }
    public int WinningSpins { get; init; }
    public int BiggestPayout { get; init; }
    public bool IsSpinning { get; init; }
    public bool IsRoundOver { get; init; }
    public bool IsJackpot { get; init; }
    public string StatusMessage { get; init; } = string.Empty;
}
