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
                new Card(CardRank.As, Suit.Coeur),
                new Card(CardRank.Roi, Suit.Pique))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand);
        PlayerTestHelper.DefinirMiseActuelle(partie, 5);
        var contexte = new GameContext(partie, Player, new List<TypeGameAction> { TypeGameAction.Relancer });
        var strategie = new RandomStrategy();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeGameAction.Relancer, action.TypeAction, "La stratégie aléatoire doit respecter l'action proposée.");
        Assert.AreEqual(Player.Chips, action.Montant, "Si la relance minimale dépasse la mise disponible, l'action doit utiliser tous les jetons restants.");
    }

    [TestMethod]
    public void ProposerAction_Miser_DoiventUtiliserLaMiseMinimum()
    {
        // Arrange
        var Player = new HumanPlayer("Bob", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Dame, Suit.Carreau),
                new Card(CardRank.Neuf, Suit.Trefle))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand, startingBet: 25);
        var contexte = new GameContext(partie, Player, new List<TypeGameAction> { TypeGameAction.Miser });
        var strategie = new RandomStrategy();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeGameAction.Miser, action.TypeAction, "L'action doit correspondre au choix tiré.");
        Assert.AreEqual(contexte.MinimumBet, action.Montant, "La mise doit correspondre au minimum requis.");
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
                new Card(CardRank.As, Suit.Coeur),
                new Card(CardRank.Roi, Suit.Pique))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand, startingBet: 15);
        var contexte = new GameContext(partie, Player, new List<TypeGameAction> { TypeGameAction.Relancer, TypeGameAction.Miser, TypeGameAction.Suivre });
        var strategie = new AggressiveStrategy();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeGameAction.Relancer, action.TypeAction, "La stratégie agressive doit relancer si possible.");
        Assert.AreEqual(contexte.MinimumBet, action.Montant, "La relance doit être au moins égale à la mise minimale.");
    }

    [TestMethod]
    public void ProposerAction_SiRelanceImpossibleMaisMiserPossible_DoitMiser()
    {
        // Arrange
        var Player = new ComputerPlayer("Bot", 10, new AggressiveStrategy())
        {
            Hand = new HandCards(
                new Card(CardRank.Valet, Suit.Carreau),
                new Card(CardRank.Dix, Suit.Trefle))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand);
        PlayerTestHelper.DefinirMiseActuelle(partie, 20);
        var contexte = new GameContext(partie, Player, new List<TypeGameAction> { TypeGameAction.Miser, TypeGameAction.Suivre });
        var strategie = new AggressiveStrategy();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeGameAction.Miser, action.TypeAction, "La stratégie agressive doit miser si la relance n'est pas possible.");
        Assert.AreEqual(contexte.MinimumBet, action.Montant, "La mise doit respecter le minimum requis.");
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
                new Card(CardRank.As, Suit.Coeur),
                new Card(CardRank.Dix, Suit.Carreau))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand);
        var contexte = new GameContext(partie, Player, new List<TypeGameAction> { TypeGameAction.Check, TypeGameAction.Suivre });
        var strategie = new ConservativeStrategy();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeGameAction.Check, action.TypeAction, "Le check doit être prioritaire lorsqu'il est disponible.");
    }

    [TestMethod]
    public void ProposerAction_SuivreDisponible_DoitSuivreAvecMainCorrecte()
    {
        // Arrange
        var Player = new HumanPlayer("Bob", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Coeur),
                new Card(CardRank.As, Suit.Carreau))
        };
        var communes = PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Roi, Suit.Pique),
            new Card(CardRank.Roi, Suit.Trefle),
            new Card(CardRank.Dame, Suit.Coeur));
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand, communes);
        var contexte = new GameContext(partie, Player, new List<TypeGameAction> { TypeGameAction.Suivre, TypeGameAction.Miser });
        var strategie = new ConservativeStrategy();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeGameAction.Suivre, action.TypeAction, "La stratégie conservatrice doit suivre avec une main correcte quand aucune check n'est possible.");
    }

    [TestMethod]
    public void ProposerAction_SiAucuneAutreActionNeConvient_DoitSeCoucher()
    {
        // Arrange
        var Player = new HumanPlayer("Claire", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Deux, Suit.Carreau),
                new Card(CardRank.Sept, Suit.Trefle))
        };
        var communes = PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.Dix, Suit.Pique),
            new Card(CardRank.Neuf, Suit.Trefle),
            new Card(CardRank.Huit, Suit.Coeur));
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand, communes);
        var contexte = new GameContext(partie, Player, new List<TypeGameAction> { TypeGameAction.SeCoucher });
        var strategie = new ConservativeStrategy();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeGameAction.SeCoucher, action.TypeAction, "La stratégie conservatrice doit abandonner en dernier recours.");
    }
}
