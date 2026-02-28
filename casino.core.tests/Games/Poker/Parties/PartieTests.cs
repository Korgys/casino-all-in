using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;
using casino.core.Games.Poker.Parties.Phases;
using casino.core.tests.Fakes;
using casino.core.tests.Games.Poker.Players;

namespace casino.core.tests.Games.Poker.Parties;

[TestClass]
public class PartieTests
{
    [TestMethod]
    public void Partie_DoitDistribuerLesCartesUniquementAuxPlayersEligibles()
    {
        // Arrange
        var deck = new FakeDeck(new[]
        {
            new Card(RangCarte.As, Couleur.Coeur),
            new Card(RangCarte.Roi, Couleur.Carreau),
            new Card(RangCarte.Dame, Couleur.Trefle),
            new Card(RangCarte.Valet, Couleur.Pique)
        });
        var PlayerActif = new PlayerHumain("Alice", 100);
        var PlayerSansJetons = new PlayerHumain("Bob", 0);

        // Act
        var partie = new Partie(new List<Player> { PlayerActif, PlayerSansJetons }, deck);

        // Assert
        Assert.IsNotNull(partie.Players.First(j => j.Name == "Alice").Hand, "Les Players avec des jetons doivent recevoir des cartes.");
        Assert.AreEqual(new Card(RangCarte.As, Couleur.Coeur), PlayerActif.Hand.Premiere, "La première carte doit provenir du deck fourni.");
        Assert.AreEqual(new Card(RangCarte.Roi, Couleur.Carreau), PlayerActif.Hand.Seconde, "La seconde carte doit provenir du deck fourni.");
        Assert.IsNull(partie.Players.First(j => j.Name == "Bob").Hand, "Les Players sans jetons ne doivent pas recevoir de main.");
    }

    [TestMethod]
    public void AvancerPhase_PreFlopVersFlop_DoitRevelerTroisCartesEtMettreAJourLEtat()
    {
        // Arrange
        var deck = new FakeDeck(new[]
        {
            new Card(RangCarte.As, Couleur.Coeur),
            new Card(RangCarte.Roi, Couleur.Coeur),
            new Card(RangCarte.Dame, Couleur.Coeur),
            new Card(RangCarte.Valet, Couleur.Coeur),
            new Card(RangCarte.Dix, Couleur.Coeur)
        });
        var Player = new PlayerHumain("Alice", 100);
        var partie = new Partie(new List<Player> { Player }, deck);

        // Act
        partie.AvancerPhase();

        // Assert
        Assert.AreEqual(Phase.Flop, partie.Phase, "La phase doit passer au flop.");
        Assert.IsInstanceOfType<FlopPhaseState>(partie.PhaseState, "L'état doit être mis à jour pour le flop.");
        Assert.AreEqual(new Card(RangCarte.Dame, Couleur.Coeur), partie.CommunityCards.Flop1, "La première carte du flop doit provenir du deck.");
        Assert.AreEqual(new Card(RangCarte.Valet, Couleur.Coeur), partie.CommunityCards.Flop2, "La deuxième carte du flop doit provenir du deck.");
        Assert.AreEqual(new Card(RangCarte.Dix, Couleur.Coeur), partie.CommunityCards.Flop3, "La troisième carte du flop doit provenir du deck.");
    }

    [TestMethod]
    public void EndGame_DernierPlayerActif_DoitGagnerParAbandon()
    {
        // Arrange
        var deck = new FakeDeck(Enumerable.Repeat(new Card(RangCarte.Deux, Couleur.Coeur), 6));
        var PlayerActif = new PlayerHumain("Alice", 100);
        var PlayerCouche = new PlayerHumain("Bob", 50) { LastAction = TypeActionJeu.SeCoucher };
        var partie = new Partie(new List<Player> { PlayerActif, PlayerCouche }, deck)
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
        var deck = new FakeDeck(Enumerable.Repeat(new Card(RangCarte.Deux, Couleur.Pique), 10));
        var alice = new PlayerHumain("Alice", 100);
        var bob = new PlayerHumain("Bob", 100);
        var partie = new Partie(new List<Player> { alice, bob }, deck)
        {
            CommunityCards = PlayerTestHelper.CreerCartesCommunes(
                new Card(RangCarte.Dame, Couleur.Coeur),
                new Card(RangCarte.Dix, Couleur.Coeur),
                new Card(RangCarte.Neuf, Couleur.Coeur),
                new Card(RangCarte.Deux, Couleur.Carreau),
                new Card(RangCarte.Trois, Couleur.Trefle)),
            Pot = 60
        };

        // Important : assigner les mains après l'initialisation de la partie
        alice.Hand = new HandCards(
            new Card(RangCarte.As, Couleur.Coeur),
            new Card(RangCarte.Roi, Couleur.Coeur));
        bob.Hand = new HandCards(
            new Card(RangCarte.As, Couleur.Pique),
            new Card(RangCarte.As, Couleur.Carreau));

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
