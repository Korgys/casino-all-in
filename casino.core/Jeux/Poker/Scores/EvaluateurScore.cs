using casino.core.Jeux.Poker.Cartes;

namespace casino.core.Jeux.Poker.Scores;

public static class EvaluateurScore
{
    public static Score EvaluerScore(CartesMain main, CartesCommunes cartesCommunes)
    {
        var cartes = main.AsEnumerable().Union(cartesCommunes.AsEnumerable()).ToList();

        // Ordre poker standard (du plus fort au plus faible)
        if (ComporteQuinteFlushRoyale(cartes)) // QuinteFlushRoyale
            // Compare uniquement sur le rang (royale = max).
            return new Score(RangMain.QuinteFlushRoyale, RangCarte.As, kickers: Array.Empty<RangCarte>());
        else if (ValeurQuinteFlush(cartes) is RangCarte vQuinteFlush) // QuinteFlush
            // Départage les quintes flush par la carte haute.
            return new Score(RangMain.QuinteFlush, vQuinteFlush, kickers: Array.Empty<RangCarte>());
        else if (ValeurCarre(cartes) is RangCarte vCarre) // Carre
            return new Score(RangMain.Carre, vCarre, DeterminerMeilleurKickersHorsGroupe(cartes, rangsExclus: new[] { vCarre }, combien: 1));
        else if (ValeurFull(cartes) is RangCarte vTripsFull) // Full
            return new Score(RangMain.Full, vTripsFull, new[] { ValeurPairePourFull(cartes, vTripsFull) ?? (RangCarte)0 });
        else if (ValeurCouleur(cartes) is RangCarte _) // Couleur
        {
            // Utilise la valeur comme 1er kicker
            var top5 = ValeursCouleurTop5(cartes);
            return new Score(RangMain.Couleur, top5[0], top5.Skip(1));
        }
        else if (ValeurSuite(cartes) is RangCarte vSuite) // Suite
            return new Score(RangMain.Suite, vSuite, kickers: Array.Empty<RangCarte>());
        else if (ValeurBrelan(cartes) is RangCarte vBrelan) // Brelan
            return new Score(RangMain.Brelan, vBrelan, DeterminerMeilleurKickersHorsGroupe(cartes, rangsExclus: new[] { vBrelan }, 2));
        else if (ValeurDoublePaire(cartes) is RangCarte vPaireHaute) // Double Paire
        {
            var paireBasse = ValeurSecondePaire(cartes, vPaireHaute) ?? (RangCarte)0;
            var kicker = DeterminerMeilleurKickersHorsGroupe(cartes, rangsExclus: new[] { vPaireHaute, paireBasse }, combien: 1);
            return new Score(RangMain.DoublePaire, vPaireHaute, new[] { paireBasse }.Concat(kicker));
        }
        else if (ValeurPaire(cartes) is RangCarte vPaire) // Paire
        {
            var kickers = DeterminerMeilleurKickersHorsGroupe(cartes, rangsExclus: new[] { vPaire }, combien: 3);
            return new Score(RangMain.Paire, vPaire, kickers);
        }
        else // Carte haute
        {
            var top5 = cartes.Select(c => c.Rang).OrderByDescending(r => (int)r).Distinct().Take(5).ToList();
            return new Score(RangMain.CarteHaute, top5[0], top5.Skip(1));
        }
    }

    private static IEnumerable<RangCarte> DeterminerMeilleurKickersHorsGroupe(IEnumerable<Carte> cartes, IEnumerable<RangCarte> rangsExclus, int combien)
    {
        var exclus = rangsExclus.ToHashSet();
        // Prends les meilleures cartes hors rangs exclus.
        return cartes
            .Select(c => c.Rang)
            .Where(r => !exclus.Contains(r))
            .OrderByDescending(r => (int)r)
            .Distinct()
            .Take(combien)
            .ToList();
    }

    private static RangCarte? ValeurSecondePaire(IEnumerable<Carte> cartes, RangCarte paireHaute)
    {
        return cartes
            .GroupBy(c => c.Rang)
            .Where(g => g.Count() >= 2 && g.Key != paireHaute)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<RangCarte?>()
            .FirstOrDefault();
    }

    private static RangCarte? ValeurPairePourFull(IEnumerable<Carte> cartes, RangCarte trips)
    {
        // Cherche la meilleure paire hors trips (ou 2e brelan).
        var groupes = cartes.GroupBy(c => c.Rang).ToList();

        var paires = groupes
            .Where(g => g.Key != trips && g.Count() >= 2)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .ToList();

        if (paires.Count > 0)
            return paires[0];

        var autresBrelans = groupes
            .Where(g => g.Key != trips && g.Count() >= 3)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .ToList();

        return autresBrelans.Count > 0 ? autresBrelans[0] : null;
    }

