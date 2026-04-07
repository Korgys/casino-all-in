namespace casino.core.Games.Poker;

public sealed record PokerGameSetup(int InitialChips, int PlayerCount, IReadOnlyList<PokerOpponentSetup> Opponents)
{
    /// <summary>
    /// Default setup table for a poker game with 
    /// 1000 initial chips, 6 players, and opponents of varying difficulties.
    /// </summary>
    /// <returns></returns>
    public static PokerGameSetup CreateDefault() =>
        new(1000, 6,
        [
            new PokerOpponentSetup(PokerDifficulty.Beginner),
            new PokerOpponentSetup(PokerDifficulty.VeryEasy),
            new PokerOpponentSetup(PokerDifficulty.Easy),
            new PokerOpponentSetup(PokerDifficulty.Medium),
            new PokerOpponentSetup(PokerDifficulty.Hard)
        ]);
}

public sealed record PokerOpponentSetup(PokerDifficulty Difficulty)
{
    public string Label => Difficulty switch
    {
        PokerDifficulty.Beginner => "Débutant",
        PokerDifficulty.VeryEasy => "Très facile",
        PokerDifficulty.Easy => "Facile",
        PokerDifficulty.Medium => "Moyen",
        PokerDifficulty.Hard => "Difficile",
        PokerDifficulty.VeryHard => "Très difficile",
        _ => Difficulty.ToString()
    };
}

public enum PokerDifficulty
{
    Beginner = 1,
    VeryEasy = 2,
    Easy = 3,
    Medium = 4,
    Hard = 5,
    VeryHard = 6,
}
