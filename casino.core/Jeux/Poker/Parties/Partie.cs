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
    public List<Joueur> Joueurs { get; set; }
    public CartesCommunes CartesCommunes { get; set; } = new CartesCommunes();
    public Joueur Gagnant { get; set; }
    public Phase Phase { get; set; } = Phase.PreFlop;
    public IPhaseState PhaseState { get; internal set; } = new PreFlopState();
    public int Pot { get; set; } = 0;
    public int MiseDeDepart { get; set; } = 10;
    public int MiseActuelle { get; internal set; }

    public Partie(List<Joueur> joueurs)
    {
        Joueurs = joueurs;
        JeuDeCartes.Instance.Melanger();
        DistribuerCartes();
    }

    public void AvancerPhase()
    {
        PhaseState.Avancer(this);
    }

    public bool EnCours() => Phase != Phase.Showdown;

    public IEnumerable<JoueurActionType> ObtenirActionsPossibles(Joueur joueur)
    {
        return PhaseState.ObtenirActionsPossibles(joueur, this);
    }

    public void AppliquerAction(Joueur joueur, JoueurAction action)
    {
        PhaseState.AppliquerAction(joueur, action, this);
    }

    public void TraiterCoucher(Joueur joueur)
    {
        joueur.EstCouche = true;
        joueur.DerniereAction = JoueurActionType.SeCoucher;

        if (Joueurs.Count(j => !j.EstCouche) == 1)
        {
            TerminerPartie();
        }
    }

    public void TraiterMiser(Joueur joueur, int montant)
    {
        if (MiseActuelle > montant)
        {
            throw new InvalidOperationException("La mise ne peut pas être inférieur à la mise de départ/actuelle.");
        }

        joueur.DerniereAction = JoueurActionType.Miser;
        MiseActuelle = montant;
        joueur.Jetons -= MiseActuelle;
        Pot += MiseActuelle;
    }

    public void TraiterSuivre(Joueur joueur)
    {
        if (joueur.Jetons - MiseActuelle < 0)
        {
            throw new InvalidOperationException("Le joueur n'a pas assez de jetons pour suivre la mise actuelle.");
        }
        else if (joueur.Jetons - MiseActuelle == 0)
        {
            TraiterTapis(joueur);
        }
        else if (MiseActuelle == 0)
        {
            TraiterCheck(joueur);
        }
        else
        {
            joueur.DerniereAction = JoueurActionType.Suivre;
            joueur.Jetons -= MiseActuelle;
            Pot += MiseActuelle;
        }
    }

    public void TraiterRelancer(Joueur joueur, int montant)
    {
        if (montant < MiseActuelle)
        {
            throw new ArgumentException("La relance doit être supérieure ou égale à la mise actuelle.");
        }
        else if (montant > joueur.Jetons)
        {
            throw new ArgumentException("Le joueur n'a pas assez de jetons pour relancer autant.");
        }
        else if (montant == joueur.Jetons)
        {
            TraiterTapis(joueur);
        }
        else
        {
            joueur.DerniereAction = JoueurActionType.Relancer;
            joueur.Jetons -= montant;
            MiseActuelle = montant;
            Pot += montant;
        }
    }

    public void TraiterTapis(Joueur joueur)
    {
        joueur.DerniereAction = JoueurActionType.Tapis;
        Pot += joueur.Jetons;
        joueur.Jetons = 0;
        joueur.EstTapis = true;
    }

    public void TraiterCheck(Joueur joueur)
    {
        if (MiseActuelle != 0)
        {
            throw new InvalidOperationException("Le joueur ne peut pas checker car il y a une mise sur la table.");
        }

        // Le joueur peut checker
        joueur.DerniereAction = JoueurActionType.Check;
    }

    private void DistribuerCartes()
    {
        foreach (var joueur in Joueurs.Where(j => j.Jetons > 0 && j.DerniereAction != JoueurActionType.SeCoucher))
        {
            joueur.Main = new CartesMain(JeuDeCartes.Instance.TirerCarte(), JeuDeCartes.Instance.TirerCarte());
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
            .Where(j => !j.EstCouche)
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
