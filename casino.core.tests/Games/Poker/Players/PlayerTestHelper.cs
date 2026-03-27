using System.Reflection;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;

namespace casino.core.tests.Games.Poker.Players;

internal static class PlayerTestHelper
{
    internal static Round CreateRoundWithPlayer(Player player, HandCards? hand = null, TableCards? communityCards = null, int startingBet = 10)
    {
        var deck = new FakeDeck(Enumerable.Repeat(new Card(CardRank.Deux, Suit.Hearts), 10));
        var round = new Round(new List<Player> { player }, deck);
        round.StartingBet = startingBet;

        if (hand != null)
        {
            player.Hand = hand;
        }

        if (communityCards != null)
        {
            round.SetCommunityCards(communityCards);
        }

        return round;
    }

    internal static void SetCurrentBet(Round round, int amount)
    {
        var property = typeof(Round).GetProperty(nameof(Round.CurrentBet), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(round, amount);
    }

    internal static TableCards CreateCommunityCards(params Card[] cards)
    {
        var communityCards = new TableCards();
        if (cards.Length > 0) communityCards.Flop1 = cards[0];
        if (cards.Length > 1) communityCards.Flop2 = cards[1];
        if (cards.Length > 2) communityCards.Flop3 = cards[2];
        if (cards.Length > 3) communityCards.Turn = cards[3];
        if (cards.Length > 4) communityCards.River = cards[4];
        return communityCards;
    }

    private class FakeDeck : IDeck
    {
        private readonly Queue<Card> _cards;

        public FakeDeck(IEnumerable<Card> cards)
        {
            _cards = new Queue<Card>(cards);
        }

        public Card DrawCard()
        {
            return _cards.Count > 0 ? _cards.Dequeue() : new Card(CardRank.Deux, Suit.Diamonds);
        }

        public void Shuffle()
        {
            // No shuffle needed for tests.
        }
    }
}
