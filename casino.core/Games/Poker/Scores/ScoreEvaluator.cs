using System;
using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Poker.Scores;

public static class ScoreEvaluator
{
    public static Score EvaluerScore(HandCards main, TableCards communityCards)
    {
        ArgumentNullException.ThrowIfNull(main);
        ArgumentNullException.ThrowIfNull(communityCards);

        // Hold'em : 2 privatives + 0..5 communes => 2..7 cartes
        var cartes = main.AsEnumerable().Concat(communityCards.AsEnumerable()).ToArray();

        // Centralisation des rangs triés (utile pour carte haute / kickers)
        // GroupBy faits une seule fois
        var rangsDistinctDesc = cartes
            .Select(c => c.Rang)
            .Distinct()
            .OrderByDescending(r => r)
            .ToArray();
        var groupesParRang = cartes
            .GroupBy(c => c.Rang)
            .ToArray();
        var groupesParSuit = cartes
            .GroupBy(c => c.Suit)
            .ToArray();

        // Ordre poker standard (du plus fort au plus faible)

        // Quinte flush royale
        if (ComporteQuinteFlushRoyale(groupesParSuit))
            return new Score(HandRank.QuinteFlushRoyale, CardRank.As, kickers: Array.Empty<CardRank>());

        // Quinte flush
        if (ValeurQuinteFlush(groupesParSuit) is CardRank vQuinteFlush)
            return new Score(HandRank.QuinteFlush, vQuinteFlush, kickers: Array.Empty<CardRank>());

        // Carré
        if (ValeurCarre(groupesParRang) is CardRank vCarre)
        {
            var kicker = DeterminerMeilleursKickers(rangsDistinctDesc, exclus: new[] { vCarre }, combien: 1);
            return new Score(HandRank.Carre, vCarre, kicker);
        }

        // Full
        if (ValeurFull(groupesParRang, out var tripsFull, out var paireFull))
            return new Score(HandRank.Full, tripsFull, new[] { paireFull });

        // Suit
        if (TryGetFlushTop5(groupesParSuit, out var flushTop5))
            return new Score(HandRank.Suit, flushTop5[0], flushTop5.Skip(1).ToArray());

        // Suite
        if (ValeurSuite(rangsDistinctDesc) is CardRank vSuite)
            return new Score(HandRank.Suite, vSuite, kickers: Array.Empty<CardRank>());

        // Brelan
        if (ValeurBrelan(groupesParRang) is CardRank vBrelan)
        {
            var kickers = DeterminerMeilleursKickers(rangsDistinctDesc, exclus: new[] { vBrelan }, combien: 2);
            return new Score(HandRank.Brelan, vBrelan, kickers);
        }

        // Double paire
        if (ValeurDoublePaire(groupesParRang, out var paireHaute, out var paireBasse))
        {
            var kicker = DeterminerMeilleursKickers(rangsDistinctDesc, exclus: new[] { paireHaute, paireBasse }, combien: 1);
            return new Score(HandRank.DoublePaire, paireHaute, new[] { paireBasse }.Concat(kicker).ToArray());
        }

        // Paire
        if (ValeurPaire(groupesParRang) is CardRank vPaire)
        {
            var kickers = DeterminerMeilleursKickers(rangsDistinctDesc, exclus: new[] { vPaire }, combien: 3);
            return new Score(HandRank.Paire, vPaire, kickers);
        }

        // Carte haute
        if (rangsDistinctDesc.Length == 0)
            return new Score(HandRank.CarteHaute, CardRank.Deux, Array.Empty<CardRank>());

        var topCarteHaute = rangsDistinctDesc[0];
        var kickersCarteHaute = rangsDistinctDesc.Skip(1).Take(4).ToArray();
        return new Score(HandRank.CarteHaute, topCarteHaute, kickersCarteHaute);
    }

    /// <summary>
    /// Prend les meilleurs rangs hors exclus, déjà triés décroissant (rangsDistinctDesc).
    /// Retourne un tableau figé (pas de lazy enumerable).
    /// </summary>
    private static CardRank[] DeterminerMeilleursKickers(
        CardRank[] rangsDistinctDesc,
        CardRank[] exclus,
        int combien)
    {
        if (combien <= 0) return Array.Empty<CardRank>();
        if (exclus.Length == 0) return rangsDistinctDesc.Take(combien).ToArray();

        var res = new List<CardRank>(capacity: combien);
        for (int i = 0; i < rangsDistinctDesc.Length && res.Count < combien; i++)
        {
            var r = rangsDistinctDesc[i];
            if (!exclus.Contains(r)) // Note : exclus est petit, donc Contains() est OK, mais on pourrait optimiser avec un HashSet si besoin
                res.Add(r);
        }

        return res.ToArray();
    }

