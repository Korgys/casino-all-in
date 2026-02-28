using System;
using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Actions.Commands;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;
using casino.core.tests.Fakes;
using casino.core.tests.Games.Poker.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace casino.core.tests.Games.Poker.Actions.Commands;

[TestClass]
public class CommandsTests
{
    private static IEnumerable<Card> CreerCartesSimples() => Enumerable.Repeat(new Card(RangCarte.Deux, Couleur.Coeur), 10);

    [TestMethod]
    public void CheckCommand_QuandPasDeMise_MetDerniereActionCheck()
    {
        var Player = new PlayerHumain("Alice", 100);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        var cmd = new CheckCommand(Player);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Check, Player.LastAction);
    }

    [TestMethod]
    public void CheckCommand_QuandMiseActuelleNonNulle_LanceInvalidOperationException()
    {
        var Player = new PlayerHumain("Alice", 100);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        // Forcer une mise sur la table
        PlayerTestHelper.DefinirMiseActuelle(partie, 10);

        var cmd = new CheckCommand(Player);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute(partie));
    }

    [TestMethod]
    public void MiserCommande_Valide_MetAJourPotEtMiseActuelleEtJetons()
    {
        var Player = new PlayerHumain("Bob", 100);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        var cmd = new BetCommand(Player, 25);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Miser, Player.LastAction);
        Assert.AreEqual(75, Player.Chips);
        Assert.AreEqual(25, partie.CurrentBet);
        Assert.AreEqual(25, partie.GetBetFor(Player));
        Assert.AreEqual(25, partie.Pot);
    }

    [TestMethod]
    public void MiserCommande_MontantZero_LanceArgumentException()
    {
        var Player = new PlayerHumain("Bob", 100);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        var cmd = new BetCommand(Player, 0);
        Assert.Throws<ArgumentException>(() => cmd.Execute(partie));
    }

    [TestMethod]
    public void RelancerCommande_Valide_MetAJourMiseEtPotEtJetons()
    {
        var Player = new PlayerHumain("Carol", 200);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        // Mise actuelle faible
        PlayerTestHelper.DefinirMiseActuelle(partie, 10);

        var cmd = new RaiseCommand(Player, 50);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Relancer, Player.LastAction);
        Assert.AreEqual(150, Player.Chips);
        Assert.AreEqual(50, partie.CurrentBet);
        Assert.AreEqual(50, partie.GetBetFor(Player));
        Assert.AreEqual(50, partie.Pot); // différence ajoutée = 50 (nouvelle) - 0 (contribution précédente) = 50
        Assert.AreEqual(50, partie.GetBetFor(Player));
    }

    [TestMethod]
    public void RelancerCommande_DifferenceEgaleAuxJetons_SeTransformeEnTapis()
    {
        var Player = new PlayerHumain("Dave", 20);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        // Aucune mise préalable : relancer à 20 provoque tapis (difference == jetons)
        var cmd = new RaiseCommand(Player, 20);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Tapis, Player.LastAction);
        Assert.AreEqual(0, Player.Chips);
        Assert.AreEqual(20, partie.GetBetFor(Player));
        Assert.IsGreaterThanOrEqualTo(20, partie.CurrentBet);
        Assert.IsGreaterThanOrEqualTo(20, partie.Pot);
    }

    [TestMethod]
    public void RelancerCommande_MontantInferieurOuEgalMiseActuelle_LanceArgumentException()
    {
        var Player = new PlayerHumain("Eve", 100);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        PlayerTestHelper.DefinirMiseActuelle(partie, 50);

        var cmd = new RaiseCommand(Player, 50);
        Assert.Throws<ArgumentException>(() => cmd.Execute(partie));
    }

    [TestMethod]
    public void SuivreCommande_Valide_ReduitJetonsEtAugmentePotEtDefinitMisePour()
    {
        var Player = new PlayerHumain("Frank", 100);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        // Définir une mise actuelle que le Player doit suivre
        PlayerTestHelper.DefinirMiseActuelle(partie, 40);
        // Le Player a déjà contribué 10
        partie.DefinirMisePour(Player, 10);

        var cmd = new CallCommand(Player);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Suivre, Player.LastAction);
        Assert.AreEqual(70, Player.Chips); // 100 - (40-10) = 70
        Assert.AreEqual(40, partie.GetBetFor(Player));
        Assert.IsGreaterThanOrEqualTo(30, partie.Pot);
    }

    [TestMethod]
    public void SuivreCommande_SansDifference_LanceInvalidOperationException()
    {
        var Player = new PlayerHumain("Gina", 100);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        // Pas de mise à rattraper
        PlayerTestHelper.DefinirMiseActuelle(partie, 0);
        partie.DefinirMisePour(Player, 0);

        var cmd = new CallCommand(Player);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute(partie));
    }

    [TestMethod]
    public void TapisCommande_MetJetonsAZero_AjouteAuPotEtDefinitMisePour()
    {
        var Player = new PlayerHumain("Hank", 30);
        var partie = new Partie(new List<Player> { Player }, new FakeDeck(CreerCartesSimples()));

        // Player a déjà contribué 5
        partie.DefinirMisePour(Player, 5);

        var cmd = new AllInCommand(Player);
        cmd.Execute(partie);

        Assert.AreEqual(TypeActionJeu.Tapis, Player.LastAction);
        Assert.AreEqual(0, Player.Chips);
        Assert.AreEqual(35, partie.GetBetFor(Player)); // 5 + 30
        Assert.IsGreaterThanOrEqualTo(35, partie.CurrentBet);
        Assert.IsGreaterThanOrEqualTo(30, partie.Pot);
    }
}