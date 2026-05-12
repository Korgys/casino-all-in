using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Poker.Scores;

public static class ScoreEvaluator
{
    private sealed record RankGroup(CardRank Rank, int Count);
    private sealed record SuitGroup(Suit Suit, Card[] Cards);

    public static Score EvaluateScore(HandCards main, TableCards communityCards)
    {
        ArgumentNullException.ThrowIfNull(main);
        ArgumentNullException.ThrowIfNull(communityCards);

        // Hold'em: 2 private cards + 0..5 community cards => 2..7 cards
        var cards = main.AsEnumerable().Concat(communityCards.AsEnumerable()).ToArray();

        var distinctRanksDesc = cards
            .Select(c => c.Rank)
            .Distinct()
            .OrderByDescending(r => r)
            .ToArray();
        var rankGroups = cards
            .GroupBy(c => c.Rank)
            .Select(g => new RankGroup(g.Key, g.Count()))
            .ToArray();
        var suitGroups = cards
            .GroupBy(c => c.Suit)
            .Select(g => new SuitGroup(g.Key, g.ToArray()))
            .ToArray();

        // Standard poker ranking order (strongest to weakest)

        // Royal straight flush
        if (ContainsRoyalStraightFlush(suitGroups))
            return new Score(HandRank.RoyalFlush, CardRank.Ace, kickers: Array.Empty<CardRank>());

        // Straight flush
        if (GetStraightFlushValue(suitGroups) is CardRank straightFlushValue)
            return new Score(HandRank.StraightFlush, straightFlushValue, kickers: Array.Empty<CardRank>());

        // Four of a kind
        if (GetFourOfAKindValue(rankGroups) is CardRank fourOfAKindValue)
        {
            var kicker = DetermineBestKickers(distinctRanksDesc, excludedRanks: new[] { fourOfAKindValue }, count: 1);
            return new Score(HandRank.FourOfAKind, fourOfAKindValue, kicker);
        }

        // Full house
        if (GetFullHouseValues(rankGroups, out var fullHouseThreeOfAKind, out var fullHousePair))
            return new Score(HandRank.FullHouse, fullHouseThreeOfAKind, new[] { fullHousePair });

        // Flush
        if (TryGetFlushTop5(suitGroups, out var flushTop5))
            return new Score(HandRank.Flush, flushTop5[0], flushTop5.Skip(1).ToArray());

        // Straight
        if (GetStraightValue(distinctRanksDesc) is CardRank straightValue)
            return new Score(HandRank.Straight, straightValue, kickers: Array.Empty<CardRank>());

        // Three of a kind
        if (GetThreeOfAKindValue(rankGroups) is CardRank threeOfAKindValue)
        {
            var kickers = DetermineBestKickers(distinctRanksDesc, excludedRanks: new[] { threeOfAKindValue }, count: 2);
            return new Score(HandRank.ThreeOfAKind, threeOfAKindValue, kickers);
        }

        // Double pair
        if (GetTwoPairValues(rankGroups, out var higherPair, out var lowerPair))
        {
            var kicker = DetermineBestKickers(distinctRanksDesc, excludedRanks: new[] { higherPair, lowerPair }, count: 1);
            return new Score(HandRank.TwoPair, higherPair, new[] { lowerPair }.Concat(kicker).ToArray());
        }

        // One pair
        if (GetPairValue(rankGroups) is CardRank pairValue)
        {
            var kickers = DetermineBestKickers(distinctRanksDesc, excludedRanks: new[] { pairValue }, count: 3);
            return new Score(HandRank.OnePair, pairValue, kickers);
        }

        // High card
        if (distinctRanksDesc.Length == 0)
            return new Score(HandRank.HighCard, CardRank.Two, Array.Empty<CardRank>());

        var highestHighCard = distinctRanksDesc[0];
        var highCardKickers = distinctRanksDesc.Skip(1).Take(4).ToArray();
        return new Score(HandRank.HighCard, highestHighCard, highCardKickers);
    }

    /// <summary>
    /// Takes the best ranks excluding provided values, already sorted descending (distinctRanksDesc).
    /// Returns a concrete array (no lazy enumerable).
    /// </summary>
    private static CardRank[] DetermineBestKickers(
        CardRank[] distinctRanksDesc,
        CardRank[] excludedRanks,
        int count)
    {
        if (count <= 0) return Array.Empty<CardRank>();
        if (excludedRanks.Length == 0) return distinctRanksDesc.Take(count).ToArray();

        var excluded = excludedRanks.ToHashSet();
        var result = new List<CardRank>(capacity: count);
        for (int i = 0; i < distinctRanksDesc.Length && result.Count < count; i++)
        {
            var r = distinctRanksDesc[i];
            if (!excluded.Contains(r))
                result.Add(r);
        }

        return result.ToArray();
    }

    private static bool TryGetFlushTop5(IEnumerable<SuitGroup> groupsBySuit, out CardRank[] top5)
    {
        foreach (var grp in groupsBySuit)
        {
            if (grp.Cards.Length < 5) continue;

            var ranks = grp.Cards.Select(c => c.Rank)
                           .Distinct()
                           .OrderByDescending(r => r)
                           .Take(5)
                           .ToArray();

            if (ranks.Length == 5)
            {
                top5 = ranks;
                return true;
            }
        }

        top5 = Array.Empty<CardRank>();
        return false;
    }

