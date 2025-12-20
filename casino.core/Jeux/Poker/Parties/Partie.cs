using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties.Phases;
using casino.core.Jeux.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace casino.core.Jeux.Poker.Parties;

public class Partie
{
    internal IDeck Deck { get; }
    internal IActionService ActionService { get; }
    public List<Joueur> Joueurs { get; set; }
    public CartesCommunes CartesCommunes { get; set; } = new CartesCommunes();
    public IReadOnlyList<Joueur> Gagnants { get; private set; } = new List<Joueur>();
    public Phase Phase { get; set; } = Phase.PreFlop;
    public IPhaseState PhaseState { get; internal set; } = new PreFlopPhaseState();
    public int Pot { get; set; } = 0;
    public int MiseDeDepart { get; set; } = 10;
    public int MiseActuelle { get; internal set; }

    private readonly Dictionary<Joueur, int> _misesParJoueur = new();

    public Partie(List<Joueur> joueurs, IDeck deck, IActionService? actionService = null)
    {
        Joueurs = joueurs;
        Deck = deck;
        ActionService = actionService ?? new ActionService();
        Deck.Melanger();
        DistribuerCartes();
        InitialiserMisesJoueurs();
    }

    public void AvancerPhase()
    {
        PhaseState.Avancer(this);
        if (Phase != Phase.Showdown)
        {
            ReinitialiserMisesEtActions();
        }
    }

    public bool EnCours() => Phase != Phase.Showdown;

    public IEnumerable<TypeActionJeu> ObtenirActionsPossibles(Joueur joueur)
    {
        return PhaseState.ObtenirActionsPossibles(joueur, this);
    }

    public void AppliquerAction(Joueur joueur, Actions.ActionJeu action)
    {
        PhaseState.AppliquerAction(joueur, action, this);
    }

    private void DistribuerCartes()
    {
        foreach (var joueur in Joueurs.Where(j => j.Jetons > 0 && j.DerniereAction != TypeActionJeu.SeCoucher))
        {
            joueur.Main = new CartesMain(Deck.TirerCarte(), Deck.TirerCarte());
        }
    }

    internal int ObtenirMisePour(Joueur joueur)
    {
        return _misesParJoueur.TryGetValue(joueur, out var mise) ? mise : 0;
    }

    internal void DefinirMisePour(Joueur joueur, int mise)
    {
        _misesParJoueur[joueur] = mise;
    }

    internal bool EstTourEnchereCloture()
    {
        if (!EnCours())
        {
            return true;
        }

        var joueursActifs = Joueurs.Where(j => !j.EstCouche() && !j.EstTapis()).ToList();

        if (!joueursActifs.Any())
        {
            return true;
        }

        int miseDuTour = ObtenirMisePour(joueursActifs.First());
        return joueursActifs.All(j => ObtenirMisePour(j) == miseDuTour && j.DerniereAction != TypeActionJeu.Aucune);
    }

    internal void ReinitialiserMisesEtActions()
    {
        MiseActuelle = 0;

        foreach (var joueur in Joueurs)
        {
            _misesParJoueur[joueur] = 0;

            if (!joueur.EstCouche() && !joueur.EstTapis())
            {
                joueur.DerniereAction = TypeActionJeu.Aucune;
            }
        }
    }

    private void InitialiserMisesJoueurs()
    {
        foreach (var joueur in Joueurs)
        {
            _misesParJoueur[joueur] = 0;
        }
    }

    public void TerminerPartie()
    {
        Phase = Phase.Showdown;

        // Gagnant par abandon
        if (Joueurs.Count(j => !j.EstCouche()) == 1)
        {
            var gagnant = Joueurs.First(j => !j.EstCouche());
            Gagnants = new List<Joueur> { gagnant };
            gagnant.Jetons += Pot;
            return;
        }

        // Gagnants par la meilleure main (peut être une égalité)
        var gagnants = EvaluateurGagnant.DeterminerGagnantsParMain(Joueurs, CartesCommunes);
        Gagnants = gagnants;

        RepartirPot(gagnants);
    }

    private void RepartirPot(IReadOnlyList<Joueur> gagnants)
    {
        if (gagnants.Count == 0)
            return;

        int part = Pot / gagnants.Count;
        int reste = Pot % gagnants.Count;

        foreach (var g in gagnants)
            g.Jetons += part;

        // Distribuer le reste de manière déterministe (simple)
        for (int i = 0; i < reste; i++)
            gagnants[i].Jetons += 1;
    }
}
