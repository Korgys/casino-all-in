using casino.core.Games.Poker;

namespace casino.core.Games.Poker.Players.Strategies;

public static class DifficultyStrategyFactory
{
    public static IPlayerStrategy Create(PokerDifficulty difficulty) => difficulty switch
    {
        PokerDifficulty.Beginner => new AdaptiveStrategy(PokerAiProfile.Beginner),
        PokerDifficulty.VeryEasy => new AdaptiveStrategy(PokerAiProfile.VeryEasy),
        PokerDifficulty.Easy => new AdaptiveStrategy(PokerAiProfile.Easy),
        PokerDifficulty.Medium => new AdaptiveStrategy(PokerAiProfile.Medium),
        PokerDifficulty.Hard => new AdaptiveStrategy(PokerAiProfile.Hard),
        PokerDifficulty.VeryHard => new AdaptiveStrategy(PokerAiProfile.VeryHard),
        _ => new RandomStrategy()
    };
}
