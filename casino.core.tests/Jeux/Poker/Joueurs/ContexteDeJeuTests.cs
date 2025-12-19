using System.Collections.Generic;
using System.Linq;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Scores;

namespace casino.core.tests.Jeux.Poker.Joueurs;

[TestClass]
public class ContexteDeJeuTests
{
    [TestMethod]
    public void Constructeur_DoitCalculerScoreEtConserverActions()
    {
        // Arrange
        var joueur = new JoueurHumain("Alice", 100)
        {
            Main = new CartesMain(
                new Carte(RangCarte.As, Couleur.Coeur),
                new Carte(RangCarte.Roi, Couleur.Carreau))
        };
        var communes = JoueurTestHelper.CreerCartesCommunes(
            new Carte(RangCarte.As, Couleur.Pique),
            new Carte(RangCarte.Dame, Couleur.Trefle),
            new Carte(RangCarte.Neuf, Couleur.Carreau));
        var actions = new List<TypeActionJeu> { TypeActionJeu.Check, TypeActionJeu.Miser };

        var partie = JoueurTestHelper.CreerPartieAvecJoueur(joueur, joueur.Main, communes);

        // Act
        var contexte = new ContexteDeJeu(partie, joueur, actions);

        // Assert
        Assert.AreEqual(RangMain.Paire, contexte.ScoreJoueur.Rang, "Le score doit détecter la paire d'As.");
        Assert.AreEqual(RangCarte.As, contexte.ScoreJoueur.Valeur, "La valeur de la main doit correspondre à l'As.");
        CollectionAssert.AreEquivalent(actions, contexte.ActionsPossibles.ToList(), "Les actions possibles doivent être conservées.");
        Assert.AreEqual(partie.MiseDeDepart, contexte.MiseMinimum, "La mise minimale doit être dérivée de la partie.");
    }

    [TestMethod]
    public void MiseMinimum_DoitTenirCompteDeLaMiseActuelleLaPlusHaute()
    {
        // Arrange
        var joueur = new JoueurHumain("Bob", 100)
        {
            Main = new CartesMain(
                new Carte(RangCarte.Dix, Couleur.Coeur),
                new Carte(RangCarte.Neuf, Couleur.Carreau))
        };
        var partie = JoueurTestHelper.CreerPartieAvecJoueur(joueur, joueur.Main);
        JoueurTestHelper.DefinirMiseActuelle(partie, 45);

        // Act
        var contexte = new ContexteDeJeu(partie, joueur, new List<TypeActionJeu> { TypeActionJeu.Miser });

        // Assert
        Assert.AreEqual(45, contexte.MiseMinimum, "La mise minimale doit correspondre à la mise actuelle lorsqu'elle est supérieure à la mise de départ.");
    }
}
