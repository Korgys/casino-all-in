using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Scores;

public static class EvaluateurGagnant
{
    /// <summary>
    /// Détermine le(s) gagnant(s) parmi les Players encore en jeu, basé sur la meilleure main.
    /// </summary>
    /// <param name="Players"></param>
    /// <param name="communityCards"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IReadOnlyList<Player> DeterminerGagnantsParMain(IEnumerable<Player> Players, TableCards communityCards)
    {
        // Calcule le score des Players en jeu
        var PlayersEnJeu = Players
            .Where(j => j.LastAction != TypeActionJeu.SeCoucher)
            .Select(j => new
            {
                Player = j,
                Score = EvaluateurScore.EvaluerScore(j.Hand, communityCards)
            });

        if (!PlayersEnJeu.Any())
        {
            throw new ArgumentException("Au moins un Player doit être encore en jeu pour déterminer le gagnant par la main.");
        }

        // Récupère le meilleur score
        var meilleurScore = PlayersEnJeu.Max(x => x.Score);

        // Retourne tous ceux à égalité parfaite
        return PlayersEnJeu
            .Where(j => j.Score.CompareTo(meilleurScore) == 0)
            .Select(x => x.Player)
            .ToList();
    }
}
