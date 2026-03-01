using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Games.Poker.Players.StrategyTests.cs;

[TestClass]
public class AggressiveStrategyTests
{
    [TestMethod]
    public void DecideAction_DoitPrivilegierRelanceQuandPossible()
    {
        // Arrange
        var Player = new ComputerPlayer("Bot", 50, new AggressiveStrategy())
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.Roi, Suit.Spades))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand, startingBet: 15);
        var contexte = new GameContext(partie, Player, new List<PokerTypeAction> { PokerTypeAction.Raise, PokerTypeAction.Bet, PokerTypeAction.Call });
        var strategy = new AggressiveStrategy();

        // Act
        var action = strategy.DecideAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction, "La stratégie agressive doit relancer si possible.");
        Assert.AreEqual(contexte.MinimumBet, action.Amount, "La relance doit être au moins égale à la mise minimale.");
    }

    [TestMethod]
    public void DecideAction_SiRelanceImpossibleMaisMiserPossible_DoitMiser()
    {
        // Arrange
        var Player = new ComputerPlayer("Bot", 10, new AggressiveStrategy())
        {
            Hand = new HandCards(
                new Card(CardRank.Valet, Suit.Diamonds),
                new Card(CardRank.Dix, Suit.Clubs))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand);
        PlayerTestHelper.DefinirMiseActuelle(partie, 20);
        var contexte = new GameContext(partie, Player, new List<PokerTypeAction> { PokerTypeAction.Bet, PokerTypeAction.Call });
        var strategy = new AggressiveStrategy();

        // Act
        var action = strategy.DecideAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction, "La stratégie agressive doit miser si la relance n'est pas possible.");
        Assert.AreEqual(contexte.MinimumBet, action.Amount, "La mise doit respecter le minimum requis.");
    }
}
