using System.Collections.Generic;
using System.Linq;
using casino.core.Jeux.Poker;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace casino.core.tests.Jeux.Poker;

[TestClass]
public class PokerGameTests
{
    [TestMethod]
    public void Run_SimplePartieDeuxJoueursUnGagnant()
    {
        // Arrange
        var human = new JoueurHumain("Alice", 100);
        var autreHumain = new JoueurHumain("Bob", 100);

        var deckCards = new[]
        {
        new Carte(RangCarte.As, Couleur.Pique),        // Alice 1
        new Carte(RangCarte.Roi, Couleur.Coeur),       // Alice 2
        new Carte(RangCarte.Neuf, Couleur.Trefle),     // Bob 1
        new Carte(RangCarte.Huit, Couleur.Carreau),    // Bob 2
        new Carte(RangCarte.As, Couleur.Coeur),        // Flop 1 (paire d'As pour Alice)
        new Carte(RangCarte.Deux, Couleur.Coeur),      // Flop 2
        new Carte(RangCarte.Trois, Couleur.Trefle),    // Flop 3
        new Carte(RangCarte.Sept, Couleur.Pique),      // Turn
        new Carte(RangCarte.Dame, Couleur.Carreau)     // River
    };

        var deckFactory = () => new FakeDeck(deckCards);

        var actionsParJoueur = new Dictionary<string, Queue<ActionJeu>>
        {
            ["Alice"] = new Queue<ActionJeu>(Enumerable.Repeat(new ActionJeu(TypeActionJeu.Check), 4)),
            ["Bob"] = new Queue<ActionJeu>(Enumerable.Repeat(new ActionJeu(TypeActionJeu.Check), 4))
        };

        PokerGameState? etatFinal = null;
        string? gagnant = null;

        var pokerGame = new PokerGame(
            new[] { human, autreHumain },
            deckFactory,
            context => actionsParJoueur[context.JoueurNom].Dequeue(),
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

        Assert.IsTrue(etatFinal.Joueurs.Single(j => j.Nom == "Alice").EstGagnant);
        Assert.IsFalse(etatFinal.Joueurs.Single(j => j.Nom == "Bob").EstGagnant);
    }


    [TestMethod]
    public void Run_SimplePartieDeuxJoueursDeuxGagnants()
    {
        // Arrange
        var human = new JoueurHumain("Alice", 100);
        var autreHumain = new JoueurHumain("Bob", 100);
        var deckCards = new[]
        {
            new Carte(RangCarte.As, Couleur.Pique),        // Alice 1
            new Carte(RangCarte.Roi, Couleur.Coeur),       // Alice 2
            new Carte(RangCarte.Neuf, Couleur.Trefle),     // Bob 1
            new Carte(RangCarte.Huit, Couleur.Carreau),    // Bob 2
            new Carte(RangCarte.Deux, Couleur.Coeur),      // Flop 1
            new Carte(RangCarte.Trois, Couleur.Trefle),    // Flop 2
            new Carte(RangCarte.Quatre, Couleur.Carreau),  // Flop 3
            new Carte(RangCarte.Cinq, Couleur.Pique),      // Turn
            new Carte(RangCarte.Six, Couleur.Coeur)        // River
        };

        var deckFactory = () => new FakeDeck(deckCards);

        var actionsParJoueur = new Dictionary<string, Queue<ActionJeu>>
        {
            ["Alice"] = new Queue<ActionJeu>(Enumerable.Repeat(new ActionJeu(TypeActionJeu.Check), 4)),
            ["Bob"] = new Queue<ActionJeu>(Enumerable.Repeat(new ActionJeu(TypeActionJeu.Check), 4))
        };

        PokerGameState? etatFinal = null;
        string? gagnant = null;

        var pokerGame = new PokerGame(
            new[] { human, autreHumain },
            deckFactory,
            context => actionsParJoueur[context.JoueurNom].Dequeue(),
            () => false);

        pokerGame.StateUpdated += (_, args) => etatFinal = args.State as PokerGameState;
        pokerGame.GameEnded += (_, args) => gagnant = args.WinnerName;

        // Act
        pokerGame.Run();

        // Assert
        Assert.IsNotNull(etatFinal);
        Assert.AreEqual("Showdown", etatFinal.Phase);
        Assert.AreEqual("Alice, Bob", gagnant);
        Assert.IsTrue(etatFinal.Joueurs.All(j => j.EstGagnant));
    }

    [TestMethod]
    public void Run_ContinueTantQueJetonsEtDemandePourRelancer()
    {
        // Arrange
        var human = new JoueurHumain("Alice", 100);
        var autreHumain = new JoueurHumain("Bob", 100);
        var deckCards = new[]
        {
            new Carte(RangCarte.As, Couleur.Pique),
            new Carte(RangCarte.Roi, Couleur.Coeur),
            new Carte(RangCarte.Neuf, Couleur.Trefle),
            new Carte(RangCarte.Huit, Couleur.Carreau),
            new Carte(RangCarte.Deux, Couleur.Coeur),
            new Carte(RangCarte.Trois, Couleur.Trefle),
            new Carte(RangCarte.Quatre, Couleur.Carreau),
            new Carte(RangCarte.Cinq, Couleur.Pique),
            new Carte(RangCarte.Six, Couleur.Coeur)
        };

        var deckFactory = () => new FakeDeck(deckCards);

        var actionsParJoueur = new Dictionary<string, Queue<ActionJeu>>
        {
            ["Alice"] = new Queue<ActionJeu>(Enumerable.Repeat(new ActionJeu(TypeActionJeu.Check), 10)),
            ["Bob"] = new Queue<ActionJeu>(Enumerable.Repeat(new ActionJeu(TypeActionJeu.Check), 10))
        };

        var continuer = new Queue<bool>(new[] { true, false });
        var continueCalled = 0;
        var pokerGame = new PokerGame(
            new[] { human, autreHumain },
            deckFactory,
            context => actionsParJoueur[context.JoueurNom].Dequeue(),
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

        Assert.IsTrue(indexesGameEnded.All(idx => events.Skip(idx + 1).Any(e => e == "state")), "StateUpdated doit être déclenché après chaque fin de partie.");
    }
}
