using casino.core.Common.Utils;
using casino.core.Jeux.Poker.Cartes;

namespace casino.core.Jeux.Poker.Scores;

public static class EvaluateurProbabilite
{
    private const int NombreCartesCommunesTotal = 5;

    /// <summary>
    /// Estime la probabilité (0..100) que le joueur gagne la manche contre un nombre d'adversaires.
    /// Les égalités comptent comme une part de victoire proportionnelle (ex: split à 3 joueurs => 33.33%).
    /// 
    /// - Si un IRandom est fourni, l'exécution est séquentielle (utile pour les tests).
    /// - Sinon, au-delà d'un certain nombre de simulations, l'exécution est parallélisée.
    /// </summary>
    public static double EstimerProbabiliteDeGagner(
        CartesMain mainJoueur,
        CartesCommunes cartesCommunes,
        int nombreAdversaires,
        int simulations = 2000,
        IRandom? random = null)
    {
        ArgumentNullException.ThrowIfNull(mainJoueur);
        ArgumentNullException.ThrowIfNull(cartesCommunes);
        ArgumentOutOfRangeException.ThrowIfLessThan(nombreAdversaires, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(simulations, 1);

        if (nombreAdversaires == 0)
            return 100d;

        // --- Cartes connues (main + board partiel) ---
        var cartesConnues = new List<Carte>(7)
        {
            mainJoueur.Premiere,
            mainJoueur.Seconde
        };

        if (cartesCommunes.Flop1 is not null) cartesConnues.Add(cartesCommunes.Flop1);
        if (cartesCommunes.Flop2 is not null) cartesConnues.Add(cartesCommunes.Flop2);
        if (cartesCommunes.Flop3 is not null) cartesConnues.Add(cartesCommunes.Flop3);
        if (cartesCommunes.Turn is not null) cartesConnues.Add(cartesCommunes.Turn);
        if (cartesCommunes.River is not null) cartesConnues.Add(cartesCommunes.River);

        ValiderCartesConnues(cartesConnues);

        int cartesCommunesConnues = 0;
        if (cartesCommunes.Flop1 is not null) cartesCommunesConnues++;
        if (cartesCommunes.Flop2 is not null) cartesCommunesConnues++;
        if (cartesCommunes.Flop3 is not null) cartesCommunesConnues++;
        if (cartesCommunes.Turn is not null) cartesCommunesConnues++;
        if (cartesCommunes.River is not null) cartesCommunesConnues++;

        if (cartesCommunesConnues > NombreCartesCommunesTotal)
            throw new ArgumentException("Il ne peut pas y avoir plus de 5 cartes communes.", nameof(cartesCommunes));

        int cartesCommunesManquantes = NombreCartesCommunesTotal - cartesCommunesConnues;
        int cartesNecessaires = cartesCommunesManquantes + nombreAdversaires * 2;

        var paquetRestant = ConstruirePaquetRestant(cartesConnues);
        if (cartesNecessaires > paquetRestant.Count)
            throw new ArgumentException("Pas assez de cartes restantes pour simuler la partie.");

        int totalRestantes = paquetRestant.Count;

        // Si un RNG est fourni, on reste séquentiel pour garder la reproductibilité des tests.
        if (random is not null || simulations < 1000)
        {
            var randomizer = random ?? new CasinoRandom();
            return EstimerSequential(
                mainJoueur,
                cartesCommunes,
                nombreAdversaires,
                simulations,
                paquetRestant,
                totalRestantes,
                cartesNecessaires,
                randomizer);
        }

        // Sinon : parallélisation
        return EstimerParallel(
            mainJoueur,
            cartesCommunes,
            nombreAdversaires,
            simulations,
            paquetRestant,
            totalRestantes,
            cartesNecessaires);
    }

    // =========================
    //       SÉQUENTIEL
    // =========================

    private static double EstimerSequential(
        CartesMain mainJoueur,
        CartesCommunes cartesCommunes,
        int nombreAdversaires,
        int simulations,
        List<Carte> paquetRestant,
        int totalRestantes,
        int cartesNecessaires,
        IRandom randomizer)
    {
        var indices = CreerIndices(totalRestantes);
        double gains = 0d;

        for (int sim = 0; sim < simulations; sim++)
        {
            gains += SimulerUneManche(
                mainJoueur,
                cartesCommunes,
                nombreAdversaires,
                paquetRestant,
                indices,
                cartesNecessaires,
                totalRestantes,
                randomizer);
        }

        return gains / simulations * 100d;
    }

    // =========================
    //       PARALLÈLE
    // =========================

    private static double EstimerParallel(
        CartesMain mainJoueur,
        CartesCommunes cartesCommunes,
        int nombreAdversaires,
        int simulations,
        List<Carte> paquetRestant,
        int totalRestantes,
        int cartesNecessaires)
    {
        // Nombre de threads logiques à utiliser
        int maxWorkers = Environment.ProcessorCount;
        int workers = Math.Min(maxWorkers, simulations);

        double totalGains = 0d;
        object sync = new();

        System.Threading.Tasks.Parallel.For(
            fromInclusive: 0,
            toExclusive: workers,
            workerId =>
            {
                // Répartition des simulations par worker
                int simsForWorker = simulations / workers;
                if (workerId < simulations % workers)
                    simsForWorker++;

                if (simsForWorker <= 0)
                    return;

                // RNG indépendant par worker
                var localRandom = new CasinoRandom();
                var localIndices = CreerIndices(totalRestantes);
                double localGains = 0d;

                for (int i = 0; i < simsForWorker; i++)
                {
                    localGains += SimulerUneManche(
                        mainJoueur,
                        cartesCommunes,
                        nombreAdversaires,
                        paquetRestant,
                        localIndices,
                        cartesNecessaires,
                        totalRestantes,
                        localRandom);
                }

                lock (sync)
                {
                    totalGains += localGains;
                }
            });

        return totalGains / simulations * 100d;
    }

    // =========================
    //       COEUR SIMULATION
    // =========================

    private static double SimulerUneManche(
        CartesMain mainJoueur,
        CartesCommunes cartesCommunes,
        int nombreAdversaires,
        List<Carte> paquetRestant,
        int[] indices,
        int cartesNecessaires,
        int totalRestantes,
        IRandom randomizer)
    {
        // Mélange partiel des indices pour obtenir les cartes nécessaires
        MelangerIndicesPartiel(indices, cartesNecessaires, totalRestantes, randomizer);

        int indexPioche = 0;

        // Distribution des mains adverses
        var mainsAdversaires = new CartesMain[nombreAdversaires];
        for (int i = 0; i < nombreAdversaires; i++)
        {
            var c1 = paquetRestant[indices[indexPioche++]];
            var c2 = paquetRestant[indices[indexPioche++]];
            mainsAdversaires[i] = new CartesMain(c1, c2);
        }

        // Construction du board complet
        var board = ConstruireCartesCommunesDepuisIndices(
            cartesCommunes,
            paquetRestant,
            indices,
            ref indexPioche);

        // Évaluation des scores
        var scoreJoueur = EvaluateurScore.EvaluerScore(mainJoueur, board);

        var meilleurScore = scoreJoueur;
        int gagnants = 1; // le joueur au départ

        for (int i = 0; i < mainsAdversaires.Length; i++)
        {
            var scoreAdv = EvaluateurScore.EvaluerScore(mainsAdversaires[i], board);
            int cmp = scoreAdv.CompareTo(meilleurScore);

            if (cmp > 0)
            {
                meilleurScore = scoreAdv;
                gagnants = 1;
            }
            else if (cmp == 0)
            {
                gagnants++;
            }
        }

        // Si le joueur fait partie des meilleurs, il récupère 1/gagnants
        if (scoreJoueur.CompareTo(meilleurScore) == 0)
            return 1d / gagnants;

        return 0d;
    }

    // =========================
    //       UTILITAIRES
    // =========================

    private static void ValiderCartesConnues(IEnumerable<Carte> cartesConnues)
    {
        var vues = new HashSet<Carte>();

        foreach (var carte in cartesConnues)
        {
            if (!vues.Add(carte))
                throw new ArgumentException("Les cartes fournies contiennent des doublons.");
        }
    }

    private static List<Carte> ConstruirePaquetRestant(IEnumerable<Carte> cartesConnues)
    {
        var dejaConnues = new HashSet<Carte>(cartesConnues);
        var paquet = new List<Carte>(capacity: 52);

        foreach (Couleur couleur in Enum.GetValues<Couleur>())
        {
            foreach (RangCarte rang in Enum.GetValues<RangCarte>())
            {
                var carte = new Carte(rang, couleur);
                if (!dejaConnues.Contains(carte))
                    paquet.Add(carte);
            }
        }

        return paquet;
    }

    private static CartesCommunes ConstruireCartesCommunesDepuisIndices(
        CartesCommunes existantes,
        List<Carte> paquetRestant,
        int[] indices,
        ref int indexPioche)
    {
        var communes = new CartesCommunes
        {
            Flop1 = existantes.Flop1,
            Flop2 = existantes.Flop2,
            Flop3 = existantes.Flop3,
            Turn = existantes.Turn,
            River = existantes.River
        };

        communes.Flop1 ??= PiocherDepuisIndices(paquetRestant, indices, ref indexPioche);
        communes.Flop2 ??= PiocherDepuisIndices(paquetRestant, indices, ref indexPioche);
        communes.Flop3 ??= PiocherDepuisIndices(paquetRestant, indices, ref indexPioche);
        communes.Turn ??= PiocherDepuisIndices(paquetRestant, indices, ref indexPioche);
        communes.River ??= PiocherDepuisIndices(paquetRestant, indices, ref indexPioche);

        return communes;
    }

    private static Carte PiocherDepuisIndices(
        List<Carte> paquetRestant,
        int[] indices,
        ref int indexPioche)
    {
        if (indexPioche >= indices.Length)
            throw new InvalidOperationException("Plus de cartes dans la pioche.");

        return paquetRestant[indices[indexPioche++]];
    }

    private static int[] CreerIndices(int n)
    {
        var indices = new int[n];
        for (int i = 0; i < n; i++)
        {
            indices[i] = i;
        }
        return indices;
    }

    /// <summary>
    /// Fisher-Yates partiel : on ne randomise que les cartesNecessaires premières positions.
    /// </summary>
    private static void MelangerIndicesPartiel(
        int[] indices,
        int cartesNecessaires,
        int tailleTotale,
        IRandom random)
    {
        for (int i = 0; i < cartesNecessaires; i++)
        {
            int j = random.Next(i, tailleTotale);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
    }
}
