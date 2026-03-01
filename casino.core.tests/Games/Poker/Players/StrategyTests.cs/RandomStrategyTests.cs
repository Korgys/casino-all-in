using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Games.Poker.Players.StrategyTests.cs;

[TestClass]
public class RandomStrategyTests
{
    [TestMethod]
    public void DecideAction_RelancerLorsqueMinimumDepasseJetons_DoitAllerTapis()
    {
        // Arrange
        var Player = new HumanPlayer("Alice", 8)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.Roi, Suit.Spades))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand);
        PlayerTestHelper.DefinirMiseActuelle(partie, 5);
        var contexte = new GameContext(partie, Player, new List<PokerTypeAction> { PokerTypeAction.Raise });
        var strategy = new RandomStrategy();

        // Act
        var action = strategy.DecideAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction, "La stratégie aléatoire doit respecter l'action proposée.");
        Assert.AreEqual(Player.Chips, action.Amount, "Si la relance minimale dépasse la mise disponible, l'action doit utiliser tous les jetons restants.");
    }

    [TestMethod]
    public void DecideAction_Miser_DoiventUtiliserLaMiseMinimum()
    {
        // Arrange
        var Player = new HumanPlayer("Bob", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Dame, Suit.Diamonds),
                new Card(CardRank.Neuf, Suit.Clubs))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand, startingBet: 25);
        var contexte = new GameContext(partie, Player, new List<PokerTypeAction> { PokerTypeAction.Bet });
        var strategy = new RandomStrategy();

        // Act
        var action = strategy.DecideAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction, "L'action doit correspondre au choix tiré.");
        Assert.AreEqual(contexte.MinimumBet, action.Amount, "La mise doit correspondre au minimum requis.");
    }
}
