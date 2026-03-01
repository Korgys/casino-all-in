using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Scores;

public static class WinnerEvaluator
{
    /// <summary>
    /// Evaluate the winner of the table.
    /// </summary>
    /// <param name="players"></param>
    /// <param name="tableCards"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IReadOnlyList<Player> DetermineWinnersByHand(IEnumerable<Player> players, TableCards tableCards)
    {
        // Calcule le score des Players en jeu
        var inGamePlayers = players
            .Where(j => j.LastAction != PokerTypeAction.Fold)
            .Select(j => new
            {
                Player = j,
                Score = ScoreEvaluator.EvaluateScore(j.Hand, tableCards)
            });

        if (!inGamePlayers.Any())
        {
            throw new ArgumentException("Au moins un Player doit être encore en jeu pour déterminer le gagnant par la main.");
        }

        // Récupère le meilleur score
        var meilleurScore = inGamePlayers.Max(x => x.Score);

        // Retourne tous ceux à égalité parfaite
        return inGamePlayers
            .Where(j => j.Score.CompareTo(meilleurScore) == 0)
            .Select(x => x.Player)
            .ToList();
    }
}
