using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.tests.Fakes;

namespace casino.core.tests.Games.Poker.Rounds;

[TestClass]
public class TablePokerTests
{
    private static IEnumerable<Card> CreateDefaultCards() => new[]
    {
        new Card(CardRank.As, Suit.Spades),
        new Card(CardRank.Roi, Suit.Hearts),
        new Card(CardRank.Dame, Suit.Clubs),
        new Card(CardRank.Valet, Suit.Diamonds),
        new Card(CardRank.Neuf, Suit.Spades),
        new Card(CardRank.Huit, Suit.Diamonds),
        new Card(CardRank.Sept, Suit.Spades),
        new Card(CardRank.Six, Suit.Hearts),
        new Card(CardRank.Cinq, Suit.Clubs)
    };

    private static IEnumerable<Card> CreateAllInCards() => new[]
    {
        new Card(CardRank.As, Suit.Hearts),
        new Card(CardRank.Roi, Suit.Hearts),
        new Card(CardRank.Dame, Suit.Spades),
        new Card(CardRank.Valet, Suit.Spades),
        new Card(CardRank.Dix, Suit.Clubs),
        new Card(CardRank.Neuf, Suit.Clubs),
        new Card(CardRank.Huit, Suit.Hearts),
        new Card(CardRank.Sept, Suit.Hearts),
        new Card(CardRank.Six, Suit.Hearts),
        new Card(CardRank.Cinq, Suit.Diamonds),
        new Card(CardRank.Quatre, Suit.Diamonds)
    };

    [TestMethod]
    public void DemarrerRound_ReinitialiseEtRotationDuPlayerInitial()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100) { LastAction = PokerTypeAction.AllIn },
            new HumanPlayer("Bob", 0) { LastAction = PokerTypeAction.Bet }
        };
        var table = new TablePoker();

        // Act - première round
        table.DemarrerRound(players, new FakeDeck(CreateDefaultCards()));

        // Assert
        Assert.AreEqual(0, table.InitialPlayerIndex);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
        Assert.IsTrue(table.Players.All(j => j.LastAction == (j.Chips > 0 ? PokerTypeAction.None : PokerTypeAction.Fold)));
        Assert.IsTrue(table.Players.All(j => !j.IsFolded() || j.Chips == 0));

        // Act - deuxième round pour vérifier la rotation
        table.DemarrerRound(players, new FakeDeck(CreateDefaultCards()));

        // Assert
        Assert.AreEqual(1, table.InitialPlayerIndex);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
    }

    [TestMethod]
    public void TousLesPlayersMoinsUnSeCouchent_TermineLaRoundEtDeclareLeGagnant()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 80),
            new HumanPlayer("Charlie", 120)
        };
        var table = new TablePoker();
        table.DemarrerRound(players, new FakeDeck(CreateDefaultCards()));

        // Act - Alice puis Bob se couchent
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Bet, 10));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Fold));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Raise, 20));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Fold));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Round.Phase);
        Assert.AreEqual("Charlie", table.Round.Winners.Single().Name);
        Assert.AreEqual(20, table.Round.CurrentBet);
    }

    [TestMethod]
    public void TraiterActionPlayer_ChecksAvancentLaPhaseEtRemettentAuPlayerInitial()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100)
        };
        var table = new TablePoker();
        table.DemarrerRound(players, new FakeDeck(CreateDefaultCards()));

        // Act
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Bet, 10));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Call));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Check));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Check));

        // Assert
        Assert.AreEqual(Phase.Turn, table.Round.Phase);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
    }

    [TestMethod]
    public void TraiterActionPlayer_CoucherMetFinALaRoundQuandUnPlayerReste()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100)
        };
        var table = new TablePoker();
        table.DemarrerRound(players, new FakeDeck(CreateDefaultCards()));

        // Act
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Bet, 10));
        table.TraiterActionPlayer(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Fold));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Round.Phase);
        Assert.HasCount(1, table.Round.Winners);
        Assert.AreEqual("Alice", table.Round.Winners.First().Name);
    }

    [TestMethod]
    public void TousLesPlayersSontATapis_LeTourDeMiseEstClotureAutomatiquement()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100),
            new HumanPlayer("Charlie", 100)
        };
        var table = new TablePoker();
        table.DemarrerRound(players, new FakeDeck(CreateAllInCards()));

        // Act : Alice ouvre en relançant son tapis, Bob et Charlie suivent en tapis
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Raise, 100));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.AllIn));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.AllIn));

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
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100),
            new HumanPlayer("Charlie", 100)
        };
        var table = new TablePoker();
        table.DemarrerRound(players, new FakeDeck(CreateDefaultCards()));

        // Alice check, Bob check
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Bet, 10));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Call));

        // Charlie mise tardivement
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Raise, 20));

        // La phase ne doit pas avancer sans que les premiers players rejouent
        Assert.AreEqual(Phase.PreFlop, table.Round.Phase);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
        Assert.AreEqual(20, table.Round.CurrentBet);

        // Alice et Bob doivent suivre avant d'avancer
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Call));
        table.TraiterActionPlayer(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Call));

        Assert.AreEqual(Phase.Flop, table.Round.Phase);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
        Assert.AreEqual(0, table.Round.CurrentBet);
        Assert.IsTrue(table.Players.Where(j => !j.IsFolded() && !j.IsAllIn()).All(j => j.LastAction == PokerTypeAction.None));
    }
}
