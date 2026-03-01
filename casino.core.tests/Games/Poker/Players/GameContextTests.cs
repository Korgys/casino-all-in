using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Players;

[TestClass]
public class GameContextTests
{
    [TestMethod]
    public void Constructeur_DoitCalculerScoreEtConserverActions()
    {
        // Arrange
        var Player = new HumanPlayer("Alice", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.As, Suit.Coeur),
                new Card(CardRank.Roi, Suit.Carreau))
        };
        var communes = PlayerTestHelper.CreateCommunityCards(
            new Card(CardRank.As, Suit.Pique),
            new Card(CardRank.Dame, Suit.Trefle),
            new Card(CardRank.Neuf, Suit.Carreau));
        var actions = new List<TypeGameAction> { TypeGameAction.Check, TypeGameAction.Miser };

        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand, communes);

        // Act
        var contexte = new GameContext(partie, Player, actions);

        // Assert
        Assert.AreEqual(HandRank.Paire, contexte.PlayerScore.Rang, "Le score doit détecter la paire d'As.");
        Assert.AreEqual(CardRank.As, contexte.PlayerScore.Valeur, "La valeur de la main doit correspondre à l'As.");
        CollectionAssert.AreEquivalent(actions, contexte.AvailableActions.ToList(), "Les actions possibles doivent être conservées.");
        Assert.AreEqual(partie.StartingBet, contexte.MinimumBet, "La mise minimale doit être dérivée de la partie.");
    }

    [TestMethod]
    public void MiseMinimum_DoitTenirCompteDeLaMiseActuelleLaPlusHaute()
    {
        // Arrange
        var Player = new HumanPlayer("Bob", 100)
        {
            Hand = new HandCards(
                new Card(CardRank.Dix, Suit.Coeur),
                new Card(CardRank.Neuf, Suit.Carreau))
        };
        var partie = PlayerTestHelper.CreerRoundAvecPlayer(Player, Player.Hand);
        PlayerTestHelper.DefinirMiseActuelle(partie, 45);

        // Act
        var contexte = new GameContext(partie, Player, new List<TypeGameAction> { TypeGameAction.Miser });

        // Assert
        Assert.AreEqual(45, contexte.MinimumBet, "La mise minimale doit correspondre à la mise actuelle lorsqu'elle est supérieure à la mise de départ.");
    }
}
