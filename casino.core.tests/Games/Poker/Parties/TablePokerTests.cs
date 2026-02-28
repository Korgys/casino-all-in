using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;
using casino.core.Games.Poker.Parties.Phases;
using casino.core.tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace casino.core.tests.Games.Poker.Parties;

[TestClass]
public class TablePokerTests
{
    private static IEnumerable<Card> CreerCartesParDefaut() => new[]
    {
        new Card(RangCarte.As, Couleur.Pique),
        new Card(RangCarte.Roi, Couleur.Coeur),
        new Card(RangCarte.Dame, Couleur.Trefle),
        new Card(RangCarte.Valet, Couleur.Carreau),
        new Card(RangCarte.Neuf, Couleur.Pique),
        new Card(RangCarte.Huit, Couleur.Carreau),
        new Card(RangCarte.Sept, Couleur.Pique),
        new Card(RangCarte.Six, Couleur.Coeur),
        new Card(RangCarte.Cinq, Couleur.Trefle)
    };

    private static IEnumerable<Card> CreerCartesPourAllIn() => new[]
    {
        new Card(RangCarte.As, Couleur.Coeur),
        new Card(RangCarte.Roi, Couleur.Coeur),
        new Card(RangCarte.Dame, Couleur.Pique),
        new Card(RangCarte.Valet, Couleur.Pique),
        new Card(RangCarte.Dix, Couleur.Trefle),
        new Card(RangCarte.Neuf, Couleur.Trefle),
        new Card(RangCarte.Huit, Couleur.Coeur),
        new Card(RangCarte.Sept, Couleur.Coeur),
        new Card(RangCarte.Six, Couleur.Coeur),
        new Card(RangCarte.Cinq, Couleur.Carreau),
        new Card(RangCarte.Quatre, Couleur.Carreau)
    };

    [TestMethod]
    public void DemarrerPartie_ReinitialiseEtRotationDuPlayerInitial()
    {
        // Arrange
        var Players = new List<Player>
        {
            new PlayerHumain("Alice", 100) { LastAction = TypeActionJeu.Tapis },
            new PlayerHumain("Bob", 0) { LastAction = TypeActionJeu.Miser }
        };
        var table = new TablePoker();

        // Act - première partie
        table.DemarrerPartie(Players, new FakeDeck(CreerCartesParDefaut()));

        // Assert
        Assert.AreEqual(0, table.PlayerInitialIndex);
        Assert.AreEqual(table.PlayerInitialIndex, table.CurrentPlayerIndex);
        Assert.IsTrue(table.Players.All(j => j.LastAction == (j.Chips > 0 ? TypeActionJeu.Aucune : TypeActionJeu.SeCoucher)));
        Assert.IsTrue(table.Players.All(j => !j.IsFolded() || j.Chips == 0));

        // Act - deuxième partie pour vérifier la rotation
        table.DemarrerPartie(Players, new FakeDeck(CreerCartesParDefaut()));

        // Assert
        Assert.AreEqual(1, table.PlayerInitialIndex);
        Assert.AreEqual(table.PlayerInitialIndex, table.PlayerActuelIndex);
    }

