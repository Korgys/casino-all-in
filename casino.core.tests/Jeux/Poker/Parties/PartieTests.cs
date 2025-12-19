using System.Collections.Generic;
using System.Linq;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using casino.core.Jeux.Poker.Parties.Phases;
using casino.core.tests.Fakes;
using casino.core.tests.Jeux.Poker.Joueurs;

namespace casino.core.tests.Jeux.Poker.Parties;

[TestClass]
public class PartieTests
{
    [TestMethod]
    public void Partie_DoitDistribuerLesCartesUniquementAuxJoueursEligibles()
    {
        // Arrange
        var deck = new FakeDeck(new[]
        {
            new Carte(RangCarte.As, Couleur.Coeur),
            new Carte(RangCarte.Roi, Couleur.Carreau),
            new Carte(RangCarte.Dame, Couleur.Trèfle),
            new Carte(RangCarte.Valet, Couleur.Pique)
        });
        var joueurActif = new JoueurHumain("Alice", 100);
        var joueurSansJetons = new JoueurHumain("Bob", 0);

        // Act
        var partie = new Partie(new List<Joueur> { joueurActif, joueurSansJetons }, deck);

        // Assert
        Assert.IsNotNull(partie.Joueurs.First(j => j.Nom == "Alice").Main, "Les joueurs avec des jetons doivent recevoir des cartes.");
        Assert.AreEqual(new Carte(RangCarte.As, Couleur.Coeur), joueurActif.Main.Premiere, "La première carte doit provenir du deck fourni.");
        Assert.AreEqual(new Carte(RangCarte.Roi, Couleur.Carreau), joueurActif.Main.Seconde, "La seconde carte doit provenir du deck fourni.");
        Assert.IsNull(partie.Joueurs.First(j => j.Nom == "Bob").Main, "Les joueurs sans jetons ne doivent pas recevoir de main.");
    }

    [TestMethod]
    public void AvancerPhase_PreFlopVersFlop_DoitRevelerTroisCartesEtMettreAJourLEtat()
    {
        // Arrange
        var deck = new FakeDeck(new[]
        {
            new Carte(RangCarte.As, Couleur.Coeur),
            new Carte(RangCarte.Roi, Couleur.Coeur),
            new Carte(RangCarte.Dame, Couleur.Coeur),
            new Carte(RangCarte.Valet, Couleur.Coeur),
            new Carte(RangCarte.Dix, Couleur.Coeur)
        });
        var joueur = new JoueurHumain("Alice", 100);
        var partie = new Partie(new List<Joueur> { joueur }, deck);

        // Act
        partie.AvancerPhase();

        // Assert
        Assert.AreEqual(Phase.Flop, partie.Phase, "La phase doit passer au flop.");
        Assert.IsInstanceOfType<FlopPhaseState>(partie.PhaseState, "L'état doit être mis à jour pour le flop.");
        Assert.AreEqual(new Carte(RangCarte.Dame, Couleur.Coeur), partie.CartesCommunes.Flop1, "La première carte du flop doit provenir du deck.");
        Assert.AreEqual(new Carte(RangCarte.Valet, Couleur.Coeur), partie.CartesCommunes.Flop2, "La deuxième carte du flop doit provenir du deck.");
        Assert.AreEqual(new Carte(RangCarte.Dix, Couleur.Coeur), partie.CartesCommunes.Flop3, "La troisième carte du flop doit provenir du deck.");
    }

    [TestMethod]
    public void TerminerPartie_DernierJoueurActif_DoitGagnerParAbandon()
    {
        // Arrange
        var deck = new FakeDeck(Enumerable.Repeat(new Carte(RangCarte.Deux, Couleur.Coeur), 6));
        var joueurActif = new JoueurHumain("Alice", 100);
        var joueurCouche = new JoueurHumain("Bob", 50) { EstCouche = true };
        var partie = new Partie(new List<Joueur> { joueurActif, joueurCouche }, deck)
        {
            Pot = 50
        };

        // Act
        partie.TerminerPartie();

        // Assert
        Assert.AreEqual(joueurActif, partie.Gagnant, "Le seul joueur encore actif doit être déclaré gagnant.");
        Assert.AreEqual(150, joueurActif.Jetons, "Le pot doit être ajouté aux jetons du gagnant.");
        Assert.AreEqual(Phase.Showdown, partie.Phase, "La partie doit se terminer en passant au showdown.");
    }

    [TestMethod]
    public void TerminerPartie_DoitComparerLesMainsEtCrediterLePotAuMeilleurJoueur()
    {
        // Arrange
        var deck = new FakeDeck(Enumerable.Repeat(new Carte(RangCarte.Deux, Couleur.Pique), 10));
        var alice = new JoueurHumain("Alice", 100)
        {
            Main = new CartesMain(
                new Carte(RangCarte.As, Couleur.Coeur),
                new Carte(RangCarte.Roi, Couleur.Coeur))
        };
        var bob = new JoueurHumain("Bob", 100)
        {
            Main = new CartesMain(
                new Carte(RangCarte.As, Couleur.Pique),
                new Carte(RangCarte.As, Couleur.Carreau))
        };
        var partie = new Partie(new List<Joueur> { alice, bob }, deck)
        {
            CartesCommunes = JoueurTestHelper.CreerCartesCommunes(
                new Carte(RangCarte.Dame, Couleur.Coeur),
                new Carte(RangCarte.Dix, Couleur.Coeur),
                new Carte(RangCarte.Neuf, Couleur.Coeur),
                new Carte(RangCarte.Deux, Couleur.Carreau),
                new Carte(RangCarte.Trois, Couleur.Trèfle)),
            Pot = 60
        };

        // Act
        partie.TerminerPartie();

        // Assert
        Assert.AreEqual(alice, partie.Gagnant, "Le joueur avec la meilleure main doit être désigné gagnant.");
        Assert.AreEqual(160, alice.Jetons, "Le pot doit être ajouté aux jetons du gagnant.");
        Assert.AreEqual(100, bob.Jetons, "Les jetons des autres joueurs ne doivent pas changer.");
        Assert.AreEqual(Phase.Showdown, partie.Phase, "La partie doit être marquée comme terminée.");
    }
}
