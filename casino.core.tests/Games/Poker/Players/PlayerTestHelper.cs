using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;

namespace casino.core.tests.Games.Poker.Players;

internal static class PlayerTestHelper
{
    internal static Round CreerRoundAvecPlayer(Player Player, HandCards? main = null, TableCards? communes = null, int startingBet = 10)
    {
        var deck = new FakeDeck(Enumerable.Repeat(new Card(CardRank.Deux, Suit.Hearts), 10));
        var partie = new Round(new List<Player> { Player }, deck)
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

    internal static void DefinirMiseActuelle(Round partie, int valeur)
    {
        var propriete = typeof(Round).GetProperty(nameof(Round.CurrentBet), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        propriete?.SetValue(partie, valeur);
    }

    internal static TableCards CreateCommunityCards(params Card[] cartes)
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

        public Card DrawCard()
        {
            return _cartes.Count > 0 ? _cartes.Dequeue() : new Card(CardRank.Deux, Suit.Diamonds);
        }

        public void Shuffle()
        {
            // Pas de mélange nécessaire pour les tests
        }
    }
}
