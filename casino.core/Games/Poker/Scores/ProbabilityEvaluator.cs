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

        var context = new SimulationContext(
            mainPlayer,
            communityCards,
            numberOfOpponents,
            remainingDeck,
            requiredCards,
            missingCommunityCards);

        // If an RNG is provided, stay sequential to preserve test reproducibility.
        if (random is not null || simulations < 1000)
        {
            var randomizer = random ?? new CasinoRandom();
            return EstimateSequential(context, simulations, randomizer);
        }

        // Otherwise: parallel execution
        return EstimateParallel(context, simulations);
    }

    // =========================
    //       SEQUENTIAL
    // =========================

    private static double EstimateSequential(
        SimulationContext context,
        int simulations,
        IRandom randomizer)
    {
        var indices = CreateIndices(context.TotalRemaining);
        double wins = 0d;

        for (int sim = 0; sim < simulations; sim++)
        {
            wins += SimulateSingleRound(context, indices, randomizer);
        }

        return wins / simulations * 100d;
    }

    // =========================
    //       PARALLEL
    // =========================

    private static double EstimateParallel(
        SimulationContext context,
        int simulations)
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
                var localIndices = CreateIndices(context.TotalRemaining);
                double localWins = 0d;

                for (int i = 0; i < simsForWorker; i++)
                {
                    localWins += SimulateSingleRound(context, localIndices, localRandom);
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
        SimulationContext context,
        int[] indices,
        IRandom randomizer)
    {
        // Partial shuffle of indices to obtain the required cards
        ShuffleIndicesPartial(indices, context.RequiredCards, randomizer);

        int drawIndex = 0;

        var opponentHands = new HandCards[context.NumberOfOpponents];
        for (int i = 0; i < context.NumberOfOpponents; i++)
        {
            var c1 = DrawFromIndices(context.RemainingDeck, indices, ref drawIndex);
            var c2 = DrawFromIndices(context.RemainingDeck, indices, ref drawIndex);
            opponentHands[i] = new HandCards(c1, c2);
        }

        var board = BuildCommunityCardsFromIndices(context, indices, ref drawIndex);

        var playerScore = ScoreEvaluator.EvaluateScore(context.MainPlayer, board);

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
        SimulationContext context,
        int[] indices,
        ref int drawIndex)
    {
        var community = new TableCards
        {
            Flop1 = context.CommunityCards.Flop1,
            Flop2 = context.CommunityCards.Flop2,
            Flop3 = context.CommunityCards.Flop3,
            Turn = context.CommunityCards.Turn,
            River = context.CommunityCards.River
        };

        community.Flop1 ??= DrawFromIndices(context.RemainingDeck, indices, ref drawIndex);
        community.Flop2 ??= DrawFromIndices(context.RemainingDeck, indices, ref drawIndex);
        community.Flop3 ??= DrawFromIndices(context.RemainingDeck, indices, ref drawIndex);
        community.Turn ??= DrawFromIndices(context.RemainingDeck, indices, ref drawIndex);
        community.River ??= DrawFromIndices(context.RemainingDeck, indices, ref drawIndex);

        return community;
    }

    private static Card DrawFromIndices(
        IReadOnlyList<Card> remainingDeck,
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
        IRandom random)
    {
        for (int i = 0; i < requiredCards; i++)
        {
            int j = random.Next(i, indices.Length);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
    }

    internal sealed record SimulationContext(
        HandCards MainPlayer,
        TableCards CommunityCards,
        int NumberOfOpponents,
        IReadOnlyList<Card> RemainingDeck,
        int RequiredCards,
        int MissingCommunityCards)
    {
        public int TotalRemaining => RemainingDeck.Count;
    }
}
