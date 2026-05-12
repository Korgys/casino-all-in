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
        new Card(CardRank.Ace, Suit.Spades),
        new Card(CardRank.King, Suit.Hearts),
        new Card(CardRank.Queen, Suit.Clubs),
        new Card(CardRank.Jack, Suit.Diamonds),
        new Card(CardRank.Nine, Suit.Spades),
        new Card(CardRank.Eight, Suit.Diamonds),
        new Card(CardRank.Seven, Suit.Spades),
        new Card(CardRank.Six, Suit.Hearts),
        new Card(CardRank.Five, Suit.Clubs)
    };

    private static IEnumerable<Card> CreateAllInCards() => new[]
    {
        new Card(CardRank.Ace, Suit.Hearts),
        new Card(CardRank.King, Suit.Hearts),
        new Card(CardRank.Queen, Suit.Spades),
        new Card(CardRank.Jack, Suit.Spades),
        new Card(CardRank.Ten, Suit.Clubs),
        new Card(CardRank.Nine, Suit.Clubs),
        new Card(CardRank.Eight, Suit.Hearts),
        new Card(CardRank.Seven, Suit.Hearts),
        new Card(CardRank.Six, Suit.Hearts),
        new Card(CardRank.Five, Suit.Diamonds),
        new Card(CardRank.Four, Suit.Diamonds)
    };

    [TestMethod]
    public void StartRound_ResetsPlayersAndRotatesInitialPlayer()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100) { LastAction = PokerTypeAction.AllIn },
            new HumanPlayer("Bob", 0) { LastAction = PokerTypeAction.Bet }
        };
        var table = new TablePoker();

        // Act - première round
        table.StartRound(players, new FakeDeck(CreateDefaultCards()));

        // Assert
        Assert.AreEqual(0, table.InitialPlayerIndex);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
        Assert.IsTrue(table.Players.All(j => j.LastAction == (j.Chips > 0 ? PokerTypeAction.None : PokerTypeAction.Fold)));
        Assert.IsTrue(table.Players.All(j => !j.IsFolded() || j.Chips == 0));

        // Act - deuxième round pour vérifier la rotation
        table.StartRound(players, new FakeDeck(CreateDefaultCards()));

        // Assert
        Assert.AreEqual(1, table.InitialPlayerIndex);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
    }

    [TestMethod]
    public void AllButOnePlayersFold_EndsRoundAndDeclaresWinner()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 80),
            new HumanPlayer("Charlie", 120)
        };
        var table = new TablePoker();
        table.StartRound(players, new FakeDeck(CreateDefaultCards()));

        // Act - Alice puis Bob se couchent
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Bet, 10));
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Fold));
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Raise, 20));
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Fold));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Round.Phase);
        Assert.AreEqual("Charlie", table.Round.Winners.Single().Name);
        Assert.AreEqual(20, table.Round.CurrentBet);
    }

    [TestMethod]
    public void ProcessPlayerAction_ChecksAdvancePhaseAndReturnToInitialPlayer()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100)
        };
        var table = new TablePoker();
        table.StartRound(players, new FakeDeck(CreateDefaultCards()));

        // Act
        table.ProcessPlayerAction(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Bet, 10));
        table.ProcessPlayerAction(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Call));
        table.ProcessPlayerAction(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Check));
        table.ProcessPlayerAction(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Check));

        // Assert
        Assert.AreEqual(Phase.Turn, table.Round.Phase);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
    }

    [TestMethod]
    public void ProcessPlayerAction_FoldEndsRoundWhenOnePlayerRemains()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100)
        };
        var table = new TablePoker();
        table.StartRound(players, new FakeDeck(CreateDefaultCards()));

        // Act
        table.ProcessPlayerAction(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Bet, 10));
        table.ProcessPlayerAction(table.Players[table.CurrentPlayerIndex], new GameAction(PokerTypeAction.Fold));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Round.Phase);
        Assert.HasCount(1, table.Round.Winners);
        Assert.AreEqual("Alice", table.Round.Winners.First().Name);
    }

    [TestMethod]
    public void AllPlayersAreAllIn_BettingRoundClosesAutomatically()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100),
            new HumanPlayer("Charlie", 100)
        };
        var table = new TablePoker();
        table.StartRound(players, new FakeDeck(CreateAllInCards()));

        // Act : Alice ouvre en relançant son tapis, Bob et Charlie suivent en tapis
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Raise, 100));
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.AllIn));
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.AllIn));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Round.Phase);
        Assert.AreEqual(300, table.Round.Pot);
        Assert.AreEqual(0, table.Round.CurrentBet);
        Assert.IsNotNull(table.Round.CommunityCards.Flop1);
        Assert.IsNotNull(table.Round.CommunityCards.River);
    }

    [TestMethod]
    public void LateRaise_ForcesNewTurnFromInitialPlayer()
    {
        // Arrange
        var players = new List<Player>
        {
            new HumanPlayer("Alice", 100),
            new HumanPlayer("Bob", 100),
            new HumanPlayer("Charlie", 100)
        };
        var table = new TablePoker();
        table.StartRound(players, new FakeDeck(CreateDefaultCards()));

        // Alice check, Bob check
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Bet, 10));
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Call));

        // Charlie mise tardivement
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Raise, 20));

        // La phase ne doit pas avancer sans que les premiers players rejouent
        Assert.AreEqual(Phase.PreFlop, table.Round.Phase);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
        Assert.AreEqual(20, table.Round.CurrentBet);

        // Alice et Bob doivent suivre avant d'avancer
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Call));
        table.ProcessPlayerAction(table.GetPlayerToAct(), new GameAction(PokerTypeAction.Call));

        Assert.AreEqual(Phase.Flop, table.Round.Phase);
        Assert.AreEqual(table.InitialPlayerIndex, table.CurrentPlayerIndex);
        Assert.AreEqual(0, table.Round.CurrentBet);
        Assert.IsTrue(table.Players.Where(j => !j.IsFolded() && !j.IsAllIn()).All(j => j.LastAction == PokerTypeAction.None));
    }
}
