namespace casino.core.Jeux.Poker.Cartes;

public class JeuDeCartes : IDeck
{
    private readonly List<Carte> _cartes;
    private readonly Random _random = new();

    public JeuDeCartes()
    {
        _cartes = new List<Carte>();
        Melanger();
    }

    /// <summary>
    /// Mélange le paquet à 52 cartes
    /// </summary>
    public void Melanger()
    {
        _cartes.Clear();

        foreach (Couleur couleur in Enum.GetValues(typeof(Couleur)))
        {
            foreach (RangCarte rang in Enum.GetValues(typeof(RangCarte)))
            {
                _cartes.Add(new Carte(rang, couleur));
            }
        }
    }

    /// <summary>
    /// Tire une carte du paquet
    /// </summary>
    public Carte TirerCarte()
    {
        if (_cartes.Count == 0)
            throw new InvalidOperationException("Le paquet est vide.");

        int index = _random.Next(_cartes.Count);
        Carte carte = _cartes[index];
        _cartes.RemoveAt(index);

        return carte;
    }
}
