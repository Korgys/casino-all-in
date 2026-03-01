using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.tests.Fakes;
using casino.core.tests.Games.Poker.Players;

namespace casino.core.tests.Games.Poker.Rounds;

[TestClass]
public class RoundTests
{
    [TestMethod]
    public void Round_DoitDistribuerLesCartesUniquementAuxPlayersEligibles()
    {
        // Arrange
        var deck = new FakeDeck(new[]
        {
            new Card(CardRank.As, Suit.Coeur),
            new Card(CardRank.Roi, Suit.Carreau),
            new Card(CardRank.Dame, Suit.Trefle),
            new Card(CardRank.Valet, Suit.Pique)
        });
        var PlayerActif = new HumanPlayer("Alice", 100);
        var PlayerSansJetons = new HumanPlayer("Bob", 0);

        // Act
        var partie = new Round(new List<Player> { PlayerActif, PlayerSansJetons }, deck);

        // Assert
        Assert.IsNotNull(partie.Players.First(j => j.Name == "Alice").Hand, "Les Players avec des jetons doivent recevoir des cartes.");
        Assert.AreEqual(new Card(CardRank.As, Suit.Coeur), PlayerActif.Hand.First, "La première carte doit provenir du deck fourni.");
        Assert.AreEqual(new Card(CardRank.Roi, Suit.Carreau), PlayerActif.Hand.Second, "La seconde carte doit provenir du deck fourni.");
        Assert.IsNull(partie.Players.First(j => j.Name == "Bob").Hand, "Les Players sans jetons ne doivent pas recevoir de main.");
    }

    [TestMethod]
    public void AdvancePhase_PreFlopVersFlop_DoitRevelerTroisCartesEtMettreAJourLEtat()
    {
        // Arrange
        var deck = new FakeDeck(new[]
        {
            new Card(CardRank.As, Suit.Coeur),
            new Card(CardRank.Roi, Suit.Coeur),
            new Card(CardRank.Dame, Suit.Coeur),
            new Card(CardRank.Valet, Suit.Coeur),
            new Card(CardRank.Dix, Suit.Coeur)
        });
        var Player = new HumanPlayer("Alice", 100);
        var partie = new Round(new List<Player> { Player }, deck);

        // Act
        partie.AdvancePhase();

        // Assert
        Assert.AreEqual(Phase.Flop, partie.Phase, "La phase doit passer au flop.");
        Assert.IsInstanceOfType<FlopPhaseState>(partie.PhaseState, "L'état doit être mis à jour pour le flop.");
        Assert.AreEqual(new Card(CardRank.Dame, Suit.Coeur), partie.CommunityCards.Flop1, "La première carte du flop doit provenir du deck.");
        Assert.AreEqual(new Card(CardRank.Valet, Suit.Coeur), partie.CommunityCards.Flop2, "La deuxième carte du flop doit provenir du deck.");
        Assert.AreEqual(new Card(CardRank.Dix, Suit.Coeur), partie.CommunityCards.Flop3, "La troisième carte du flop doit provenir du deck.");
    }

    [TestMethod]
    public void EndGame_DernierPlayerActif_DoitGagnerParAbandon()
    {
        // Arrange
        var deck = new FakeDeck(Enumerable.Repeat(new Card(CardRank.Deux, Suit.Coeur), 6));
        var PlayerActif = new HumanPlayer("Alice", 100);
        var PlayerCouche = new HumanPlayer("Bob", 50) { LastAction = TypeGameAction.SeCoucher };
        var partie = new Round(new List<Player> { PlayerActif, PlayerCouche }, deck)
        {
            Pot = 50
        };

        // Act
        partie.EndGame();

        // Assert
        Assert.HasCount(1, partie.Winners, "Il doit y avoir un seul gagnant.");
        Assert.AreEqual(PlayerActif, partie.Winners.First(), "Le seul Player encore actif doit être déclaré gagnant.");
        Assert.AreEqual(150, PlayerActif.Chips, "Le pot doit être ajouté aux jetons du gagnant.");
        Assert.AreEqual(Phase.Showdown, partie.Phase, "La partie doit se terminer en passant au showdown.");
    }

    [TestMethod]
    public void EndGame_DoitComparerLesMainsEtCrediterLePotAuMeilleurPlayer()
    {
        // Arrange
        var deck = new FakeDeck(Enumerable.Repeat(new Card(CardRank.Deux, Suit.Pique), 10));
        var alice = new HumanPlayer("Alice", 100);
        var bob = new HumanPlayer("Bob", 100);
        var partie = new Round(new List<Player> { alice, bob }, deck)
        {
            CommunityCards = PlayerTestHelper.CreateCommunityCards(
                new Card(CardRank.Dame, Suit.Coeur),
                new Card(CardRank.Dix, Suit.Coeur),
                new Card(CardRank.Neuf, Suit.Coeur),
                new Card(CardRank.Deux, Suit.Carreau),
                new Card(CardRank.Trois, Suit.Trefle)),
            Pot = 60
        };

        // Important : assigner les mains après l'initialisation de la partie
        alice.Hand = new HandCards(
            new Card(CardRank.As, Suit.Coeur),
            new Card(CardRank.Roi, Suit.Coeur));
        bob.Hand = new HandCards(
            new Card(CardRank.As, Suit.Pique),
            new Card(CardRank.As, Suit.Carreau));

        // Act
        partie.EndGame();

        // Assert
        Assert.HasCount(1, partie.Winners, "Il doit y avoir un seul gagnant.");
        Assert.AreEqual(alice, partie.Winners.First(), "Le Player avec la meilleure main doit être désigné gagnant.");
        Assert.AreEqual(160, alice.Chips, "Le pot doit être ajouté aux jetons du gagnant.");
        Assert.AreEqual(100, bob.Chips, "Les jetons des autres Players ne doivent pas changer.");
        Assert.AreEqual(Phase.Showdown, partie.Phase, "La partie doit être marquée comme terminée.");
    }
}
