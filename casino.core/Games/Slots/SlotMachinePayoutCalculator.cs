namespace casino.core.Games.Slots;

public static class SlotMachinePayoutCalculator
{
    public static int Calculate(IReadOnlyList<SlotSymbol> reels, int bet)
    {
        ArgumentNullException.ThrowIfNull(reels);

        if (reels.Count != 3)
            throw new ArgumentException("A slot machine spin must contain exactly three reels.", nameof(reels));

        if (bet <= 0)
            throw new ArgumentOutOfRangeException(nameof(bet), "Bet must be greater than zero.");

        if (reels.All(symbol => symbol == SlotSymbol.Seven))
            return bet * 20;

        if (reels.All(symbol => symbol == SlotSymbol.Bar))
            return bet * 12;

        if (reels.All(symbol => symbol == SlotSymbol.Bell))
            return bet * 10;

        if (reels.All(symbol => symbol == SlotSymbol.Star))
            return bet * 8;

        if (reels.All(symbol => symbol == SlotSymbol.Cherry))
            return bet * 6;

        var cherries = reels.Count(symbol => symbol == SlotSymbol.Cherry);
        return cherries switch
        {
            2 => bet * 2,
            1 => bet,
            _ => 0
        };
    }
}
