using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace casino.core.Games.Poker.Rounds;

public class Round
{
    internal IDeck Deck { get; }
    internal IActionService ActionService { get; }
    private readonly List<Player> _players;
    private readonly ReadOnlyCollection<Player> _readOnlyPlayers;

    public IReadOnlyList<Player> Players => _readOnlyPlayers;
    public TableCards CommunityCards { get; private set; } = new TableCards();
    public IReadOnlyList<Player> Winners { get; private set; } = new List<Player>();
    public Phase Phase { get; private set; } = Phase.PreFlop;
    public IPhaseState PhaseState { get; internal set; } = new PreFlopPhaseState();
    public int Pot { get; private set; } = 0;
    public int StartingBet { get; internal set; } = 10;
    public int CurrentBet { get; internal set; }
    public int NumberOfRoundsPlayed { get; internal set; } = 0;

    private readonly Dictionary<Player, int> _betsByPlayer = new();

    public Round(IReadOnlyList<Player> Players, IDeck deck, IActionService? actionService = null)
    {
        _players = Players?.ToList() ?? throw new ArgumentNullException(nameof(Players));
        _readOnlyPlayers = _players.AsReadOnly();
        Deck = deck ?? throw new ArgumentNullException(nameof(deck));
        ActionService = actionService ?? new ActionService();
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

    public IEnumerable<PokerTypeAction> GetAvailableActions(Player Player)
    {
        return PhaseState.GetAvailableActions(Player, this);
    }

    public void ApplyAction(Player Player, Actions.GameAction action)
    {
        PhaseState.ApplyAction(Player, action, this);
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
        foreach (var Player in Players.Where(j => j.Chips > 0 && j.LastAction != PokerTypeAction.Fold))
        {
            Player.Hand = new HandCards(Deck.DrawCard(), Deck.DrawCard());
        }
    }

    internal int GetBetFor(Player Player)
    {
        return _betsByPlayer.TryGetValue(Player, out var bet) ? bet : 0;
    }

    internal void SetBetFor(Player Player, int bet)
    {
        _betsByPlayer[Player] = bet;
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

        foreach (var Player in Players)
        {
            _betsByPlayer[Player] = 0;

            if (!Player.IsFolded() && !Player.IsAllIn())
            {
                Player.LastAction = PokerTypeAction.None;
            }
        }
    }

    private void InitializePlayerBets()
    {
        foreach (var Player in Players)
        {
            _betsByPlayer[Player] = 0;
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
            DistributePot(winners);
        }

        // Increase the starting bet every N rounds
        NumberOfRoundsPlayed++;
        if (NumberOfRoundsPlayed % Players.Count == 0)
            StartingBet *= 2;
    }

    private void DistributePot(IReadOnlyList<Player> winners)
    {
        if (winners.Count == 0)
            return;

        int share = Pot / winners.Count;
        int remainder = Pot % winners.Count;

        foreach (var winner in winners)
            winner.Chips += share;

        // Distribute the remainder deterministically (simple)
        for (int i = 0; i < remainder; i++)
            winners[i].Chips += 1;
    }
}
