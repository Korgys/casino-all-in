using casino.core.Jeux.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Fakes;

public class FakeDeck : IDeck
{
    private readonly Queue<Carte> _cartes;

    public FakeDeck(IEnumerable<Carte> cartes)
    {
        _cartes = new Queue<Carte>(cartes);
    }

    public Carte TirerCarte()
    {
        return _cartes.Count > 0 ? _cartes.Dequeue() : new Carte(RangCarte.Deux, Couleur.Carreau);
    }

    public void Melanger()
    {
        // Aucun mélange nécessaire pour les tests.
    }
}