using casino.core.Common.Utils;
using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Poker.Scores;

public static class ProbabilityEvaluator
{
    private const int TOTAL_TABLE_CARDS = 5;

    /// <summary>
    /// Estimates the probability (0..100) that the Player wins the round against a number of opponents.
    /// Ties count as a proportional share of the win (e.g.: 3-way split => 33.33%).
    /// 
    /// - If an IRandom is provided, execution is sequential (useful for tests).
    /// - Otherwise, above a certain number of simulations, execution is parallelized.
    /// </summary>
    public static double EstimateWinProbability(
        HandCards mainPlayer,
        TableCards communityCards,
        int numberOfOpponents,
        int simulations = 2000,
        IRandom? random = null)
    {
        ArgumentNullException.ThrowIfNull(mainPlayer);
        ArgumentNullException.ThrowIfNull(communityCards);
        ArgumentOutOfRangeException.ThrowIfLessThan(numberOfOpponents, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(simulations, 1);

        if (numberOfOpponents == 0)
            return 100d;

        // --- Known cards (hand + partial board) ---
        var knownCards = new List<Card>(7)
        {
            mainPlayer.First,
            mainPlayer.Second
        };

        if (communityCards.Flop1 is not null) knownCards.Add(communityCards.Flop1);
        if (communityCards.Flop2 is not null) knownCards.Add(communityCards.Flop2);
        if (communityCards.Flop3 is not null) knownCards.Add(communityCards.Flop3);
        if (communityCards.Turn is not null) knownCards.Add(communityCards.Turn);
        if (communityCards.River is not null) knownCards.Add(communityCards.River);

        ValidateKnownCards(knownCards);

        int knownCommunityCards = 0;
        if (communityCards.Flop1 is not null) knownCommunityCards++;
        if (communityCards.Flop2 is not null) knownCommunityCards++;
        if (communityCards.Flop3 is not null) knownCommunityCards++;
        if (communityCards.Turn is not null) knownCommunityCards++;
        if (communityCards.River is not null) knownCommunityCards++;

        int missingCommunityCards = TOTAL_TABLE_CARDS - knownCommunityCards;
        int requiredCards = missingCommunityCards + numberOfOpponents * 2;

        var remainingDeck = BuildRemainingDeck(knownCards);
        if (requiredCards > remainingDeck.Count)
            throw new ArgumentException("Not enough remaining cards to simulate the round.");

        int totalRemaining = remainingDeck.Count;

        // If an RNG is provided, stay sequential to preserve test reproducibility.
        if (random is not null || simulations < 1000)
        {
            var randomizer = random ?? new CasinoRandom();
            return EstimateSequential(
                mainPlayer,
                communityCards,
                numberOfOpponents,
                simulations,
                remainingDeck,
                totalRemaining,
                requiredCards,
                randomizer);
        }

        // Otherwise: parallel execution
        return EstimateParallel(
            mainPlayer,
            communityCards,
            numberOfOpponents,
            simulations,
            remainingDeck,
            totalRemaining,
            requiredCards);
    }

    // =========================
    //       SEQUENTIAL
    // =========================

    private static double EstimateSequential(
        HandCards mainPlayer,
        TableCards communityCards,
        int numberOfOpponents,
        int simulations,
        List<Card> remainingDeck,
        int totalRemaining,
        int requiredCards,
        IRandom randomizer)
    {
        var indices = CreateIndices(totalRemaining);
        double wins = 0d;

        for (int sim = 0; sim < simulations; sim++)
        {
            wins += SimulateSingleRound(
                mainPlayer,
                communityCards,
                numberOfOpponents,
                remainingDeck,
                indices,
                requiredCards,
                totalRemaining,
                randomizer);
        }

        return wins / simulations * 100d;
    }

    // =========================
    //       PARALLEL
    // =========================

    private static double EstimateParallel(
        HandCards mainPlayer,
        TableCards communityCards,
        int numberOfOpponents,
        int simulations,
        List<Card> remainingDeck,
        int totalRemaining,
        int requiredCards)
    {
        int maxWorkers = Environment.ProcessorCount;
        int workers = Math.Min(maxWorkers, simulations);

        double totalWins = 0d;
        object sync = new();

        System.Threading.Tasks.Parallel.For(
            fromInclusive: 0,
            toExclusive: workers,
            workerId =>
            {
                int simsForWorker = simulations / workers;
                if (workerId < simulations % workers)
                    simsForWorker++;

                if (simsForWorker <= 0)
                    return;

                var localRandom = new CasinoRandom();
                var localIndices = CreateIndices(totalRemaining);
                double localWins = 0d;

                for (int i = 0; i < simsForWorker; i++)
                {
                    localWins += SimulateSingleRound(
                        mainPlayer,
                        communityCards,
                        numberOfOpponents,
                        remainingDeck,
                        localIndices,
                        requiredCards,
                        totalRemaining,
                        localRandom);
                }

                lock (sync)
                {
                    totalWins += localWins;
                }
            });

        return totalWins / simulations * 100d;
    }

    // =========================
    //       SIMULATION CORE
    // =========================

    private static double SimulateSingleRound(
        HandCards mainPlayer,
        TableCards communityCards,
        int numberOfOpponents,
        List<Card> remainingDeck,
        int[] indices,
        int requiredCards,
        int totalRemaining,
        IRandom randomizer)
    {
        // Partial shuffle of indices to obtain the required cards
        ShuffleIndicesPartial(indices, requiredCards, totalRemaining, randomizer);

        int drawIndex = 0;

        var opponentHands = new HandCards[numberOfOpponents];
        for (int i = 0; i < numberOfOpponents; i++)
        {
            var c1 = remainingDeck[indices[drawIndex++]];
            var c2 = remainingDeck[indices[drawIndex++]];
            opponentHands[i] = new HandCards(c1, c2);
        }

        var board = BuildCommunityCardsFromIndices(
            communityCards,
            remainingDeck,
            indices,
            ref drawIndex);

        var playerScore = ScoreEvaluator.EvaluateScore(mainPlayer, board);

        var bestScore = playerScore;
        int winners = 1;

        for (int i = 0; i < opponentHands.Length; i++)
        {
            var opponentScore = ScoreEvaluator.EvaluateScore(opponentHands[i], board);
            int cmp = opponentScore.CompareTo(bestScore);

            if (cmp > 0)
            {
                bestScore = opponentScore;
                winners = 1;
            }
            else if (cmp == 0)
            {
                winners++;
            }
        }

        if (playerScore.CompareTo(bestScore) == 0)
            return 1d / winners;

        return 0d;
    }

    // =========================
    //       UTILITIES
    // =========================

    private static void ValidateKnownCards(IEnumerable<Card> knownCards)
    {
        var seen = new HashSet<Card>();

        foreach (var card in knownCards)
        {
            if (!seen.Add(card))
                throw new ArgumentException("Provided cards contain duplicates.");
        }
    }

    private static List<Card> BuildRemainingDeck(IEnumerable<Card> knownCards)
    {
        var alreadyKnown = new HashSet<Card>(knownCards);
        var deck = new List<Card>(capacity: 52);

        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (CardRank rank in Enum.GetValues<CardRank>())
            {
                var card = new Card(rank, suit);
                if (!alreadyKnown.Contains(card))
                    deck.Add(card);
            }
        }

        return deck;
    }

