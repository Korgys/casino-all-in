using System.Collections.Generic;
using System.Linq;
using casino.core.Jeux.Poker;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using casino.core.Jeux.Poker.Parties.Phases;
using casino.core.tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace casino.core.tests.Jeux.Poker.Parties;

[TestClass]
public class TablePokerTests
{
    private static IEnumerable<Carte> CreerCartesParDefaut() => new[]
    {
        new Carte(RangCarte.As, Couleur.Pique),
        new Carte(RangCarte.Roi, Couleur.Coeur),
        new Carte(RangCarte.Dame, Couleur.Trefle),
        new Carte(RangCarte.Valet, Couleur.Carreau),
        new Carte(RangCarte.Neuf, Couleur.Pique),
        new Carte(RangCarte.Huit, Couleur.Carreau),
        new Carte(RangCarte.Sept, Couleur.Pique),
        new Carte(RangCarte.Six, Couleur.Coeur),
        new Carte(RangCarte.Cinq, Couleur.Trefle)
    };

    [TestMethod]
    public void DemarrerPartie_ReinitialiseEtRotationDuJoueurInitial()
    {
        // Arrange
        var joueurs = new List<Joueur>
        {
            new JoueurHumain("Alice", 100) { DerniereAction = TypeActionJeu.Tapis, EstCouche = true, EstTapis = true },
            new JoueurHumain("Bob", 0) { DerniereAction = TypeActionJeu.Miser, EstCouche = false, EstTapis = false }
        };
        var table = new TablePoker();

        // Act - première partie
        table.DemarrerPartie(joueurs, new FakeDeck(CreerCartesParDefaut()));

        // Assert
        Assert.AreEqual(0, table.JoueurInitialIndex);
        Assert.AreEqual(table.JoueurInitialIndex, table.JoueurActuelIndex);
        Assert.IsTrue(table.Joueurs.All(j => j.DerniereAction == (j.Jetons > 0 ? TypeActionJeu.Aucune : TypeActionJeu.SeCoucher)));
        Assert.IsTrue(table.Joueurs.All(j => !j.EstCouche || j.Jetons == 0));

        // Act - deuxième partie pour vérifier la rotation
        table.DemarrerPartie(joueurs, new FakeDeck(CreerCartesParDefaut()));

        // Assert
        Assert.AreEqual(1, table.JoueurInitialIndex);
        Assert.AreEqual(table.JoueurInitialIndex, table.JoueurActuelIndex);
    }

    [TestMethod]
    public void TraiterActionJoueur_ChecksAvancentLaPhaseEtRemettentAuJoueurInitial()
    {
        // Arrange
        var joueurs = new List<Joueur>
        {
            new JoueurHumain("Alice", 100),
            new JoueurHumain("Bob", 100)
        };
        var table = new TablePoker();
        table.DemarrerPartie(joueurs, new FakeDeck(CreerCartesParDefaut()));

        // Act
        table.TraiterActionJoueur(table.Joueurs[table.JoueurActuelIndex], new ActionJeu(TypeActionJeu.Check));
        table.TraiterActionJoueur(table.Joueurs[table.JoueurActuelIndex], new ActionJeu(TypeActionJeu.Check));

        // Assert
        Assert.AreEqual(Phase.Flop, table.Partie.Phase);
        Assert.AreEqual(table.JoueurInitialIndex, table.JoueurActuelIndex);
    }

    [TestMethod]
    public void TraiterActionJoueur_CoucherMetFinALaPartieQuandUnJoueurReste()
    {
        // Arrange
        var joueurs = new List<Joueur>
        {
            new JoueurHumain("Alice", 100),
            new JoueurHumain("Bob", 100)
        };
        var table = new TablePoker();
        table.DemarrerPartie(joueurs, new FakeDeck(CreerCartesParDefaut()));

        // Act
        table.TraiterActionJoueur(table.Joueurs[table.JoueurActuelIndex], new ActionJeu(TypeActionJeu.SeCoucher));

        // Assert
        Assert.AreEqual(Phase.Showdown, table.Partie.Phase);
        Assert.HasCount(1, table.Partie.Gagnants);
        Assert.AreEqual("Bob", table.Partie.Gagnants.First().Nom);
    }
}
