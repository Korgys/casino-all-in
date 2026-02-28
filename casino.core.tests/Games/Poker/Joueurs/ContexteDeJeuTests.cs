using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Players;

[TestClass]
public class ContexteDeJeuTests
{
    [TestMethod]
    public void Constructeur_DoitCalculerScoreEtConserverActions()
    {
        // Arrange
        var Player = new PlayerHumain("Alice", 100)
        {
            Hand = new HandCards(
                new Card(RangCarte.As, Couleur.Coeur),
                new Card(RangCarte.Roi, Couleur.Carreau))
        };
        var communes = PlayerTestHelper.CreerCartesCommunes(
            new Card(RangCarte.As, Couleur.Pique),
            new Card(RangCarte.Dame, Couleur.Trefle),
            new Card(RangCarte.Neuf, Couleur.Carreau));
        var actions = new List<TypeActionJeu> { TypeActionJeu.Check, TypeActionJeu.Miser };

        var partie = PlayerTestHelper.CreerPartieAvecPlayer(Player, Player.Hand, communes);

        // Act
        var contexte = new ContexteDeJeu(partie, Player, actions);

        // Assert
        Assert.AreEqual(RangMain.Paire, contexte.ScorePlayer.Rang, "Le score doit détecter la paire d'As.");
        Assert.AreEqual(RangCarte.As, contexte.ScorePlayer.Valeur, "La valeur de la main doit correspondre à l'As.");
        CollectionAssert.AreEquivalent(actions, contexte.ActionsPossibles.ToList(), "Les actions possibles doivent être conservées.");
        Assert.AreEqual(partie.StartingBet, contexte.MinimumBet, "La mise minimale doit être dérivée de la partie.");
    }

    [TestMethod]
    public void MiseMinimum_DoitTenirCompteDeLaMiseActuelleLaPlusHaute()
    {
        // Arrange
        var Player = new PlayerHumain("Bob", 100)
        {
            Hand = new HandCards(
                new Card(RangCarte.Dix, Couleur.Coeur),
                new Card(RangCarte.Neuf, Couleur.Carreau))
        };
        var partie = PlayerTestHelper.CreerPartieAvecPlayer(Player, Player.Hand);
        PlayerTestHelper.DefinirMiseActuelle(partie, 45);

        // Act
        var contexte = new ContexteDeJeu(partie, Player, new List<TypeActionJeu> { TypeActionJeu.Miser });

        // Assert
        Assert.AreEqual(45, contexte.MinimumBet, "La mise minimale doit correspondre à la mise actuelle lorsqu'elle est supérieure à la mise de départ.");
    }
}
