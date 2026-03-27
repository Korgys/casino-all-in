using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker;

[TestClass]
public class PokerStateRecordsTests
{
    [TestMethod]
    public void PokerPlayerState_Constructor_ShouldMapAllProperties()
    {
        var hand = new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts));
        var playerState = new PokerPlayerState(
            Name: "Charlie",
            Chips: 800,
            Contribution: 50,
            IsHuman: true,
            IsFolded: false,
            LastAction: PokerTypeAction.Call,
            Hand: hand,
            IsWinner: true);

        Assert.AreEqual("Charlie", playerState.Name);
        Assert.AreEqual(800, playerState.Chips);
        Assert.AreEqual(50, playerState.Contribution);
        Assert.IsTrue(playerState.IsHuman);
        Assert.IsFalse(playerState.IsFolded);
        Assert.AreEqual(PokerTypeAction.Call, playerState.LastAction);
        Assert.AreSame(hand, playerState.Hand);
        Assert.IsTrue(playerState.IsWinner);
    }

    [TestMethod]
    public void PokerGameState_Constructor_ShouldMapAllProperties()
    {
        var communityCards = new TableCards
        {
            Flop1 = new Card(CardRank.Dix, Suit.Clubs),
            Flop2 = new Card(CardRank.Valet, Suit.Clubs),
            Flop3 = new Card(CardRank.Dame, Suit.Clubs)
        };

        var players = new List<PokerPlayerState>
        {
            new("Alice", 1000, 0, true, false, PokerTypeAction.Check, null, false),
            new("Bot-1", 900, 100, false, false, PokerTypeAction.Bet, null, false)
        };

        var gameState = new PokerGameState(
            Phase: "Flop",
            StartingBet: 25,
            Pot: 200,
            CurrentBet: 100,
            CommunityCards: communityCards,
            Players: players,
            CurrentPlayer: "Alice");

        Assert.AreEqual("Flop", gameState.Phase);
        Assert.AreEqual(25, gameState.StartingBet);
        Assert.AreEqual(200, gameState.Pot);
        Assert.AreEqual(100, gameState.CurrentBet);
        Assert.AreSame(communityCards, gameState.CommunityCards);
        Assert.AreSame(players, gameState.Players);
        Assert.AreEqual("Alice", gameState.CurrentPlayer);
    }
}
