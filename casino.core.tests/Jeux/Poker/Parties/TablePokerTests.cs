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
            new JoueurHumain("Alice", 100) { DerniereAction = TypeActionJeu.Tapis },
            new JoueurHumain("Bob", 0) { DerniereAction = TypeActionJeu.Miser }
        };
        var table = new TablePoker();

        // Act - première partie
        table.DemarrerPartie(joueurs, new FakeDeck(CreerCartesParDefaut()));

        // Assert
        Assert.AreEqual(0, table.JoueurInitialIndex);
        Assert.AreEqual(table.JoueurInitialIndex, table.JoueurActuelIndex);
        Assert.IsTrue(table.Joueurs.All(j => j.DerniereAction == (j.Jetons > 0 ? TypeActionJeu.Aucune : TypeActionJeu.SeCoucher)));
        Assert.IsTrue(table.Joueurs.All(j => !j.EstCouche() || j.Jetons == 0));

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

    [TestMethod]
    public void RelanceTardive_ForceUnNouveauTourDepuisLeJoueurInitial()
    {
        // Arrange
        var joueurs = new List<Joueur>
        {
            new JoueurHumain("Alice", 100),
            new JoueurHumain("Bob", 100),
            new JoueurHumain("Charlie", 100)
        };
        var table = new TablePoker();
        table.DemarrerPartie(joueurs, new FakeDeck(CreerCartesParDefaut()));

        // Alice check, Bob check
        table.TraiterActionJoueur(table.ObtenirJoueurQuiDoitJouer(), new ActionJeu(TypeActionJeu.Check));
        table.TraiterActionJoueur(table.ObtenirJoueurQuiDoitJouer(), new ActionJeu(TypeActionJeu.Check));

        // Charlie mise tardivement
        table.TraiterActionJoueur(table.ObtenirJoueurQuiDoitJouer(), new ActionJeu(TypeActionJeu.Miser, 20));

        // La phase ne doit pas avancer sans que les premiers joueurs rejouent
        Assert.AreEqual(Phase.PreFlop, table.Partie.Phase);
        Assert.AreEqual(table.JoueurInitialIndex, table.JoueurActuelIndex);
        Assert.AreEqual(20, table.Partie.MiseActuelle);

        // Alice et Bob doivent suivre avant d'avancer
        table.TraiterActionJoueur(table.ObtenirJoueurQuiDoitJouer(), new ActionJeu(TypeActionJeu.Suivre));
        table.TraiterActionJoueur(table.ObtenirJoueurQuiDoitJouer(), new ActionJeu(TypeActionJeu.Suivre));

        Assert.AreEqual(Phase.Flop, table.Partie.Phase);
        Assert.AreEqual(table.JoueurInitialIndex, table.JoueurActuelIndex);
        Assert.AreEqual(0, table.Partie.MiseActuelle);
        Assert.IsTrue(table.Joueurs.Where(j => !j.EstCouche() && !j.EstTapis()).All(j => j.DerniereAction == TypeActionJeu.Aucune));
    }
}
