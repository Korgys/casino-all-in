using System.Collections.Generic;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Players.StrategyTests.cs;

[TestClass]
public class ConservativeStrategyTests
{
    [TestMethod]
    public void DecideAction_PresenceDeCheck_DoitRetournerCheck()
    {
        // Arrange
        var Player = new HumanPlayer("Alice", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.Dix, Suit.Diamonds))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand);
        var contexte = new GameContext(partie, Player, new List<PokerTypeAction> { PokerTypeAction.Check, PokerTypeAction.Call });
        var strategy = new ConservativeStrategy();

        // Act
        var action = strategy.DecideAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Check, action.TypeAction, "Le check doit être prioritaire lorsqu'il est disponible.");
    }

    [TestMethod]
    public void DecideAction_SuivreDisponible_DoitSuivreAvecMainCorrecte()
    {
        // Arrange
        var Player = new HumanPlayer("Bob", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Hearts),
                new Card(CardRank.As, Suit.Diamonds))
        };
        var communes = PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Roi, Suit.Spades),
            new Card(CardRank.Roi, Suit.Clubs),
            new Card(CardRank.Dame, Suit.Hearts));
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand, communes);
        var contexte = new GameContext(partie, Player, new List<PokerTypeAction> { PokerTypeAction.Call, PokerTypeAction.Bet });
        var strategy = new ConservativeStrategy();

        // Act
        var action = strategy.DecideAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Call, action.TypeAction, "La stratégie conservatrice doit suivre avec une main correcte quand aucune check n'est possible.");
    }

    [TestMethod]
    public void DecideAction_SiAucuneAutreActionNeConvient_DoitSeCoucher()
    {
        // Arrange
        var Player = new HumanPlayer("Claire", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Deux, Suit.Diamonds),
                new Card(CardRank.Sept, Suit.Clubs))
        };
        var communes = PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Dix, Suit.Spades),
            new Card(CardRank.Neuf, Suit.Clubs),
            new Card(CardRank.Huit, Suit.Hearts));
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand, communes);
        var contexte = new GameContext(partie, Player, new List<PokerTypeAction> { PokerTypeAction.Fold });
        var strategy = new ConservativeStrategy();

        // Act
        var action = strategy.DecideAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Fold, action.TypeAction, "La stratégie conservatrice doit abandonner en dernier recours.");
    }
}
