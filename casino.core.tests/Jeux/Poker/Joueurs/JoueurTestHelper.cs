using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;

namespace casino.core.tests.Jeux.Poker.Joueurs;

internal static class JoueurTestHelper
{
    internal static Partie CreerPartieAvecJoueur(Joueur joueur, CartesMain? main = null, CartesCommunes? communes = null, int miseDeDepart = 10)
    {
        var deck = new FakeDeck(Enumerable.Repeat(new Carte(RangCarte.Deux, Couleur.Coeur), 10));
        var partie = new Partie(new List<Joueur> { joueur }, deck)
        {
            MiseDeDepart = miseDeDepart
        };

        if (main != null)
        {
            joueur.Main = main;
        }

        if (communes != null)
        {
            partie.CartesCommunes = communes;
        }

        return partie;
    }

    internal static void DefinirMiseActuelle(Partie partie, int valeur)
    {
        var propriete = typeof(Partie).GetProperty(nameof(Partie.MiseActuelle), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        propriete?.SetValue(partie, valeur);
    }

    internal static CartesCommunes CreerCartesCommunes(params Carte[] cartes)
    {
        var communes = new CartesCommunes();
        if (cartes.Length > 0) communes.Flop1 = cartes[0];
        if (cartes.Length > 1) communes.Flop2 = cartes[1];
        if (cartes.Length > 2) communes.Flop3 = cartes[2];
        if (cartes.Length > 3) communes.Turn = cartes[3];
        if (cartes.Length > 4) communes.River = cartes[4];
        return communes;
    }

    private class FakeDeck : IDeck
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
            // Pas de mélange nécessaire pour les tests
        }
    }
}
