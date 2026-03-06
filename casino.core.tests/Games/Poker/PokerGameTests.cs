using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace casino.core.tests.Games.Poker;

[TestClass]
public class PokerGameTests
{
    [TestMethod]
    public void Run_SimpleRoundDeuxPlayersUnGagnant()
    {
        // Arrange
        var human = new HumanPlayer("Alice", 100);
        var autreHumain = new HumanPlayer("Bob", 100);

        var deckCards = new[]
        {
            new Card(CardRank.As, Suit.Spades),        // Alice 1
            new Card(CardRank.Roi, Suit.Hearts),       // Alice 2
            new Card(CardRank.Neuf, Suit.Clubs),     // Bob 1
            new Card(CardRank.Huit, Suit.Diamonds),    // Bob 2
            new Card(CardRank.As, Suit.Hearts),        // Flop 1 (paire d'As pour Alice)
            new Card(CardRank.Deux, Suit.Hearts),      // Flop 2
            new Card(CardRank.Trois, Suit.Clubs),    // Flop 3
            new Card(CardRank.Sept, Suit.Spades),      // Turn
            new Card(CardRank.Dame, Suit.Diamonds)     // River
        };

        var deckFactory = () => new FakeDeck(deckCards);

        // 1er tour : Alice (premier Player) doit miser, Bob peut suivre.
        var actionsParPlayer = new Dictionary<string, Queue<GameAction>>
        {
            ["Alice"] = new Queue<GameAction>(new[]
            {
                new GameAction(PokerTypeAction.Bet, 10), // 1er tour, 1er Player : mise obligatoire (>= 10 géré dans PokerGame)
                new GameAction(PokerTypeAction.Check),
                new GameAction(PokerTypeAction.Check),
                new GameAction(PokerTypeAction.Check)
            }),
            ["Bob"] = new Queue<GameAction>(new[]
            {
                new GameAction(PokerTypeAction.Call), // Il suit la mise d’Alice
                new GameAction(PokerTypeAction.Check),
                new GameAction(PokerTypeAction.Check),
                new GameAction(PokerTypeAction.Check)
            })
        };

        PokerGameState? etatFinal = null;
        string? gagnant = null;

        var pokerGame = new PokerGame(
            new[] { human, autreHumain },
            deckFactory,
            context => actionsParPlayer[context.PlayerName].Dequeue(),
            () => false,
            new NoOpWaitStrategy());

        pokerGame.StateUpdated += (_, args) => etatFinal = args.State as PokerGameState;
        pokerGame.GameEnded += (_, args) => gagnant = args.WinnerName;

        // Act
        pokerGame.Run();

        // Assert
        Assert.IsNotNull(etatFinal);
        Assert.AreEqual("Showdown", etatFinal.Phase);

        // Ici WinnerName doit être "Alice" 
        Assert.AreEqual("Alice", gagnant);

        Assert.IsTrue(etatFinal.Players.Single(j => j.Name == "Alice").IsWinner);
        Assert.IsFalse(etatFinal.Players.Single(j => j.Name == "Bob").IsWinner);
    }


    [TestMethod]
    public void Run_SimpleRoundDeuxPlayersDeuxGagnants()
    {
        // Arrange
        var human = new HumanPlayer("Alice", 100);
        var autreHumain = new HumanPlayer("Bob", 100);
        var deckCards = new[]
        {
            new Card(CardRank.As, Suit.Spades),        // Alice 1
            new Card(CardRank.Roi, Suit.Hearts),       // Alice 2
            new Card(CardRank.Neuf, Suit.Clubs),     // Bob 1
            new Card(CardRank.Huit, Suit.Diamonds),    // Bob 2
            new Card(CardRank.Deux, Suit.Hearts),      // Flop 1
            new Card(CardRank.Trois, Suit.Clubs),    // Flop 2
            new Card(CardRank.Quatre, Suit.Diamonds),  // Flop 3
            new Card(CardRank.Cinq, Suit.Spades),      // Turn
            new Card(CardRank.Six, Suit.Hearts)        // River
        };

        var deckFactory = () => new FakeDeck(deckCards);

        // Idem : 1er tour, Alice doit miser, Bob suit.
        var actionsParPlayer = new Dictionary<string, Queue<GameAction>>
        {
            ["Alice"] = new Queue<GameAction>(new[]
            {
                new GameAction(PokerTypeAction.Bet, 10),
                new GameAction(PokerTypeAction.Check),
                new GameAction(PokerTypeAction.Check),
                new GameAction(PokerTypeAction.Check)
            }),
            ["Bob"] = new Queue<GameAction>(new[]
            {
                new GameAction(PokerTypeAction.Call),
                new GameAction(PokerTypeAction.Check),
                new GameAction(PokerTypeAction.Check),
                new GameAction(PokerTypeAction.Check)
            })
        };

        PokerGameState? etatFinal = null;
        string? gagnant = null;

        var pokerGame = new PokerGame(
            new[] { human, autreHumain },
            deckFactory,
            context => actionsParPlayer[context.PlayerName].Dequeue(),
            () => false,
            new NoOpWaitStrategy());

        pokerGame.StateUpdated += (_, args) => etatFinal = args.State as PokerGameState;
        pokerGame.GameEnded += (_, args) => gagnant = args.WinnerName;

        // Act
        pokerGame.Run();

        // Assert
        Assert.IsNotNull(etatFinal);
        Assert.AreEqual("Showdown", etatFinal.Phase);
        Assert.AreEqual("Alice, Bob", gagnant);
        Assert.IsTrue(etatFinal.Players.All(j => j.IsWinner));
    }