    private static TableCards BuildCommunityCardsFromIndices(
        TableCards existing,
        List<Card> remainingDeck,
        int[] indices,
        ref int drawIndex)
    {
        var community = new TableCards
        {
            Flop1 = existing.Flop1,
            Flop2 = existing.Flop2,
            Flop3 = existing.Flop3,
            Turn = existing.Turn,
            River = existing.River
        };

        community.Flop1 ??= DrawFromIndices(remainingDeck, indices, ref drawIndex);
        community.Flop2 ??= DrawFromIndices(remainingDeck, indices, ref drawIndex);
        community.Flop3 ??= DrawFromIndices(remainingDeck, indices, ref drawIndex);
        community.Turn ??= DrawFromIndices(remainingDeck, indices, ref drawIndex);
        community.River ??= DrawFromIndices(remainingDeck, indices, ref drawIndex);

        return community;
    }

    private static Card DrawFromIndices(
        List<Card> remainingDeck,
        int[] indices,
        ref int drawIndex)
    {
        if (drawIndex >= indices.Length)
            throw new InvalidOperationException("No more cards in the deck.");

        return remainingDeck[indices[drawIndex++]];
    }

    private static int[] CreateIndices(int n)
    {
        var indices = new int[n];
        for (int i = 0; i < n; i++)
        {
            indices[i] = i;
        }
        return indices;
    }

    /// <summary>
    /// Partial Fisher-Yates shuffle: only randomizes the first requiredCards positions.
    /// </summary>
    private static void ShuffleIndicesPartial(
        int[] indices,
        int requiredCards,
        int totalSize,
        IRandom random)
    {
        for (int i = 0; i < requiredCards; i++)
        {
            int j = random.Next(i, totalSize);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
    }
}