    private static List<RangCarte> ValeursCouleurTop5(IEnumerable<Carte> cartes)
    {
        // Prendre la meilleure couleur et ses 5 meilleures cartes.
        List<RangCarte>? best = null;

        foreach (var grp in cartes.GroupBy(c => c.Couleur))
        {
            var top5 = grp.Select(c => c.Rang)
                          .OrderByDescending(r => (int)r)
                          .Distinct()
                          .Take(5)
                          .ToList();

            if (top5.Count < 5) continue;

            if (best is null || ComparerLexico(top5, best) > 0)
                best = top5;
        }

        return best ?? new List<RangCarte> { (RangCarte)0 };
    }

    private static int ComparerLexico(IReadOnlyList<RangCarte> a, IReadOnlyList<RangCarte> b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        int min = Math.Min(a.Count, b.Count);
        for (int i = 0; i < min; i++)
        {
            int cmp = a[i].CompareTo(b[i]);
            if (cmp != 0) return cmp;
        }

        return a.Count.CompareTo(b.Count);
    }

    private static bool ComporteQuinteFlushRoyale(IEnumerable<Carte> cartes)
    {
        // Une quinte flush dont la carte haute est As (donc 10-J-Q-K-A)
        // -> suffit de détecter une quinte flush avec high = As et présence du 10.
        foreach (var grp in cartes.GroupBy(c => c.Couleur))
        {
            if (grp.Count() < 5) continue;

            var high = ValeurSuite(grp.Select(c => c.Rang));
            if (high == RangCarte.As && grp.Any(c => c.Rang == RangCarte.Dix))
                return true;
        }

        return false;
    }

    private static RangCarte? ValeurQuinteFlush(IEnumerable<Carte> cartes)
    {
        RangCarte? best = null;

        foreach (var grp in cartes.GroupBy(c => c.Couleur))
        {
            if (grp.Count() < 5) continue;

            var high = ValeurSuite(grp.Select(c => c.Rang));
            if (high.HasValue && (!best.HasValue || high.Value > best.Value))
                best = high;
        }

        return best;
    }

    private static RangCarte? ValeurCarre(IEnumerable<Carte> cartes)
    {
        return cartes
            .GroupBy(c => c.Rang)
            .Where(g => g.Count() == 4)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<RangCarte?>()
            .FirstOrDefault();
    }

    private static RangCarte? ValeurFull(IEnumerable<Carte> cartes)
    {
        var groupes = cartes.GroupBy(c => c.Rang).ToList();

        var brelans = groupes
            .Where(g => g.Count() >= 3)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .ToList();

        if (brelans.Count == 0)
            return null;

        // On choisit le meilleur brelan comme "trips"
        var trips = brelans[0];

        // Pour la paire : soit un vrai groupe de 2+, soit un autre brelan (qui compte comme paire)
        var paires = groupes
            .Where(g => g.Key != trips && g.Count() >= 2)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .ToList();

        // Cas spécial : deux brelans -> le second brelan sert de paire
        if (paires.Count == 0 && brelans.Count >= 2)
            paires.Add(brelans[1]);

        return paires.Count > 0 ? trips : (RangCarte?)null;
    }

    private static RangCarte? ValeurCouleur(IEnumerable<Carte> cartes)
    {
        RangCarte? best = null;

        foreach (var grp in cartes.GroupBy(c => c.Couleur))
        {
            if (grp.Count() < 5) continue;

            var high = grp.Max(c => c.Rang);
            if (!best.HasValue || high > best.Value)
                best = high;
        }

        return best;
    }

    private static RangCarte? ValeurSuite(IEnumerable<Carte> cartes)
    {
        return ValeurSuite(cartes.Select(c => c.Rang));
    }

    private static RangCarte? ValeurBrelan(IEnumerable<Carte> cartes)
    {
        return cartes
            .GroupBy(c => c.Rang)
            .Where(g => g.Count() >= 3)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<RangCarte?>()
            .FirstOrDefault();
    }

    private static RangCarte? ValeurDoublePaire(IEnumerable<Carte> cartes)
    {
        var paires = cartes
            .GroupBy(c => c.Rang)
            .Where(g => g.Count() >= 2)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .ToList();

        if (paires.Count >= 2)
            return paires[0]; // paire la plus haute

        return null;
    }

    private static RangCarte? ValeurPaire(IEnumerable<Carte> cartes)
    {
        return cartes
            .GroupBy(c => c.Rang)
            .Where(g => g.Count() >= 2)
            .Select(g => g.Key)
            .OrderByDescending(r => r)
            .Cast<RangCarte?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Retourne la carte haute d'une suite (ex: 9 pour 5-6-7-8-9).
    /// Gère A-2-3-4-5 => high = 5.
    /// Retourne null si pas de suite.
    /// </summary>
    private static RangCarte? ValeurSuite(IEnumerable<RangCarte> rangCartes)
    {
        var values = rangCartes
            .Select(r => (int)r)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        if (values.Count < 5)
            return null;

        // As bas : si on a un As (14), on ajoute 1 pour représenter A comme 1
        if (values.Contains(14))
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
            else if (values[i] != values[i - 1])
            {
                consecutive = 1;
            }
        }

        if (bestHigh == -1)
            return null;

        // bestHigh vaut 5 pour wheel, ou 14 pour broadway, etc.
        return (RangCarte)bestHigh;
    }
}
