using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Parties;

namespace casino.core.Jeux.Poker.Scores;

public class EvaluateurScore
{
    public static Score EvaluerScore(CartesMain main, CartesCommunes cartesCommunes)
    {
        var cartes = main.AsEnumerable().Union(cartesCommunes.AsEnumerable()).ToList();
        var score = new Score();

        // Ordre poker standard (du plus fort au plus faible)
        if (ComporteQuinteFlushRoyale(cartes))
        {
            score.Rang = RangMain.QuinteFlushRoyale;
            score.Valeur = RangCarte.As;
        }
        else if (ValeurQuinteFlush(cartes) is RangCarte vQuinteFlush)
        {
            score.Rang = RangMain.QuinteFlush;
            score.Valeur = vQuinteFlush; // carte haute de la quinte flush (5 pour A-2-3-4-5)
        }
        else if (ValeurCarre(cartes) is RangCarte vCarre)
        {
            score.Rang = RangMain.Carre;
            score.Valeur = vCarre;
        }
        else if (ValeurFull(cartes) is RangCarte vFull)
        {
            score.Rang = RangMain.Full;
            score.Valeur = vFull; // valeur du brelan du full
        }
        else if (ValeurCouleur(cartes) is RangCarte vCouleur)
        {
            score.Rang = RangMain.Couleur;
            score.Valeur = vCouleur; // plus haute carte de la couleur
        }
        else if (ValeurSuite(cartes) is RangCarte vSuite)
        {
            score.Rang = RangMain.Suite;
            score.Valeur = vSuite; // carte haute de la suite (5 pour A-2-3-4-5)
        }
        else if (ValeurBrelan(cartes) is RangCarte vBrelan)
        {
            score.Rang = RangMain.Brelan;
            score.Valeur = vBrelan;
        }
        else if (ValeurDoublePaire(cartes) is RangCarte vDoublePaire)
        {
            score.Rang = RangMain.DoublePaire;
            score.Valeur = vDoublePaire; // valeur de la paire la plus haute
        }
        else if (ValeurPaire(cartes) is RangCarte vPaire)
        {
            score.Rang = RangMain.Paire;
            score.Valeur = vPaire;
        }
        else
        {
            score.Rang = RangMain.CarteHaute;
            score.Valeur = cartes.Max(c => c.Rang);
        }

        return score;
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

    private static bool ComporteQuinteFlush(IEnumerable<Carte> cartes)
        => ValeurQuinteFlush(cartes).HasValue;

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
