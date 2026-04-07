using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Rounds;

public class Round
{
    internal IDeck Deck { get; }
    internal IActionService ActionService { get; }

    public IReadOnlyList<Player> Players { get; internal set; }
    public TableCards CommunityCards { get; private set; } = new TableCards();
    public IReadOnlyList<Player> Winners { get; private set; } = new List<Player>();
    public Phase Phase { get; private set; } = Phase.PreFlop;
    public IPhaseState PhaseState { get; internal set; } = new PreFlopPhaseState();
    public int Pot { get; private set; } = 0;
    public int StartingBet { get; internal set; } = 10;
    public int CurrentBet { get; internal set; }
    public int NumberOfRoundsPlayed { get; internal set; } = 0;

    private readonly Dictionary<Player, int> _betsByPlayer = new();
    private readonly Dictionary<Player, int> _totalContributionByPlayer = new();

    public Round(IReadOnlyList<Player> players, IDeck deck, int numberOfRoundsPlayed, IActionService? actionService = null)
    {
        Players = players?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(players));
        Deck = deck ?? throw new ArgumentNullException(nameof(deck));
        ActionService = actionService ?? new ActionService();
        NumberOfRoundsPlayed = numberOfRoundsPlayed;
        Deck.Shuffle();
        DealCards();
        InitializePlayerBets();
    }

    public void AdvancePhase()
    {
        PhaseState.Avancer(this);
        if (Phase != Phase.Showdown)
        {
            ResetBetsAndActions();
        }
    }

    public bool IsInProgress() => Phase != Phase.Showdown;

    public IEnumerable<PokerTypeAction> GetAvailableActions(Player player)
    {
        return PhaseState.GetAvailableActions(player, this);
    }

    public void ApplyAction(Player player, Actions.GameAction action)
    {
        PhaseState.ApplyAction(player, action, this);
    }

    internal void MoveToNextPhase(Phase phase, IPhaseState nextPhaseState)
    {
        Phase = phase;
        PhaseState = nextPhaseState;
    }

    internal void RevealFlop(Card flop1, Card flop2, Card flop3)
    {
        CommunityCards.Flop1 = flop1;
        CommunityCards.Flop2 = flop2;
        CommunityCards.Flop3 = flop3;
    }

    internal void RevealTurn(Card turn)
    {
        CommunityCards.Turn = turn;
    }

    internal void RevealRiver(Card river)
    {
        CommunityCards.River = river;
    }

    internal void AddToPot(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Pot += amount;
    }

    internal void AddToPot(Player player, int amount)
    {
        if (player is null)
            throw new ArgumentNullException(nameof(player));

        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        AddToPot(amount);
        _totalContributionByPlayer[player] = GetTotalContributionFor(player) + amount;
    }

    internal int GetTotalContributionFor(Player player)
    {
        return _totalContributionByPlayer.TryGetValue(player, out var contribution) ? contribution : 0;
    }

    internal void SetCurrentBet(int amount)
    {
        CurrentBet = amount;
    }

    internal void RaiseCurrentBetToAtLeast(int amount)
    {
        CurrentBet = Math.Max(CurrentBet, amount);
    }

    internal void SetCommunityCards(TableCards communityCards)
    {
        CommunityCards = communityCards ?? throw new ArgumentNullException(nameof(communityCards));
    }

    private void DealCards()
    {
        foreach (var player in Players.Where(j => j.Chips > 0 && j.LastAction != PokerTypeAction.Fold))
        {
            player.Hand = new HandCards(Deck.DrawCard(), Deck.DrawCard());
        }
    }

    internal int GetBetFor(Player player)
    {
        return _betsByPlayer.TryGetValue(player, out var bet) ? bet : 0;
    }

    internal void SetBetFor(Player player, int bet)
    {
        _betsByPlayer[player] = bet;
    }

    internal bool IsBettingRoundClosed()
    {
        if (!IsInProgress())
        {
            return true;
        }

        var activePlayers = Players.Where(j => !j.IsFolded()).ToList();

        if (!activePlayers.Any())
        {
            return true;
        }

        var maxBet = activePlayers.Max(GetBetFor);

        return activePlayers.All(j =>
            j.IsAllIn() ||
            (GetBetFor(j) == maxBet && j.LastAction != PokerTypeAction.None));
    }

    internal void ResetBetsAndActions()
    {
        CurrentBet = 0;

        foreach (var player in Players)
        {
            _betsByPlayer[player] = 0;

            if (!player.IsFolded() && !player.IsAllIn())
            {
                player.LastAction = PokerTypeAction.None;
            }
        }
    }

    private void InitializePlayerBets()
    {
        foreach (var player in Players)
        {
            _betsByPlayer[player] = 0;
            _totalContributionByPlayer[player] = 0;
        }
    }

    public void EndGame()
    {
        Phase = Phase.Showdown;

        // Winner by fold
        if (Players.Count(j => !j.IsFolded()) == 1)
        {
            var winner = Players.First(j => !j.IsFolded());
            Winners = new List<Player> { winner };
            winner.Chips += Pot;
        }

        // Winners by best hand (can be a tie)
        else
        {
            var winners = WinnerEvaluator.DetermineWinnersByHand(Players, CommunityCards);
            Winners = winners;
            DistributePot();
        }

        // Increase the starting bet every 5 rounds
        NumberOfRoundsPlayed++;
        if (NumberOfRoundsPlayed % 5 == 0)
            StartingBet *= 2;
    }

    private void DistributePot()
    {
        var playersWithContribution = Players
            .Where(player => GetTotalContributionFor(player) > 0)
            .ToList();

        if (playersWithContribution.Count == 0 || playersWithContribution.Sum(GetTotalContributionFor) != Pot)
        {
            DistributeAmountAcrossWinners(Pot, Winners);
            return;
        }

        var contributionLevels = playersWithContribution
            .Select(GetTotalContributionFor)
            .Distinct()
            .OrderBy(level => level)
            .ToList();

        var lowerBound = 0;
        foreach (var upperBound in contributionLevels)
        {
            var potContributors = playersWithContribution
                .Where(player => GetTotalContributionFor(player) >= upperBound)
                .ToList();

            var potAmount = (upperBound - lowerBound) * potContributors.Count;
            lowerBound = upperBound;

            if (potAmount <= 0)
            {
                continue;
            }

            var eligiblePlayers = potContributors
                .Where(player => !player.IsFolded())
                .ToList();

            if (eligiblePlayers.Count == 0)
            {
                continue;
            }

            var potWinners = WinnerEvaluator.DetermineWinnersByHand(eligiblePlayers, CommunityCards);
            DistributeAmountAcrossWinners(potAmount, potWinners);
        }
    }

    private void DistributeAmountAcrossWinners(int amount, IReadOnlyList<Player> winners)
    {
        if (amount <= 0 || winners.Count == 0)
            return;

        int share = amount / winners.Count;
        int remainder = amount % winners.Count;

        foreach (var winner in winners)
            winner.Chips += share;

        // Deterministic remainder rule: extra chips are awarded by table order.
        var orderedWinners = winners
            .OrderBy(winner => Players.ToList().IndexOf(winner))
            .ToList();

        for (int i = 0; i < remainder; i++)
            orderedWinners[i].Chips += 1;
    }
}
