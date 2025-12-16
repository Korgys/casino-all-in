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
    public Joueur Gagnant { get; set; }
    public Phase Phase { get; set; } = Phase.PreFlop;
    public IPhaseState PhaseState { get; internal set; } = new PreFlopPhaseState();
    public int Pot { get; set; } = 0;
    public int MiseDeDepart { get; set; } = 10;
    public int MiseActuelle { get; internal set; }

    public Partie(List<Joueur> joueurs, IDeck deck, IActionService? actionService = null)
    {
        Joueurs = joueurs;
        Deck = deck;
        ActionService = actionService ?? new ActionService();
        Deck.Melanger();
        DistribuerCartes();
    }

    public void AvancerPhase()
    {
        PhaseState.Avancer(this);
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

    internal void TerminerPartie()
    {
        Phase = Phase.Showdown;

        // Gagnant par abandon
        if (Joueurs.Count(j => !j.EstCouche) == 1)
        {
            Gagnant = Joueurs.First(j => !j.EstCouche);
            Gagnant.Jetons += Pot;
        }
        else
        {
            // Gagnant par la meilleure main
            Gagnant = DeterminerGagnantParMain();
            Gagnant.Jetons += Pot;
        }
    }

    private Joueur DeterminerGagnantParMain()
    {
        return Joueurs
            .Where(j => j.DerniereAction != TypeActionJeu.SeCoucher)
            .Select(j => new
            {
                Joueur = j,
                Score = EvaluateurScore.EvaluerScore(j.Main, CartesCommunes)
            })
            .OrderByDescending(js => js.Score.Rang)
            .ThenByDescending(js => js.Score.Valeur)
            .ThenByDescending(js =>
                js.Joueur.Main.AsEnumerable().Union(CartesCommunes.AsEnumerable())
                .Select(c => c.Rang)
                .OrderByDescending(r => (int)r)
                .Take(5)
                .Sum(s => (int)s)) // Tie-breaker par la somme des rangs des 5 meilleures cartes
            .Select(js => js.Joueur)
            .First();
    }
}
