using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.tests.Games.Poker.Players.Strategies;

[TestClass]
public class OpportunisticStrategyTests
{
    [TestMethod]
    public void ProposerAction_ShouldAllInWhenOddsAreGoods()
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
        var gameAction = strategy.ProposerAction(gameContext);

        // Assert
        Assert.IsNotNull(gameAction);
        Assert.AreEqual(PokerTypeAction.AllIn, gameAction.TypeAction);
    }

    private static IEnumerable<Card> CreerCartesSimples() => Enumerable.Repeat(new Card(CardRank.Deux, Suit.Hearts), 10);
}