using System.Collections.Generic;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Players;

[TestClass]
public class StrategieRandomTests
{
    [TestMethod]
    public void ProposerAction_RelancerLorsqueMinimumDepasseJetons_DoitAllerTapis()
    {
        // Arrange
        var Player = new PlayerHumain("Alice", 8)
        {
            Hand = new HandCards(
                new Card(RangCarte.As, Couleur.Coeur),
                new Card(RangCarte.Roi, Couleur.Pique))
        };
        var partie = PlayerTestHelper.CreerPartieAvecPlayer(Player, Player.Hand);
        PlayerTestHelper.DefinirMiseActuelle(partie, 5);
        var contexte = new ContexteDeJeu(partie, Player, new List<TypeActionJeu> { TypeActionJeu.Relancer });
        var strategie = new StrategieRandom();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Relancer, action.TypeAction, "La stratégie aléatoire doit respecter l'action proposée.");
        Assert.AreEqual(Player.Chips, action.Montant, "Si la relance minimale dépasse la mise disponible, l'action doit utiliser tous les jetons restants.");
    }

    [TestMethod]
    public void ProposerAction_Miser_DoiventUtiliserLaMiseMinimum()
    {
        // Arrange
        var Player = new PlayerHumain("Bob", 100)
        {
            Hand = new HandCards(
                new Card(RangCarte.Dame, Couleur.Carreau),
                new Card(RangCarte.Neuf, Couleur.Trefle))
        };
        var partie = PlayerTestHelper.CreerPartieAvecPlayer(Player, Player.Hand, startingBet: 25);
        var contexte = new ContexteDeJeu(partie, Player, new List<TypeActionJeu> { TypeActionJeu.Miser });
        var strategie = new StrategieRandom();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Miser, action.TypeAction, "L'action doit correspondre au choix tiré.");
        Assert.AreEqual(contexte.MinimumBet, action.Montant, "La mise doit correspondre au minimum requis.");
    }
}

[TestClass]
public class StrategieAgressiveTests
{
    [TestMethod]
    public void ProposerAction_DoitPrivilegierRelanceQuandPossible()
    {
        // Arrange
        var Player = new PlayerOrdi("Bot", 50, new StrategieAgressive())
        {
            Hand = new HandCards(
                new Card(RangCarte.As, Couleur.Coeur),
                new Card(RangCarte.Roi, Couleur.Pique))
        };
        var partie = PlayerTestHelper.CreerPartieAvecPlayer(Player, Player.Hand, startingBet: 15);
        var contexte = new ContexteDeJeu(partie, Player, new List<TypeActionJeu> { TypeActionJeu.Relancer, TypeActionJeu.Miser, TypeActionJeu.Suivre });
        var strategie = new StrategieAgressive();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Relancer, action.TypeAction, "La stratégie agressive doit relancer si possible.");
        Assert.AreEqual(contexte.MinimumBet, action.Montant, "La relance doit être au moins égale à la mise minimale.");
    }

    [TestMethod]
    public void ProposerAction_SiRelanceImpossibleMaisMiserPossible_DoitMiser()
    {
        // Arrange
        var Player = new PlayerOrdi("Bot", 10, new StrategieAgressive())
        {
            Hand = new HandCards(
                new Card(RangCarte.Valet, Couleur.Carreau),
                new Card(RangCarte.Dix, Couleur.Trefle))
        };
        var partie = PlayerTestHelper.CreerPartieAvecPlayer(Player, Player.Hand);
        PlayerTestHelper.DefinirMiseActuelle(partie, 20);
        var contexte = new ContexteDeJeu(partie, Player, new List<TypeActionJeu> { TypeActionJeu.Miser, TypeActionJeu.Suivre });
        var strategie = new StrategieAgressive();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Miser, action.TypeAction, "La stratégie agressive doit miser si la relance n'est pas possible.");
        Assert.AreEqual(contexte.MinimumBet, action.Montant, "La mise doit respecter le minimum requis.");
    }
}

[TestClass]
public class StrategieConservatriceTests
{
    [TestMethod]
    public void ProposerAction_PresenceDeCheck_DoitRetournerCheck()
    {
        // Arrange
        var Player = new PlayerHumain("Alice", 100)
        {
            Hand = new HandCards(
                new Card(RangCarte.As, Couleur.Coeur),
                new Card(RangCarte.Dix, Couleur.Carreau))
        };
        var partie = PlayerTestHelper.CreerPartieAvecPlayer(Player, Player.Hand);
        var contexte = new ContexteDeJeu(partie, Player, new List<TypeActionJeu> { TypeActionJeu.Check, TypeActionJeu.Suivre });
        var strategie = new StrategieConservatrice();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Check, action.TypeAction, "Le check doit être prioritaire lorsqu'il est disponible.");
    }

    [TestMethod]
    public void ProposerAction_SuivreDisponible_DoitSuivreAvecMainCorrecte()
    {
        // Arrange
        var Player = new PlayerHumain("Bob", 100)
        {
            Hand = new HandCards(
                new Card(RangCarte.As, Couleur.Coeur),
                new Card(RangCarte.As, Couleur.Carreau))
        };
        var communes = PlayerTestHelper.CreerCartesCommunes(
            new Card(RangCarte.Roi, Couleur.Pique),
            new Card(RangCarte.Roi, Couleur.Trefle),
            new Card(RangCarte.Dame, Couleur.Coeur));
        var partie = PlayerTestHelper.CreerPartieAvecPlayer(Player, Player.Hand, communes);
        var contexte = new ContexteDeJeu(partie, Player, new List<TypeActionJeu> { TypeActionJeu.Suivre, TypeActionJeu.Miser });
        var strategie = new StrategieConservatrice();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Suivre, action.TypeAction, "La stratégie conservatrice doit suivre avec une main correcte quand aucune check n'est possible.");
    }

    [TestMethod]
    public void ProposerAction_SiAucuneAutreActionNeConvient_DoitSeCoucher()
    {
        // Arrange
        var Player = new PlayerHumain("Claire", 100)
        {
            Hand = new HandCards(
                new Card(RangCarte.Deux, Couleur.Carreau),
                new Card(RangCarte.Sept, Couleur.Trefle))
        };
        var communes = PlayerTestHelper.CreerCartesCommunes(
            new Card(RangCarte.Dix, Couleur.Pique),
            new Card(RangCarte.Neuf, Couleur.Trefle),
            new Card(RangCarte.Huit, Couleur.Coeur));
        var partie = PlayerTestHelper.CreerPartieAvecPlayer(Player, Player.Hand, communes);
        var contexte = new ContexteDeJeu(partie, Player, new List<TypeActionJeu> { TypeActionJeu.SeCoucher });
        var strategie = new StrategieConservatrice();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.SeCoucher, action.TypeAction, "La stratégie conservatrice doit abandonner en dernier recours.");
    }
}