    [TestMethod]
    public void Run_ContinueTantQueJetonsEtDemandePourRelancer()
    {
        // Arrange
        var human = new HumanPlayer("Alice", 100);
        var autreHumain = new HumanPlayer("Bob", 100);
        var deckCards = new[]
        {
            new Card(CardRank.As, Suit.Spades),
            new Card(CardRank.Roi, Suit.Hearts),
            new Card(CardRank.Neuf, Suit.Clubs),
            new Card(CardRank.Huit, Suit.Diamonds),
            new Card(CardRank.Deux, Suit.Hearts),
            new Card(CardRank.Trois, Suit.Clubs),
            new Card(CardRank.Quatre, Suit.Diamonds),
            new Card(CardRank.Cinq, Suit.Spades),
            new Card(CardRank.Six, Suit.Hearts)
        };

        var deckFactory = () => new FakeDeck(deckCards);

        var actionsParPlayer = new Dictionary<string, Queue<GameAction>>
        {
            ["Alice"] = new Queue<GameAction>(new[]
            {
                new GameAction(PokerTypeAction.Bet, 10),
                new GameAction(PokerTypeAction.Bet, 10),
                new GameAction(PokerTypeAction.Bet, 10),
                new GameAction(PokerTypeAction.Bet, 10),
                new GameAction(PokerTypeAction.Call),
                new GameAction(PokerTypeAction.Call),
                new GameAction(PokerTypeAction.Call),
                new GameAction(PokerTypeAction.Call)
            }),
            ["Bob"] = new Queue<GameAction>(new[]
            {
                new GameAction(PokerTypeAction.Call),
                new GameAction(PokerTypeAction.Call),
                new GameAction(PokerTypeAction.Call),
                new GameAction(PokerTypeAction.Call),
                new GameAction(PokerTypeAction.Bet, 10),
                new GameAction(PokerTypeAction.Bet, 10),
                new GameAction(PokerTypeAction.Bet, 10),
                new GameAction(PokerTypeAction.Bet, 10)
            })
        };

        var continuer = new Queue<bool>(new[] { true, false });
        var continueCalled = 0;
        var pokerGame = new PokerGame(
            new[] { human, autreHumain },
            deckFactory,
            context => actionsParPlayer[context.PlayerName].Dequeue(),
            () =>
            {
                continueCalled++;
                return continuer.Dequeue();
            },
            new NoOpWaitStrategy());

        var events = new List<string>();
        pokerGame.StateUpdated += (_, _) => events.Add("state");
        pokerGame.GameEnded += (_, _) => events.Add("ended");

        // Act
        pokerGame.Run();

        // Assert
        Assert.AreEqual(2, events.Count(e => e == "ended"));
        Assert.AreEqual(2, continueCalled);

        var indexesGameEnded = events
            .Select((evt, index) => (evt, index))
            .Where(e => e.evt == "ended")
            .Select(e => e.index)
            .ToList();

        Assert.IsTrue(indexesGameEnded.All(idx => events.Skip(idx + 1).Any(e => e == "state")),
            "StateUpdated doit être déclenché après chaque fin de partie.");
    }
}
