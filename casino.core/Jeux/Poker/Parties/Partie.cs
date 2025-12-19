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

    public void TerminerPartie()
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
            Gagnant = EvaluateurGagnant.DeterminerGagnantParMain(Joueurs, CartesCommunes);
            Gagnant.Jetons += Pot;
        }
    }
}
