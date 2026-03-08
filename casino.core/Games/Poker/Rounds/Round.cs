using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace casino.core.Games.Poker.Rounds;

public class Round
{
    internal IDeck Deck { get; }
    internal IActionService ActionService { get; }
    public List<Player> Players { get; set; }
    public TableCards CommunityCards { get; set; } = new TableCards();
    public IReadOnlyList<Player> Winners { get; private set; } = new List<Player>();
    public Phase Phase { get; set; } = Phase.PreFlop;
    public IPhaseState PhaseState { get; internal set; } = new PreFlopPhaseState();
    public int Pot { get; set; } = 0;
    public int StartingBet { get; set; } = 10;
    public int CurrentBet { get; internal set; }
    public int NumberOfRoundsPlayed { get; set; } = 0;

    private readonly Dictionary<Player, int> _betsByPlayer = new();

    public Round(List<Player> Players, IDeck deck, IActionService? actionService = null)
    {
        this.Players = Players ?? throw new ArgumentNullException(nameof(Players));
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

    public void SetBetForPlayer(Player Player, int bet)
    {
        SetBetFor(Player, bet);
    }

    public int GetBetForPlayer(Player Player)
    {
        return GetBetFor(Player);
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