    private static bool TryGetFlushTop5(IEnumerable<IGrouping<Suit, Card>> groupesParSuit, out CardRank[] top5)
    {
        foreach (var grp in groupesParSuit)
        {
            if (grp.Count() < 5) continue;

            var ranks = grp.Select(c => c.Rang)
                           .Distinct()
                           .OrderByDescending(r => r)
                           .Take(5)
                           .ToArray();

            if (ranks.Length == 5)
            {
                top5 = ranks;
                return true;
            }
        }

        top5 = Array.Empty<CardRank>();
        return false;
    }

    private static bool ComporteQuinteFlushRoyale(IEnumerable<IGrouping<Suit, Card>> groupesParSuit)
    {
        // Une quinte flush dont la carte haute est As (10-J-Q-K-A)
        foreach (var grp in groupesParSuit)
        {
            if (grp.Count() < 5) continue;

            var rangsDistinctDesc = grp.Select(c => c.Rang)
                                       .Distinct()
                                       .OrderByDescending(r => r)
                                       .ToArray();

            // Pour être "royale", il faut la suite avec high = As et un 10 présent.
            var high = ValeurSuite(rangsDistinctDesc);
            if (high == CardRank.As && rangsDistinctDesc.Contains(CardRank.Dix))
                return true;
        }

        return false;
    }

    private static CardRank? ValeurQuinteFlush(IEnumerable<IGrouping<Suit, Card>> groupesParSuit)
    {
        CardRank? best = null;

        foreach (var grp in groupesParSuit)
        {
            if (grp.Count() < 5) continue;

            var rangsDistinctDesc = grp.Select(c => c.Rang)
                                       .Distinct()
                                       .OrderByDescending(r => r)
                                       .ToArray();

            var high = ValeurSuite(rangsDistinctDesc);
            if (high.HasValue && (!best.HasValue || high.Value > best.Value))
                best = high;
        }

        return best;
    }

    private static CardRank? ValeurCarre(IEnumerable<IGrouping<CardRank, Card>> groupesParRang)
    {
        return groupesParRang
            .Where(g => g.Count() == 4)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<CardRank?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Full house :
    /// - trips = meilleur brelan
    /// - paire = meilleure paire hors trips, ou second brelan utilisé comme paire
    /// </summary>
    private static bool ValeurFull(
        IEnumerable<IGrouping<CardRank, Card>> groupesParRang,
        out CardRank trips,
        out CardRank paire)
    {
        trips = default;
        paire = default;

        var brelans = groupesParRang
            .Where(g => g.Count() >= 3)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .ToArray();

        if (brelans.Length == 0)
            return false;

        trips = brelans[0];

        var paires = groupesParRang
            .Where(g => g.Key != brelans[0] && g.Count() >= 2)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .ToArray();

        if (paires.Length > 0)
        {
            paire = paires[0];
            return true;
        }

        // Cas spécial : 2 brelans => le second brelan sert de paire
        if (brelans.Length >= 2)
        {
            paire = brelans[1];
            return true;
        }

        return false;
    }

    private static CardRank? ValeurBrelan(IEnumerable<IGrouping<CardRank, Card>> groupesParRang)
    {
        return groupesParRang
            .Where(g => g.Count() >= 3)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<CardRank?>()
            .FirstOrDefault();
    }

    private static bool ValeurDoublePaire(
        IEnumerable<IGrouping<CardRank, Card>> groupesParRang,
        out CardRank paireHaute,
        out CardRank paireBasse)
    {
        paireHaute = default;
        paireBasse = default;

        var paires = groupesParRang
            .Where(g => g.Count() >= 2)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .ToArray();

        if (paires.Length < 2)
            return false;

        paireHaute = paires[0];
        paireBasse = paires[1];
        return true;
    }

    private static CardRank? ValeurPaire(IEnumerable<IGrouping<CardRank, Card>> groupesParRang)
    {
        return groupesParRang
            .Where(g => g.Count() >= 2)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<CardRank?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Retourne la carte haute d'une suite à partir d'un tableau de rangs distincts triés décroissant.
    /// Gère A-2-3-4-5 => high = 5.
    /// Retourne null si pas de suite.
    /// </summary>
    private static CardRank? ValeurSuite(IReadOnlyList<CardRank> rangsDistinctDesc)
    {
        // On repasse en croissant d'int pour la détection de suite.
        var values = rangsDistinctDesc
            .Select(r => (int)r)
            .OrderBy(v => v)
            .ToList();

        if (values.Count < 5)
            return null;

        // As bas : si on a un As (14), on ajoute 1 pour représenter A comme 1
        if (values.Contains((int)CardRank.As))
            values.Insert(0, 1);

        int consecutive = 1;
        int bestHigh = -1;

        for (int i = 1; i < values.Count; i++)
        {
            if (values[i] == values[i - 1] + 1)
            {
                consecutive++;
                if (consecutive >= 5)
                    bestHigh = values[i];
            }
            else
            {
                consecutive = 1;
            }
        }

        return bestHigh == -1 ? null : (CardRank)bestHigh;
    }
}
