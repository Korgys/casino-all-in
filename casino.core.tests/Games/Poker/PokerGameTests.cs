using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace casino.core.tests.Games.Poker;

[TestClass]
public class PokerGameTests
{
    [TestMethod]
    public void Run_SimplePartieDeuxPlayersUnGagnant()
    {
        // Arrange
        var human = new PlayerHumain("Alice", 100);
        var autreHumain = new PlayerHumain("Bob", 100);

        var deckCards = new[]
        {
            new Card(RangCarte.As, Couleur.Pique),        // Alice 1
            new Card(RangCarte.Roi, Couleur.Coeur),       // Alice 2
            new Card(RangCarte.Neuf, Couleur.Trefle),     // Bob 1
            new Card(RangCarte.Huit, Couleur.Carreau),    // Bob 2
            new Card(RangCarte.As, Couleur.Coeur),        // Flop 1 (paire d'As pour Alice)
            new Card(RangCarte.Deux, Couleur.Coeur),      // Flop 2
            new Card(RangCarte.Trois, Couleur.Trefle),    // Flop 3
            new Card(RangCarte.Sept, Couleur.Pique),      // Turn
            new Card(RangCarte.Dame, Couleur.Carreau)     // River
        };

        var deckFactory = () => new FakeDeck(deckCards);

        // 1er tour : Alice (premier Player) doit miser, Bob peut suivre.
        var actionsParPlayer = new Dictionary<string, Queue<ActionJeu>>
        {
            ["Alice"] = new Queue<ActionJeu>(new[]
            {
                new ActionJeu(TypeActionJeu.Miser, 10), // 1er tour, 1er Player : mise obligatoire (>= 10 géré dans PokerGame)
                new ActionJeu(TypeActionJeu.Check),
                new ActionJeu(TypeActionJeu.Check),
                new ActionJeu(TypeActionJeu.Check)
            }),
            ["Bob"] = new Queue<ActionJeu>(new[]
            {
                new ActionJeu(TypeActionJeu.Suivre), // Il suit la mise d’Alice
                new ActionJeu(TypeActionJeu.Check),
                new ActionJeu(TypeActionJeu.Check),
                new ActionJeu(TypeActionJeu.Check)
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
    public void Run_SimplePartieDeuxPlayersDeuxGagnants()
    {
        // Arrange
        var human = new PlayerHumain("Alice", 100);
        var autreHumain = new PlayerHumain("Bob", 100);
        var deckCards = new[]
        {
            new Card(RangCarte.As, Couleur.Pique),        // Alice 1
            new Card(RangCarte.Roi, Couleur.Coeur),       // Alice 2
            new Card(RangCarte.Neuf, Couleur.Trefle),     // Bob 1
            new Card(RangCarte.Huit, Couleur.Carreau),    // Bob 2
            new Card(RangCarte.Deux, Couleur.Coeur),      // Flop 1
            new Card(RangCarte.Trois, Couleur.Trefle),    // Flop 2
            new Card(RangCarte.Quatre, Couleur.Carreau),  // Flop 3
            new Card(RangCarte.Cinq, Couleur.Pique),      // Turn
            new Card(RangCarte.Six, Couleur.Coeur)        // River
        };

        var deckFactory = () => new FakeDeck(deckCards);

        // Idem : 1er tour, Alice doit miser, Bob suit.
        var actionsParPlayer = new Dictionary<string, Queue<ActionJeu>>
        {
            ["Alice"] = new Queue<ActionJeu>(new[]
            {
                new ActionJeu(TypeActionJeu.Miser, 10),
                new ActionJeu(TypeActionJeu.Check),
                new ActionJeu(TypeActionJeu.Check),
                new ActionJeu(TypeActionJeu.Check)
            }),
            ["Bob"] = new Queue<ActionJeu>(new[]
            {
                new ActionJeu(TypeActionJeu.Suivre),
                new ActionJeu(TypeActionJeu.Check),
                new ActionJeu(TypeActionJeu.Check),
                new ActionJeu(TypeActionJeu.Check)
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

    [TestMethod]
    public void Run_ContinueTantQueJetonsEtDemandePourRelancer()
    {
        // Arrange
        var human = new PlayerHumain("Alice", 100);
        var autreHumain = new PlayerHumain("Bob", 100);
        var deckCards = new[]
        {
            new Card(RangCarte.As, Couleur.Pique),
            new Card(RangCarte.Roi, Couleur.Coeur),
            new Card(RangCarte.Neuf, Couleur.Trefle),
            new Card(RangCarte.Huit, Couleur.Carreau),
            new Card(RangCarte.Deux, Couleur.Coeur),
            new Card(RangCarte.Trois, Couleur.Trefle),
            new Card(RangCarte.Quatre, Couleur.Carreau),
            new Card(RangCarte.Cinq, Couleur.Pique),
            new Card(RangCarte.Six, Couleur.Coeur)
        };

        var deckFactory = () => new FakeDeck(deckCards);

        var actionsParPlayer = new Dictionary<string, Queue<ActionJeu>>
        {
            ["Alice"] = new Queue<ActionJeu>(new[]
            {
                new ActionJeu(TypeActionJeu.Miser, 10),
                new ActionJeu(TypeActionJeu.Miser, 10),
                new ActionJeu(TypeActionJeu.Miser, 10),
                new ActionJeu(TypeActionJeu.Miser, 10),
                new ActionJeu(TypeActionJeu.Suivre),
                new ActionJeu(TypeActionJeu.Suivre),
                new ActionJeu(TypeActionJeu.Suivre),
                new ActionJeu(TypeActionJeu.Suivre)
            }),
            ["Bob"] = new Queue<ActionJeu>(new[]
            {
                new ActionJeu(TypeActionJeu.Suivre),
                new ActionJeu(TypeActionJeu.Suivre),
                new ActionJeu(TypeActionJeu.Suivre),
                new ActionJeu(TypeActionJeu.Suivre),
                new ActionJeu(TypeActionJeu.Miser, 10),
                new ActionJeu(TypeActionJeu.Miser, 10),
                new ActionJeu(TypeActionJeu.Miser, 10),
                new ActionJeu(TypeActionJeu.Miser, 10)
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
            });

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
