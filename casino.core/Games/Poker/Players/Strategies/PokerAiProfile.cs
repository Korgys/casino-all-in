namespace casino.core.Games.Poker.Players.Strategies;

public sealed record PokerAiProfile(
    double FoldThresholdFactor,
    double CallThresholdFactor,
    double RaiseThresholdFactor,
    double AllInThresholdFactor,
    double BluffFrequency,
    double RaiseSizeMultiplier,
    bool CanAllInWhenStrong,
    bool PreferCheckWhenAvailable)
{
    public static PokerAiProfile Beginner => new(
        FoldThresholdFactor: 1.00,
        CallThresholdFactor: 1.25,
        RaiseThresholdFactor: 2.20,
        AllInThresholdFactor: 3.00,
        BluffFrequency: 0.00,
        RaiseSizeMultiplier: 1.00,
        CanAllInWhenStrong: false,
        PreferCheckWhenAvailable: true);

    public static PokerAiProfile VeryEasy => new(
        FoldThresholdFactor: 0.90,
        CallThresholdFactor: 1.15,
        RaiseThresholdFactor: 1.90,
        AllInThresholdFactor: 2.60,
        BluffFrequency: 0.03,
        RaiseSizeMultiplier: 1.10,
        CanAllInWhenStrong: false,
        PreferCheckWhenAvailable: true);

    public static PokerAiProfile Easy => new(
        FoldThresholdFactor: 0.82,
        CallThresholdFactor: 1.08,
        RaiseThresholdFactor: 1.60,
        AllInThresholdFactor: 2.20,
        BluffFrequency: 0.07,
        RaiseSizeMultiplier: 1.20,
        CanAllInWhenStrong: false,
        PreferCheckWhenAvailable: true);

    public static PokerAiProfile Medium => new(
        FoldThresholdFactor: 0.75,
        CallThresholdFactor: 1.00,
        RaiseThresholdFactor: 1.35,
        AllInThresholdFactor: 1.90,
        BluffFrequency: 0.12,
        RaiseSizeMultiplier: 1.40,
        CanAllInWhenStrong: true,
        PreferCheckWhenAvailable: false);

    public static PokerAiProfile Hard => new(
        FoldThresholdFactor: 0.68,
        CallThresholdFactor: 0.95,
        RaiseThresholdFactor: 1.22,
        AllInThresholdFactor: 1.65,
        BluffFrequency: 0.18,
        RaiseSizeMultiplier: 1.80,
        CanAllInWhenStrong: true,
        PreferCheckWhenAvailable: false);

    public static PokerAiProfile VeryHard => new(
        FoldThresholdFactor: 0.62,
        CallThresholdFactor: 0.90,
        RaiseThresholdFactor: 1.12,
        AllInThresholdFactor: 1.45,
        BluffFrequency: 0.24,
        RaiseSizeMultiplier: 2.20,
        CanAllInWhenStrong: true,
        PreferCheckWhenAvailable: false);
}
