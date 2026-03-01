using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace casino.core.tests.Games.Poker.Rounds;

[TestClass]
public class TablePokerTests
{
    private static IEnumerable<Card> CreerCartesParDefaut() => new[]
    {
        new Card(CardRank.As, Suit.Pique),
        new Card(CardRank.Roi, Suit.Coeur),
        new Card(CardRank.Dame, Suit.Trefle),
        new Card(CardRank.Valet, Suit.Carreau),
        new Card(CardRank.Neuf, Suit.Pique),
        new Card(CardRank.Huit, Suit.Carreau),
        new Card(CardRank.Sept, Suit.Pique),
        new Card(CardRank.Six, Suit.Coeur),
        new Card(CardRank.Cinq, Suit.Trefle)
    };

    private static IEnumerable<Card> CreerCartesPourAllIn() => new[]
    {
        new Card(CardRank.As, Suit.Coeur),
        new Card(CardRank.Roi, Suit.Coeur),
        new Card(CardRank.Dame, Suit.Pique),
        new Card(CardRank.Valet, Suit.Pique),
        new Card(CardRank.Dix, Suit.Trefle),
        new Card(CardRank.Neuf, Suit.Trefle),
        new Card(CardRank.Huit, Suit.Coeur),
        new Card(CardRank.Sept, Suit.Coeur),
        new Card(CardRank.Six, Suit.Coeur),
        new Card(CardRank.Cinq, Suit.Carreau),
        new Card(CardRank.Quatre, Suit.Carreau)
    };

    [TestMethod]
    public void TousLesPlayersMoinsUnSeCouchent_TermineLaRoundEtDeclareLeGagnant()
    {
        // Arrange
        var Players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 80),
            new HumanPlayer("Charlie", 120)
        };
        var table = new TablePoker();
        table.DemarrerRound(Players, new FakeDeck(CreerCartesParDefaut()));

        // Act - Alice puis Bob se couchent
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Miser, 10));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.SeCoucher));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Relancer, 20));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.SeCoucher));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Round.Phase);
        Assert.AreEqual("Charlie", table.Round.Winners.Single().Name);
        Assert.AreEqual(20, table.Round.CurrentBet);
    }

    [TestMethod]
    public void TraiterActionPlayer_ChecksAvancentLaPhaseEtRemettentAuPlayerInitial()
    {
        // Arrange
        var Players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100)
        };
        var table = new TablePoker();
        table.DemarrerRound(Players, new FakeDeck(CreerCartesParDefaut()));

        // Act
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(TypeGameAction.Miser, 10));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(TypeGameAction.Suivre));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(TypeGameAction.Check));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(TypeGameAction.Check));

        // Assert
        Assert.AreEqual(Phase.Turn, table.Round.Phase);
        Assert.AreEqual(table.PlayerInitialIndex, table.PlayerActuelIndex);
    }

    [TestMethod]
    public void TraiterActionPlayer_CoucherMetFinALaRoundQuandUnPlayerReste()
    {
        // Arrange
        var Players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100)
        };
        var table = new TablePoker();
        table.DemarrerRound(Players, new FakeDeck(CreerCartesParDefaut()));

        // Act
        table.TraiterActionPlayer(table.Players[table.PlayerActuelIndex], new GameAction(TypeGameAction.Miser, 10));
        table.TraiterActionPlayer(table.Players[table.PlayerActuelIndex], new GameAction(TypeGameAction.SeCoucher));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Round.Phase);
        Assert.HasCount(1, table.Round.Winners);
        Assert.AreEqual("Alice", table.Round.Winners.First().Name);
    }

    [TestMethod]
    public void TousLesPlayersSontATapis_LeTourDeMiseEstClotureAutomatiquement()
    {
        // Arrange
        var Players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100),
            new HumanPlayer("Charlie", 100)
        };
        var table = new TablePoker();
        table.DemarrerRound(Players, new FakeDeck(CreerCartesPourAllIn()));

        // Act : Alice ouvre en relançant son tapis, Bob et Charlie suivent en tapis
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Relancer, 100));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Tapis));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Tapis));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Round.Phase);
        Assert.AreEqual(300, table.Round.Pot);
        Assert.AreEqual(0, table.Round.CurrentBet);
        Assert.IsNotNull(table.Round.CommunityCards.Flop1);
        Assert.IsNotNull(table.Round.CommunityCards.River);
    }

    [TestMethod]
    public void RelanceTardive_ForceUnNouveauTourDepuisLePlayerInitial()
    {
        // Arrange
        var Players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100),
            new HumanPlayer("Charlie", 100)
        };
        var table = new TablePoker();
        table.DemarrerRound(Players, new FakeDeck(CreerCartesParDefaut()));

        // Alice check, Bob check
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Miser, 10));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Suivre));

        // Charlie mise tardivement
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Relancer, 20));

        // La phase ne doit pas avancer sans que les premiers Players rejouent
        Assert.AreEqual(Phase.PreFlop, table.Round.Phase);
        Assert.AreEqual(table.PlayerInitialIndex, table.CurrentPlayerIndex);
        Assert.AreEqual(20, table.Round.CurrentBet);

        // Alice et Bob doivent suivre avant d'avancer
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Suivre));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(TypeGameAction.Suivre));

        Assert.AreEqual(Phase.Flop, table.Round.Phase);
        Assert.AreEqual(table.PlayerInitialIndex, table.CurrentPlayerIndex);
        Assert.AreEqual(0, table.Round.CurrentBet);
        Assert.IsTrue(table.Players.Where(j => !j.IsFolded() && !j.IsAllIn()).All(j => j.LastAction == TypeGameAction.Aucune));
    }
}
