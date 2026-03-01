using System.Collections.Generic;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Players;

[TestClass]
public class RandomStrategyTests
{
    [TestMethod]
    public void ProposerAction_RelancerLorsqueMinimumDepasseJetons_DoitAllerTapis()
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
        var action = strategy.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction, "La stratégie aléatoire doit respecter l'action proposée.");
        Assert.AreEqual(Player.Chips, action.Amount, "Si la relance minimale dépasse la mise disponible, l'action doit utiliser tous les jetons restants.");
    }

    [TestMethod]
    public void ProposerAction_Miser_DoiventUtiliserLaMiseMinimum()
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
        var action = strategy.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction, "L'action doit correspondre au choix tiré.");
        Assert.AreEqual(contexte.MinimumBet, action.Amount, "La mise doit correspondre au minimum requis.");
    }
}

[TestClass]
public class AggressiveStrategyTests
{
    [TestMethod]
    public void ProposerAction_DoitPrivilegierRelanceQuandPossible()
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
        var action = strategy.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction, "La stratégie agressive doit relancer si possible.");
        Assert.AreEqual(contexte.MinimumBet, action.Amount, "La relance doit être au moins égale à la mise minimale.");
    }

    [TestMethod]
    public void ProposerAction_SiRelanceImpossibleMaisMiserPossible_DoitMiser()
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
        var action = strategy.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction, "La stratégie agressive doit miser si la relance n'est pas possible.");
        Assert.AreEqual(contexte.MinimumBet, action.Amount, "La mise doit respecter le minimum requis.");
    }
}

[TestClass]
public class ConservativeStrategyTests
{
    [TestMethod]
    public void ProposerAction_PresenceDeCheck_DoitRetournerCheck()
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
        var action = strategy.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Check, action.TypeAction, "Le check doit être prioritaire lorsqu'il est disponible.");
    }

    [TestMethod]
    public void ProposerAction_SuivreDisponible_DoitSuivreAvecMainCorrecte()
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
        var action = strategy.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Call, action.TypeAction, "La stratégie conservatrice doit suivre avec une main correcte quand aucune check n'est possible.");
    }

    [TestMethod]
    public void ProposerAction_SiAucuneAutreActionNeConvient_DoitSeCoucher()
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
        var action = strategy.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(PokerTypeAction.Fold, action.TypeAction, "La stratégie conservatrice doit abandonner en dernier recours.");
    }
}
