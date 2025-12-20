using System;
using System.Collections.Generic;
using System.Linq;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Actions.Commandes;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using casino.core.tests.Fakes;
using casino.core.tests.Jeux.Poker.Joueurs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace casino.core.tests.Jeux.Poker.Actions.Commandes;

[TestClass]
public class CommandesTests
{
    private static IEnumerable<Carte> CreerCartesSimples() => Enumerable.Repeat(new Carte(RangCarte.Deux, Couleur.Coeur), 10);

    [TestMethod]
    public void CheckCommande_QuandPasDeMise_MetDerniereActionCheck()
    {
        var joueur = new JoueurHumain("Alice", 100);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        var cmd = new CheckCommande(joueur);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Check, joueur.DerniereAction);
    }

    [TestMethod]
    public void CheckCommande_QuandMiseActuelleNonNulle_LanceInvalidOperationException()
    {
        var joueur = new JoueurHumain("Alice", 100);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        // Forcer une mise sur la table
        JoueurTestHelper.DefinirMiseActuelle(partie, 10);

        var cmd = new CheckCommande(joueur);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute(partie));
    }

    [TestMethod]
    public void MiserCommande_Valide_MetAJourPotEtMiseActuelleEtJetons()
    {
        var joueur = new JoueurHumain("Bob", 100);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        var cmd = new MiserCommande(joueur, 25);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Miser, joueur.DerniereAction);
        Assert.AreEqual(75, joueur.Jetons);
        Assert.AreEqual(25, partie.MiseActuelle);
        Assert.AreEqual(25, partie.ObtenirMisePour(joueur));
        Assert.AreEqual(25, partie.Pot);
    }

    [TestMethod]
    public void MiserCommande_MontantZero_LanceArgumentException()
    {
        var joueur = new JoueurHumain("Bob", 100);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        var cmd = new MiserCommande(joueur, 0);
        Assert.Throws<ArgumentException>(() => cmd.Execute(partie));
    }

    [TestMethod]
    public void RelancerCommande_Valide_MetAJourMiseEtPotEtJetons()
    {
        var joueur = new JoueurHumain("Carol", 200);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        // Mise actuelle faible
        JoueurTestHelper.DefinirMiseActuelle(partie, 10);

        var cmd = new RelancerCommande(joueur, 50);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Relancer, joueur.DerniereAction);
        Assert.AreEqual(150, joueur.Jetons);
        Assert.AreEqual(50, partie.MiseActuelle);
        Assert.AreEqual(50, partie.ObtenirMisePour(joueur));
        Assert.AreEqual(50, partie.Pot); // différence ajoutée = 50 (nouvelle) - 0 (contribution précédente) = 50
        Assert.AreEqual(50, partie.ObtenirMisePour(joueur));
    }

    [TestMethod]
    public void RelancerCommande_DifferenceEgaleAuxJetons_SeTransformeEnTapis()
    {
        var joueur = new JoueurHumain("Dave", 20);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        // Aucune mise préalable : relancer à 20 provoque tapis (difference == jetons)
        var cmd = new RelancerCommande(joueur, 20);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Tapis, joueur.DerniereAction);
        Assert.AreEqual(0, joueur.Jetons);
        Assert.AreEqual(20, partie.ObtenirMisePour(joueur));
        Assert.IsGreaterThanOrEqualTo(20, partie.MiseActuelle);
        Assert.IsGreaterThanOrEqualTo(20, partie.Pot);
    }

    [TestMethod]
    public void RelancerCommande_MontantInferieurOuEgalMiseActuelle_LanceArgumentException()
    {
        var joueur = new JoueurHumain("Eve", 100);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        JoueurTestHelper.DefinirMiseActuelle(partie, 50);

        var cmd = new RelancerCommande(joueur, 50);
        Assert.Throws<ArgumentException>(() => cmd.Execute(partie));
    }

    [TestMethod]
    public void SuivreCommande_Valide_ReduitJetonsEtAugmentePotEtDefinitMisePour()
    {
        var joueur = new JoueurHumain("Frank", 100);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        // Définir une mise actuelle que le joueur doit suivre
        JoueurTestHelper.DefinirMiseActuelle(partie, 40);
        // Le joueur a déjà contribué 10
        partie.DefinirMisePour(joueur, 10);

        var cmd = new SuivreCommande(joueur);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Suivre, joueur.DerniereAction);
        Assert.AreEqual(70, joueur.Jetons); // 100 - (40-10) = 70
        Assert.AreEqual(40, partie.ObtenirMisePour(joueur));
        Assert.IsGreaterThanOrEqualTo(30, partie.Pot);
    }

    [TestMethod]
    public void SuivreCommande_SansDifference_LanceInvalidOperationException()
    {
        var joueur = new JoueurHumain("Gina", 100);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        // Pas de mise à rattraper
        JoueurTestHelper.DefinirMiseActuelle(partie, 0);
        partie.DefinirMisePour(joueur, 0);

        var cmd = new SuivreCommande(joueur);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute(partie));
    }

    [TestMethod]
    public void TapisCommande_MetJetonsAZero_AjouteAuPotEtDefinitMisePour()
    {
        var joueur = new JoueurHumain("Hank", 30);
        var partie = new Partie(new List<Joueur> { joueur }, new FakeDeck(CreerCartesSimples()));

        // joueur a déjà contribué 5
        partie.DefinirMisePour(joueur, 5);

        var cmd = new TapisCommande(joueur);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Tapis, joueur.DerniereAction);
        Assert.AreEqual(0, joueur.Jetons);
        Assert.AreEqual(35, partie.ObtenirMisePour(joueur)); // 5 + 30
        Assert.IsGreaterThanOrEqualTo(35, partie.MiseActuelle);
        Assert.IsGreaterThanOrEqualTo(30, partie.Pot);
    }
}