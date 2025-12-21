using casino.core.Common.Utils;
using System;

namespace casino.core.Jeux.Poker.Cartes;

public class JeuDeCartes : IDeck
{
    private readonly List<Carte> _cartes = new();
    private readonly IRandom _random;

    public int CartesRestantes => _cartes.Count;

    public JeuDeCartes(IRandom? random = null)
    {
        _random = random ?? new CasinoRandom();
        Melanger();
    }

    /// <summary>
    /// Reconstruire un paquet standard (52 cartes) puis mélanger réellement.
    /// </summary>
    public void Melanger()
    {
        // Reconstruire le paquet.
        _cartes.Clear();
        _cartes.AddRange(CreerPaquetStandard());

        // Mélanger le paquet (Fisher–Yates).
        for (int i = _cartes.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (_cartes[i], _cartes[j]) = (_cartes[j], _cartes[i]);
        }
    }

    /// <summary>
    /// Tire une carte du dessus (index 0) pour simplifier et rendre testable.
    /// </summary>
    public Carte TirerCarte()
    {
        if (_cartes.Count == 0)
            throw new InvalidOperationException("Le paquet est vide.");

        // Retirer la première carte : déterministe après mélange.
        var carte = _cartes[0];
        _cartes.RemoveAt(0);
        return carte;
    }

    /// <summary>
    /// Crée un jeu de cartes avec 52 cartes (4 couleurs, 13 rangs).
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<Carte> CreerPaquetStandard()
    {
        foreach (Couleur couleur in Enum.GetValues<Couleur>())
        {
            foreach (RangCarte rang in Enum.GetValues<RangCarte>())
            {
                yield return new Carte(rang, couleur);
            }
        }
    }
}