    [TestMethod]
    public void TousLesPlayersMoinsUnSeCouchent_TermineLaPartieEtDeclareLeGagnant()
    {
        // Arrange
        var Players = new List<Player>
        {
            new PlayerHumain("Alice", 100),
            new PlayerHumain("Bob", 80),
            new PlayerHumain("Charlie", 120)
        };
        var table = new TablePoker();
        table.DemarrerPartie(Players, new FakeDeck(CreerCartesParDefaut()));

        // Act - Alice puis Bob se couchent
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Miser, 10));
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.SeCoucher));
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Relancer, 20));
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.SeCoucher));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Partie.Phase);
        Assert.AreEqual("Charlie", table.Partie.Winners.Single().Name);
        Assert.AreEqual(20, table.Partie.CurrentBet);
    }

    [TestMethod]
    public void TraiterActionPlayer_ChecksAvancentLaPhaseEtRemettentAuPlayerInitial()
    {
        // Arrange
        var Players = new List<Player>
        {
            new PlayerHumain("Alice", 100),
            new PlayerHumain("Bob", 100)
        };
        var table = new TablePoker();
        table.DemarrerPartie(Players, new FakeDeck(CreerCartesParDefaut()));

        // Act
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new ActionJeu(TypeActionJeu.Miser, 10));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new ActionJeu(TypeActionJeu.Suivre));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new ActionJeu(TypeActionJeu.Check));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new ActionJeu(TypeActionJeu.Check));

        // Assert
        Assert.AreEqual(Phase.Turn, table.Partie.Phase);
        Assert.AreEqual(table.PlayerInitialIndex, table.PlayerActuelIndex);
    }

    [TestMethod]
    public void TraiterActionPlayer_CoucherMetFinALaPartieQuandUnPlayerReste()
    {
        // Arrange
        var Players = new List<Player>
        {
            new PlayerHumain("Alice", 100),
            new PlayerHumain("Bob", 100)
        };
        var table = new TablePoker();
        table.DemarrerPartie(Players, new FakeDeck(CreerCartesParDefaut()));

        // Act
        table.TraiterActionPlayer(table.Players[table.PlayerActuelIndex], new ActionJeu(TypeActionJeu.Miser, 10));
        table.TraiterActionPlayer(table.Players[table.PlayerActuelIndex], new ActionJeu(TypeActionJeu.SeCoucher));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Partie.Phase);
        Assert.HasCount(1, table.Partie.Winners);
        Assert.AreEqual("Alice", table.Partie.Winners.First().Name);
    }

    [TestMethod]
    public void TousLesPlayersSontATapis_LeTourDeMiseEstClotureAutomatiquement()
    {
        // Arrange
        var Players = new List<Player>
        {
            new PlayerHumain("Alice", 100),
            new PlayerHumain("Bob", 100),
            new PlayerHumain("Charlie", 100)
        };
        var table = new TablePoker();
        table.DemarrerPartie(Players, new FakeDeck(CreerCartesPourAllIn()));

        // Act : Alice ouvre en relançant son tapis, Bob et Charlie suivent en tapis
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Relancer, 100));
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Tapis));
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Tapis));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Partie.Phase);
        Assert.AreEqual(300, table.Partie.Pot);
        Assert.AreEqual(0, table.Partie.CurrentBet);
        Assert.IsNotNull(table.Partie.CommunityCards.Flop1);
        Assert.IsNotNull(table.Partie.CommunityCards.River);
    }

    [TestMethod]
    public void RelanceTardive_ForceUnNouveauTourDepuisLePlayerInitial()
    {
        // Arrange
        var Players = new List<Player>
        {
            new PlayerHumain("Alice", 100),
            new PlayerHumain("Bob", 100),
            new PlayerHumain("Charlie", 100)
        };
        var table = new TablePoker();
        table.DemarrerPartie(Players, new FakeDeck(CreerCartesParDefaut()));

        // Alice check, Bob check
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Miser, 10));
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Suivre));

        // Charlie mise tardivement
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Relancer, 20));

        // La phase ne doit pas avancer sans que les premiers Players rejouent
        Assert.AreEqual(Phase.PreFlop, table.Partie.Phase);
        Assert.AreEqual(table.PlayerInitialIndex, table.CurrentPlayerIndex);
        Assert.AreEqual(20, table.Partie.CurrentBet);

        // Alice et Bob doivent suivre avant d'avancer
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Suivre));
        table.TraiterActionPlayer(table.ObtenirPlayerQuiDoitJouer(), new ActionJeu(TypeActionJeu.Suivre));

        Assert.AreEqual(Phase.Flop, table.Partie.Phase);
        Assert.AreEqual(table.PlayerInitialIndex, table.CurrentPlayerIndex);
        Assert.AreEqual(0, table.Partie.CurrentBet);
        Assert.IsTrue(table.Players.Where(j => !j.IsFolded() && !j.IsAllIn()).All(j => j.LastAction == TypeActionJeu.Aucune));
    }
}
