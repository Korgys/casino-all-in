using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Jeux.Poker.Scores;

public static class EvaluateurGagnant
{
    /// <summary>
    /// Détermine le(s) gagnant(s) parmi les joueurs encore en jeu, basé sur la meilleure main.
    /// </summary>
    /// <param name="joueurs"></param>
    /// <param name="cartesCommunes"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IReadOnlyList<Joueur> DeterminerGagnantsParMain(IEnumerable<Joueur> joueurs, CartesCommunes cartesCommunes)
    {
        // Calcule le score des joueurs en jeu
        var joueursEnJeu = joueurs
            .Where(j => j.DerniereAction != TypeActionJeu.SeCoucher)
            .Select(j => new
            {
                Joueur = j,
                Score = EvaluateurScore.EvaluerScore(j.Main, cartesCommunes)
            });

        if (!joueursEnJeu.Any())
        {
            throw new ArgumentException("Au moins un joueur doit être encore en jeu pour déterminer le gagnant par la main.");
        }

        // Récupère le meilleur score
        var meilleurScore = joueursEnJeu.Max(x => x.Score);

        // Retourne tous ceux à égalité parfaite
        return joueursEnJeu
            .Where(j => j.Score.CompareTo(meilleurScore) == 0)
            .Select(x => x.Joueur)
            .ToList();
    }
}
