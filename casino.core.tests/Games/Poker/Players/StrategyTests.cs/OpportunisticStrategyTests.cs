using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.tests.Fakes;
using casino.core.tests.Games.Poker.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.tests.Games.Poker.Players.Strategies;

[TestClass]
public class OpportunisticStrategyTests
{
    [TestMethod]
    public void DecideAction_ShouldAllInWhenOddsAreGoods()
    {
        // Arrange
        var player = new Player("bob", 1000);
        var players = new List<Player>()
        {
            player,
            new Player("alice", 1000)
        };
        var round = new Round(players, new FakeDeck(CreerCartesSimples()));
        var gameContext = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.AllIn });
        var strategy = new OpportunisticStrategy();

        // Act
        var gameAction = strategy.DecideAction(gameContext);

        // Assert
        Assert.IsNotNull(gameAction);
        Assert.AreEqual(PokerTypeAction.AllIn, gameAction.TypeAction);
    }

    [TestMethod]
    public void DecideAction_ShouldBetWithMinimumBet()
    {
        // Arrange
        var player = new Player("bob", 1000);
        var players = new List<Player>()
        {
            player,
            new Player("alice", 1000)
        };
        var round = new Round(players, new FakeDeck(CreerCartesSimples()));
        var gameContext = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Bet });
        var strategy = new OpportunisticStrategy();

        // Act
        var gameAction = strategy.DecideAction(gameContext);

        // Assert
        Assert.IsNotNull(gameAction);
        Assert.AreEqual(PokerTypeAction.Bet, gameAction.TypeAction);
        Assert.AreEqual(10, gameAction.Amount);
    }


    [TestMethod]
    public void DecideAction_ShouldRaiseWhenMostOpponentsAreFolded()
    {
        // Arrange
        var player = new Player("bob", 1000);
        var opponentActive = new Player("alice", 1000);
        var foldedOne = new Player("charlie", 1000) { LastAction = PokerTypeAction.Fold };
        var foldedTwo = new Player("diana", 1000) { LastAction = PokerTypeAction.Fold };
        var foldedThree = new Player("eric", 1000) { LastAction = PokerTypeAction.Fold };

        var players = new List<Player>
        {
            player,
            opponentActive,
            foldedOne,
            foldedTwo,
            foldedThree
        };

        var cards = new List<Card>
        {
            new(CardRank.As, Suit.Spades),
            new(CardRank.As, Suit.Hearts),
            new(CardRank.Deux, Suit.Clubs),
            new(CardRank.Trois, Suit.Diamonds),
            new(CardRank.Quatre, Suit.Spades),
            new(CardRank.Cinq, Suit.Hearts),
            new(CardRank.Six, Suit.Clubs),
            new(CardRank.Sept, Suit.Diamonds),
            new(CardRank.Huit, Suit.Spades),
            new(CardRank.Neuf, Suit.Hearts)
        };

        var round = new Round(players, new FakeDeck(cards));
        var gameContext = new GameContext(
            round,
            player,
            new List<PokerTypeAction> { PokerTypeAction.Fold, PokerTypeAction.Call, PokerTypeAction.Raise });
        var strategy = new OpportunisticStrategy();

        // Act
        var gameAction = strategy.DecideAction(gameContext);

        // Assert
        Assert.IsNotNull(gameAction);
        Assert.AreEqual(PokerTypeAction.Raise, gameAction.TypeAction);
        Assert.AreEqual(gameContext.MinimumBet, gameAction.Amount);
    }


    [TestMethod]
    public void DecideAction_WithRaiseAvailableAndExistingCurrentBet_ShouldRaiseAboveCurrentBet()
    {
        // Arrange
        var player = new Player("bob", 1000);
        var players = new List<Player>()
        {
            player,
            new Player("alice", 1000)
        };
        var round = new Round(players, new FakeDeck(CreerCartesSimples()));
        PlayerTestHelper.DefinirMiseActuelle(round, 14);
        var gameContext = new GameContext(round, player, new List<PokerTypeAction> { PokerTypeAction.Raise });
        var strategy = new OpportunisticStrategy();

        // Act
        var gameAction = strategy.DecideAction(gameContext);

        // Assert
        Assert.IsNotNull(gameAction);
        Assert.AreEqual(PokerTypeAction.Raise, gameAction.TypeAction);
        Assert.AreEqual(15, gameAction.Amount);
    }

    private static IEnumerable<Card> CreerCartesSimples() => Enumerable.Repeat(new Card(CardRank.Deux, Suit.Hearts), 10);
}