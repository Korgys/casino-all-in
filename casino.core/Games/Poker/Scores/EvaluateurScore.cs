using System;
using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker.Cartes;

namespace casino.core.Games.Poker.Scores;

public static class EvaluateurScore
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
        var groupesParCouleur = cartes
            .GroupBy(c => c.Couleur)
            .ToArray();

        // Ordre poker standard (du plus fort au plus faible)

        // Quinte flush royale
        if (ComporteQuinteFlushRoyale(groupesParCouleur))
            return new Score(RangMain.QuinteFlushRoyale, RangCarte.As, kickers: Array.Empty<RangCarte>());

        // Quinte flush
        if (ValeurQuinteFlush(groupesParCouleur) is RangCarte vQuinteFlush)
            return new Score(RangMain.QuinteFlush, vQuinteFlush, kickers: Array.Empty<RangCarte>());

        // Carré
        if (ValeurCarre(groupesParRang) is RangCarte vCarre)
        {
            var kicker = DeterminerMeilleursKickers(rangsDistinctDesc, exclus: new[] { vCarre }, combien: 1);
            return new Score(RangMain.Carre, vCarre, kicker);
        }

        // Full
        if (ValeurFull(groupesParRang, out var tripsFull, out var paireFull))
            return new Score(RangMain.Full, tripsFull, new[] { paireFull });

        // Couleur
        if (TryGetFlushTop5(groupesParCouleur, out var flushTop5))
            return new Score(RangMain.Couleur, flushTop5[0], flushTop5.Skip(1).ToArray());

        // Suite
        if (ValeurSuite(rangsDistinctDesc) is RangCarte vSuite)
            return new Score(RangMain.Suite, vSuite, kickers: Array.Empty<RangCarte>());

        // Brelan
        if (ValeurBrelan(groupesParRang) is RangCarte vBrelan)
        {
            var kickers = DeterminerMeilleursKickers(rangsDistinctDesc, exclus: new[] { vBrelan }, combien: 2);
            return new Score(RangMain.Brelan, vBrelan, kickers);
        }

        // Double paire
        if (ValeurDoublePaire(groupesParRang, out var paireHaute, out var paireBasse))
        {
            var kicker = DeterminerMeilleursKickers(rangsDistinctDesc, exclus: new[] { paireHaute, paireBasse }, combien: 1);
            return new Score(RangMain.DoublePaire, paireHaute, new[] { paireBasse }.Concat(kicker).ToArray());
        }

        // Paire
        if (ValeurPaire(groupesParRang) is RangCarte vPaire)
        {
            var kickers = DeterminerMeilleursKickers(rangsDistinctDesc, exclus: new[] { vPaire }, combien: 3);
            return new Score(RangMain.Paire, vPaire, kickers);
        }

        // Carte haute
        if (rangsDistinctDesc.Length == 0)
            return new Score(RangMain.CarteHaute, RangCarte.Deux, Array.Empty<RangCarte>());

        var topCarteHaute = rangsDistinctDesc[0];
        var kickersCarteHaute = rangsDistinctDesc.Skip(1).Take(4).ToArray();
        return new Score(RangMain.CarteHaute, topCarteHaute, kickersCarteHaute);
    }

    /// <summary>
    /// Prend les meilleurs rangs hors exclus, déjà triés décroissant (rangsDistinctDesc).
    /// Retourne un tableau figé (pas de lazy enumerable).
    /// </summary>
    private static RangCarte[] DeterminerMeilleursKickers(
        RangCarte[] rangsDistinctDesc,
        RangCarte[] exclus,
        int combien)
    {
        if (combien <= 0) return Array.Empty<RangCarte>();
        if (exclus.Length == 0) return rangsDistinctDesc.Take(combien).ToArray();

        var res = new List<RangCarte>(capacity: combien);
        for (int i = 0; i < rangsDistinctDesc.Length && res.Count < combien; i++)
        {
            var r = rangsDistinctDesc[i];
            if (!exclus.Contains(r)) // Note : exclus est petit, donc Contains() est OK, mais on pourrait optimiser avec un HashSet si besoin
                res.Add(r);
        }

        return res.ToArray();
    }

    private static bool TryGetFlushTop5(IEnumerable<IGrouping<Couleur, Card>> groupesParCouleur, out RangCarte[] top5)
    {
        foreach (var grp in groupesParCouleur)
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

        top5 = Array.Empty<RangCarte>();
        return false;
    }

    private static bool ComporteQuinteFlushRoyale(IEnumerable<IGrouping<Couleur, Card>> groupesParCouleur)
    {
        // Une quinte flush dont la carte haute est As (10-J-Q-K-A)
        foreach (var grp in groupesParCouleur)
        {
            if (grp.Count() < 5) continue;

            var rangsDistinctDesc = grp.Select(c => c.Rang)
                                       .Distinct()
                                       .OrderByDescending(r => r)
                                       .ToArray();

            // Pour être "royale", il faut la suite avec high = As et un 10 présent.
            var high = ValeurSuite(rangsDistinctDesc);
            if (high == RangCarte.As && rangsDistinctDesc.Contains(RangCarte.Dix))
                return true;
        }

        return false;
    }

    private static RangCarte? ValeurQuinteFlush(IEnumerable<IGrouping<Couleur, Card>> groupesParCouleur)
    {
        RangCarte? best = null;

        foreach (var grp in groupesParCouleur)
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

    private static RangCarte? ValeurCarre(IEnumerable<IGrouping<RangCarte, Card>> groupesParRang)
    {
        return groupesParRang
            .Where(g => g.Count() == 4)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<RangCarte?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Full house :
    /// - trips = meilleur brelan
    /// - paire = meilleure paire hors trips, ou second brelan utilisé comme paire
    /// </summary>
    private static bool ValeurFull(
        IEnumerable<IGrouping<RangCarte, Card>> groupesParRang,
        out RangCarte trips,
        out RangCarte paire)
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

    private static RangCarte? ValeurBrelan(IEnumerable<IGrouping<RangCarte, Card>> groupesParRang)
    {
        return groupesParRang
            .Where(g => g.Count() >= 3)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<RangCarte?>()
            .FirstOrDefault();
    }

    private static bool ValeurDoublePaire(
        IEnumerable<IGrouping<RangCarte, Card>> groupesParRang,
        out RangCarte paireHaute,
        out RangCarte paireBasse)
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

    private static RangCarte? ValeurPaire(IEnumerable<IGrouping<RangCarte, Card>> groupesParRang)
    {
        return groupesParRang
            .Where(g => g.Count() >= 2)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<RangCarte?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Retourne la carte haute d'une suite à partir d'un tableau de rangs distincts triés décroissant.
    /// Gère A-2-3-4-5 => high = 5.
    /// Retourne null si pas de suite.
    /// </summary>
    private static RangCarte? ValeurSuite(IReadOnlyList<RangCarte> rangsDistinctDesc)
    {
        // On repasse en croissant d'int pour la détection de suite.
        var values = rangsDistinctDesc
            .Select(r => (int)r)
            .OrderBy(v => v)
            .ToList();

        if (values.Count < 5)
            return null;

        // As bas : si on a un As (14), on ajoute 1 pour représenter A comme 1
        if (values.Contains((int)RangCarte.As))
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

        return bestHigh == -1 ? null : (RangCarte)bestHigh;
    }
}