    private static bool ContainsRoyalStraightFlush(IEnumerable<SuitGroup> groupsBySuit)
    {
        // A straight flush whose high card is Ace (10-J-Q-K-A).
        foreach (var grp in groupsBySuit)
        {
            if (grp.Cards.Length < 5) continue;

            var distinctRanksDesc = grp.Cards.Select(c => c.Rank)
                                       .Distinct()
                                       .OrderByDescending(r => r)
                                       .ToArray();

            // For "royal", the straight high card must be Ace and Ten must be present.
            var high = GetStraightValue(distinctRanksDesc);
            if (high == CardRank.Ace && distinctRanksDesc.Contains(CardRank.Ten))
                return true;
        }

        return false;
    }

    private static CardRank? GetStraightFlushValue(IEnumerable<SuitGroup> groupsBySuit)
    {
        CardRank? best = null;

        foreach (var grp in groupsBySuit)
        {
            if (grp.Cards.Length < 5) continue;

            var distinctRanksDesc = grp.Cards.Select(c => c.Rank)
                                       .Distinct()
                                       .OrderByDescending(r => r)
                                       .ToArray();

            var high = GetStraightValue(distinctRanksDesc);
            if (high.HasValue && (!best.HasValue || high.Value > best.Value))
                best = high;
        }

        return best;
    }

    private static CardRank? GetFourOfAKindValue(IEnumerable<RankGroup> groupsByRank)
    {
        return groupsByRank
            .Where(g => g.Count == 4)
            .Select(g => g.Rank)
            .OrderByDescending(r => r)
            .Cast<CardRank?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Full house:
    /// - threeOfAKind = highest three of a kind
    /// - pair = highest pair excluding the selected three of a kind, or second three of a kind used as pair
    /// </summary>
    private static bool GetFullHouseValues(
        IEnumerable<RankGroup> groupsByRank,
        out CardRank threeOfAKind,
        out CardRank pair)
    {
        threeOfAKind = default;
        pair = default;

        var threeOfAKinds = groupsByRank
            .Where(g => g.Count >= 3)
            .Select(g => g.Rank)
            .OrderByDescending(r => r)
            .ToArray();

        if (threeOfAKinds.Length == 0)
            return false;

        threeOfAKind = threeOfAKinds[0];

        var pairs = groupsByRank
            .Where(g => g.Rank != threeOfAKinds[0] && g.Count >= 2)
            .Select(g => g.Rank)
            .OrderByDescending(r => r)
            .ToArray();

        if (pairs.Length > 0)
        {
            pair = pairs[0];
            return true;
        }

        // Special case: two three-of-a-kind groups => second one becomes the pair.
        if (threeOfAKinds.Length >= 2)
        {
            pair = threeOfAKinds[1];
            return true;
        }

        return false;
    }

    private static CardRank? GetThreeOfAKindValue(IEnumerable<RankGroup> groupsByRank)
    {
        return groupsByRank
            .Where(g => g.Count >= 3)
            .Select(g => g.Rank)
            .OrderByDescending(r => r)
            .Cast<CardRank?>()
            .FirstOrDefault();
    }

    private static bool GetTwoPairValues(
        IEnumerable<RankGroup> groupsByRank,
        out CardRank higherPair,
        out CardRank lowerPair)
    {
        higherPair = default;
        lowerPair = default;

        var pairs = groupsByRank
            .Where(g => g.Count >= 2)
            .Select(g => g.Rank)
            .OrderByDescending(r => r)
            .ToArray();

        if (pairs.Length < 2)
            return false;

        higherPair = pairs[0];
        lowerPair = pairs[1];
        return true;
    }

    private static CardRank? GetPairValue(IEnumerable<RankGroup> groupsByRank)
    {
        return groupsByRank
            .Where(g => g.Count >= 2)
            .Select(g => g.Rank)
            .OrderByDescending(r => r)
            .Cast<CardRank?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Returns the high card of a straight from distinct ranks sorted descending.
    /// Handles A-2-3-4-5 => high = 5.
    /// Returns null if no straight exists.
    /// </summary>
    private static CardRank? GetStraightValue(IReadOnlyList<CardRank> distinctRanksDesc)
    {
        // Convert to ascending ints for straight detection.
        var values = distinctRanksDesc
            .Select(r => (int)r)
            .OrderBy(v => v)
            .ToList();

        if (values.Count < 5)
            return null;

        // Wheel straight: if Ace (14) exists, insert 1 to represent low Ace
        if (values.Contains((int)CardRank.Ace))
            values.Insert(0, 1);

        int consecutive = 1;
        int bestHigh = -1;

        for (int i = 1; i < values.Count; i++)
        {
            if (values[i] == values[i - 1] + 1)
            {
                consecutive++;
                if (consecutive >= 5)
                    bestHigh = values[i];
            }
            else
            {
                consecutive = 1;
            }
        }

        return bestHigh == -1 ? null : (CardRank)bestHigh;
    }
}
