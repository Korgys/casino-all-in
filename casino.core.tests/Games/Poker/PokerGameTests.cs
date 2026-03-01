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
            new Card(CardRank.As, Suit.Pique),        // Alice 1
            new Card(CardRank.Roi, Suit.Coeur),       // Alice 2
            new Card(CardRank.Neuf, Suit.Trefle),     // Bob 1
            new Card(CardRank.Huit, Suit.Carreau),    // Bob 2
            new Card(CardRank.As, Suit.Coeur),        // Flop 1 (paire d'As pour Alice)
            new Card(CardRank.Deux, Suit.Coeur),      // Flop 2
            new Card(CardRank.Trois, Suit.Trefle),    // Flop 3
            new Card(CardRank.Sept, Suit.Pique),      // Turn
            new Card(CardRank.Dame, Suit.Carreau)     // River
        };

        var deckFactory = () => new FakeDeck(deckCards);

        // 1er tour : Alice (premier Player) doit miser, Bob peut suivre.
        var actionsParPlayer = new Dictionary<string, Queue<GameAction>>
        {
            ["Alice"] = new Queue<GameAction>(new[]
            {
                new GameAction(TypeGameAction.Miser, 10), // 1er tour, 1er Player : mise obligatoire (>= 10 géré dans PokerGame)
                new GameAction(TypeGameAction.Check),
                new GameAction(TypeGameAction.Check),
                new GameAction(TypeGameAction.Check)
            }),
            ["Bob"] = new Queue<GameAction>(new[]
            {
                new GameAction(TypeGameAction.Suivre), // Il suit la mise d’Alice
                new GameAction(TypeGameAction.Check),
                new GameAction(TypeGameAction.Check),
                new GameAction(TypeGameAction.Check)
            })
        };

        PokerGameState? etatFinal = null;
        string? gagnant = null;

        var pokerGame = new PokerGame(
            new[] { human, autreHumain },
            deckFactory,
            context => actionsParPlayer[context.PlayerName].Dequeue(),
            () => false);

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
            new Card(CardRank.As, Suit.Pique),        // Alice 1
            new Card(CardRank.Roi, Suit.Coeur),       // Alice 2
            new Card(CardRank.Neuf, Suit.Trefle),     // Bob 1
            new Card(CardRank.Huit, Suit.Carreau),    // Bob 2
            new Card(CardRank.Deux, Suit.Coeur),      // Flop 1
            new Card(CardRank.Trois, Suit.Trefle),    // Flop 2
            new Card(CardRank.Quatre, Suit.Carreau),  // Flop 3
            new Card(CardRank.Cinq, Suit.Pique),      // Turn
            new Card(CardRank.Six, Suit.Coeur)        // River
        };

        var deckFactory = () => new FakeDeck(deckCards);

        // Idem : 1er tour, Alice doit miser, Bob suit.
        var actionsParPlayer = new Dictionary<string, Queue<GameAction>>
        {
            ["Alice"] = new Queue<GameAction>(new[]
            {
                new GameAction(TypeGameAction.Miser, 10),
                new GameAction(TypeGameAction.Check),
                new GameAction(TypeGameAction.Check),
                new GameAction(TypeGameAction.Check)
            }),
            ["Bob"] = new Queue<GameAction>(new[]
            {
                new GameAction(TypeGameAction.Suivre),
                new GameAction(TypeGameAction.Check),
                new GameAction(TypeGameAction.Check),
                new GameAction(TypeGameAction.Check)
            })
        };

        PokerGameState? etatFinal = null;
        string? gagnant = null;

        var pokerGame = new PokerGame(
            new[] { human, autreHumain },
            deckFactory,
            context => actionsParPlayer[context.PlayerName].Dequeue(),
            () => false);

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
}
