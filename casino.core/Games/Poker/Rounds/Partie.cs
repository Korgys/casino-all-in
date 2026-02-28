using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties.Phases;
using casino.core.Games.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace casino.core.Games.Poker.Parties;

public class Partie
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
    public int NombrePartiesJouees { get; set; } = 0;

    private readonly Dictionary<Player, int> _misesParPlayer = new();

    public Partie(List<Player> Players, IDeck deck, IActionService? actionService = null)
    {
        this.Players = Players ?? throw new ArgumentNullException(nameof(Players));
        Deck = deck ?? throw new ArgumentNullException(nameof(deck));
        ActionService = actionService ?? new ActionService();
        Deck.Melanger();
        DealCards();
        InitializePlayerBets();
    }

    public void AvancerPhase()
    {
        PhaseState.Avancer(this);
        if (Phase != Phase.Showdown)
        {
            ResetBetsAndActions();
        }
    }

    public bool EnCours() => Phase != Phase.Showdown;

    public IEnumerable<TypeActionJeu> ObtenirActionsPossibles(Player Player)
    {
        return PhaseState.ObtenirActionsPossibles(Player, this);
    }

    public void AppliquerAction(Player Player, Actions.ActionJeu action)
    {
        PhaseState.AppliquerAction(Player, action, this);
    }

    private void DealCards()
    {
        foreach (var Player in Players.Where(j => j.Chips > 0 && j.LastAction != TypeActionJeu.SeCoucher))
        {
            Player.Hand = new HandCards(Deck.TirerCarte(), Deck.TirerCarte());
        }
    }

    internal int GetBetFor(Player Player)
    {
        return _misesParPlayer.TryGetValue(Player, out var mise) ? mise : 0;
    }

    internal void SetBetFor(Player Player, int mise)
    {
        _misesParPlayer[Player] = mise;
    }

    public void DefinirMisePour(Player Player, int mise)
    {
        SetBetFor(Player, mise);
    }

    internal bool IsBettingRoundClosed()
    {
        if (!EnCours())
        {
            return true;
        }

        var PlayersNonCouches = Players.Where(j => !j.IsFolded()).ToList();

        if (!PlayersNonCouches.Any())
        {
            return true;
        }

        var miseMax = PlayersNonCouches.Max(GetBetFor);

        return PlayersNonCouches.All(j =>
            j.IsAllIn() ||
            (GetBetFor(j) == miseMax && j.LastAction != TypeActionJeu.Aucune));
    }

    internal void ResetBetsAndActions()
    {
        CurrentBet = 0;

        foreach (var Player in Players)
        {
            _misesParPlayer[Player] = 0;

            if (!Player.IsFolded() && !Player.IsAllIn())
            {
                Player.LastAction = TypeActionJeu.Aucune;
            }
        }
    }

    private void InitializePlayerBets()
    {
        foreach (var Player in Players)
        {
            _misesParPlayer[Player] = 0;
        }
    }

    public void EndGame()
    {
        Phase = Phase.Showdown;

        // Winner by fold
        if (Players.Count(j => !j.IsFolded()) == 1)
        {
            var gagnant = Players.First(j => !j.IsFolded());
            Winners = new List<Player> { gagnant };
            gagnant.Chips += Pot;
            return;
        }

        // Gagnants par la meilleure main (peut �tre une �galit�)
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(Players, CommunityCards);
        Winners = gagnants;

        DistributePot(gagnants);

        // Augmente la mise de d�part toutes les N parties
        NombrePartiesJouees++;
        if (NombrePartiesJouees % Players.Count == 0)
            StartingBet *= 2;
    }

    private void DistributePot(IReadOnlyList<Player> gagnants)
    {
        if (gagnants.Count == 0)
            return;

        int part = Pot / gagnants.Count;
        int reste = Pot % gagnants.Count;

        foreach (var g in gagnants)
            g.Chips += part;

        // Distribuer le reste de mani�re d�terministe (simple)
        for (int i = 0; i < reste; i++)
            gagnants[i].Chips += 1;
    }
}
