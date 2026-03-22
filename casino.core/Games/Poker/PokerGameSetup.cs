namespace casino.core.Games.Poker;

public sealed record PokerGameSetup(int InitialChips, int PlayerCount, IReadOnlyList<PokerOpponentSetup> Opponents)
{
    public static PokerGameSetup CreateDefault() =>
        new(1000, 5,
        [
            new PokerOpponentSetup(PokerDifficulty.Hard),
            new PokerOpponentSetup(PokerDifficulty.Expert),
            new PokerOpponentSetup(PokerDifficulty.Medium),
            new PokerOpponentSetup(PokerDifficulty.Easy)
        ]);
}

public sealed record PokerOpponentSetup(PokerDifficulty Difficulty)
{
    public string Label => Difficulty switch
    {
        PokerDifficulty.Easy => "Facile",
        PokerDifficulty.Medium => "Moyen",
        PokerDifficulty.Hard => "Difficile",
        PokerDifficulty.Expert => "Expert",
        _ => Difficulty.ToString()
    };
}

public enum PokerDifficulty
{
    Easy = 1,
    Medium = 2,
    Hard = 3,
    Expert = 4,
}
