using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;

namespace casino.core.tests.Games.Poker.Players;

internal static class PlayerTestHelper
{
    internal static Partie CreerPartieAvecPlayer(Player Player, HandCards? main = null, TableCards? communes = null, int startingBet = 10)
    {
        var deck = new FakeDeck(Enumerable.Repeat(new Card(RangCarte.Deux, Couleur.Coeur), 10));
        var partie = new Partie(new List<Player> { Player }, deck)
        {
            StartingBet = startingBet
        };

        if (main != null)
        {
            Player.Hand = main;
        }

        if (communes != null)
        {
            partie.CommunityCards = communes;
        }

        return partie;
    }

    internal static void DefinirMiseActuelle(Partie partie, int valeur)
    {
        var propriete = typeof(Partie).GetProperty(nameof(Partie.CurrentBet), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        propriete?.SetValue(partie, valeur);
    }

    internal static TableCards CreerCartesCommunes(params Card[] cartes)
    {
        var communes = new TableCards();
        if (cartes.Length > 0) communes.Flop1 = cartes[0];
        if (cartes.Length > 1) communes.Flop2 = cartes[1];
        if (cartes.Length > 2) communes.Flop3 = cartes[2];
        if (cartes.Length > 3) communes.Turn = cartes[3];
        if (cartes.Length > 4) communes.River = cartes[4];
        return communes;
    }

    private class FakeDeck : IDeck
    {
        private readonly Queue<Card> _cartes;

        public FakeDeck(IEnumerable<Card> cartes)
        {
            _cartes = new Queue<Card>(cartes);
        }

        public Card TirerCarte()
        {
            return _cartes.Count > 0 ? _cartes.Dequeue() : new Card(RangCarte.Deux, Couleur.Carreau);
        }

        public void Melanger()
        {
            // Pas de mélange nécessaire pour les tests
        }
    }
}
