using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Jeux.Poker.Scores;

public static class EvaluateurGagnant
{
    public static Joueur DeterminerGagnantParMain(IEnumerable<Joueur> joueurs, CartesCommunes cartesCommunes)
    {
        if (!joueurs.Any(j => j.DerniereAction != TypeActionJeu.SeCoucher))
        {
            throw new ArgumentException("Au moins un joueur doit être encore en jeu pour déterminer le gagnant par la main.");
        }

        return joueurs
            .Where(j => j.DerniereAction != TypeActionJeu.SeCoucher)
            .Select(j => new
            {
                Joueur = j,
                Score = EvaluateurScore.EvaluerScore(j.Main, cartesCommunes)
            })
            .OrderByDescending(js => js.Score.Rang)
            .ThenByDescending(js => js.Score.Valeur)
            .ThenByDescending(js =>
                js.Joueur.Main.AsEnumerable().Union(cartesCommunes.AsEnumerable())
                .Select(c => c.Rang)
                .OrderByDescending(r => (int)r)
                .Take(5)
                .Sum(s => (int)s)) // Tie-breaker par la somme des rangs des 5 meilleures cartes
            .Select(js => js.Joueur)
            .First();
    }
}
