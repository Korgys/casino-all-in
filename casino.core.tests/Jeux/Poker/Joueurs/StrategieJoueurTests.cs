using System.Collections.Generic;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Joueurs.Strategies;
using casino.core.Jeux.Poker.Scores;

namespace casino.core.tests.Jeux.Poker.Joueurs;

[TestClass]
public class StrategieRandomTests
{
    [TestMethod]
    public void ProposerAction_RelancerLorsqueMinimumDepasseJetons_DoitAllerTapis()
    {
        // Arrange
        var joueur = new JoueurHumain("Alice", 8)
        {
            Main = new CartesMain(
                new Carte(RangCarte.As, Couleur.Coeur),
                new Carte(RangCarte.Roi, Couleur.Pique))
        };
        var partie = JoueurTestHelper.CreerPartieAvecJoueur(joueur, joueur.Main);
        JoueurTestHelper.DefinirMiseActuelle(partie, 5);
        var contexte = new ContexteDeJeu(partie, joueur, new List<TypeActionJeu> { TypeActionJeu.Relancer });
        var strategie = new StrategieRandom();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Relancer, action.TypeAction, "La stratégie aléatoire doit respecter l'action proposée.");
        Assert.AreEqual(joueur.Jetons, action.Montant, "Si la relance minimale dépasse la mise disponible, l'action doit utiliser tous les jetons restants.");
    }

    [TestMethod]
    public void ProposerAction_Miser_DoiventUtiliserLaMiseMinimum()
    {
        // Arrange
        var joueur = new JoueurHumain("Bob", 100)
        {
            Main = new CartesMain(
                new Carte(RangCarte.Dame, Couleur.Carreau),
                new Carte(RangCarte.Neuf, Couleur.Trèfle))
        };
        var partie = JoueurTestHelper.CreerPartieAvecJoueur(joueur, joueur.Main, miseDeDepart: 25);
        var contexte = new ContexteDeJeu(partie, joueur, new List<TypeActionJeu> { TypeActionJeu.Miser });
        var strategie = new StrategieRandom();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Miser, action.TypeAction, "L'action doit correspondre au choix tiré.");
        Assert.AreEqual(contexte.MiseMinimum, action.Montant, "La mise doit correspondre au minimum requis.");
    }
}

[TestClass]
public class StrategieAgressiveTests
{
    [TestMethod]
    public void ProposerAction_DoitPrivilegierRelanceQuandPossible()
    {
        // Arrange
        var joueur = new JoueurOrdi("Bot", 50, new StrategieAgressive())
        {
            Main = new CartesMain(
                new Carte(RangCarte.As, Couleur.Coeur),
                new Carte(RangCarte.Roi, Couleur.Pique))
        };
        var partie = JoueurTestHelper.CreerPartieAvecJoueur(joueur, joueur.Main, miseDeDepart: 15);
        var contexte = new ContexteDeJeu(partie, joueur, new List<TypeActionJeu> { TypeActionJeu.Relancer, TypeActionJeu.Miser, TypeActionJeu.Suivre });
        var strategie = new StrategieAgressive();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Relancer, action.TypeAction, "La stratégie agressive doit relancer si possible.");
        Assert.AreEqual(contexte.MiseMinimum, action.Montant, "La relance doit être au moins égale à la mise minimale.");
    }

    [TestMethod]
    public void ProposerAction_SiRelanceImpossibleMaisMiserPossible_DoitMiser()
    {
        // Arrange
        var joueur = new JoueurOrdi("Bot", 10, new StrategieAgressive())
        {
            Main = new CartesMain(
                new Carte(RangCarte.Valet, Couleur.Carreau),
                new Carte(RangCarte.Dix, Couleur.Trèfle))
        };
        var partie = JoueurTestHelper.CreerPartieAvecJoueur(joueur, joueur.Main);
        JoueurTestHelper.DefinirMiseActuelle(partie, 20);
        var contexte = new ContexteDeJeu(partie, joueur, new List<TypeActionJeu> { TypeActionJeu.Miser, TypeActionJeu.Suivre });
        var strategie = new StrategieAgressive();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Miser, action.TypeAction, "La stratégie agressive doit miser si la relance n'est pas possible.");
        Assert.AreEqual(contexte.MiseMinimum, action.Montant, "La mise doit respecter le minimum requis.");
    }
}

[TestClass]
public class StrategieConservatriceTests
{
    [TestMethod]
    public void ProposerAction_PresenceDeCheck_DoitRetournerCheck()
    {
        // Arrange
        var joueur = new JoueurHumain("Alice", 100)
        {
            Main = new CartesMain(
                new Carte(RangCarte.As, Couleur.Coeur),
                new Carte(RangCarte.Dix, Couleur.Carreau))
        };
        var partie = JoueurTestHelper.CreerPartieAvecJoueur(joueur, joueur.Main);
        var contexte = new ContexteDeJeu(partie, joueur, new List<TypeActionJeu> { TypeActionJeu.Check, TypeActionJeu.Suivre });
        var strategie = new StrategieConservatrice();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Check, action.TypeAction, "Le check doit être prioritaire lorsqu'il est disponible.");
    }

    [TestMethod]
    public void ProposerAction_SuivreDisponible_DoitSuivreAvecMainCorrecte()
    {
        // Arrange
        var joueur = new JoueurHumain("Bob", 100)
        {
            Main = new CartesMain(
                new Carte(RangCarte.As, Couleur.Coeur),
                new Carte(RangCarte.As, Couleur.Carreau))
        };
        var communes = JoueurTestHelper.CreerCartesCommunes(
            new Carte(RangCarte.Roi, Couleur.Pique),
            new Carte(RangCarte.Roi, Couleur.Trèfle),
            new Carte(RangCarte.Dame, Couleur.Coeur));
        var partie = JoueurTestHelper.CreerPartieAvecJoueur(joueur, joueur.Main, communes);
        var contexte = new ContexteDeJeu(partie, joueur, new List<TypeActionJeu> { TypeActionJeu.Suivre, TypeActionJeu.Miser });
        var strategie = new StrategieConservatrice();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.Suivre, action.TypeAction, "La stratégie conservatrice doit suivre avec une main correcte quand aucune check n'est possible.");
    }

    [TestMethod]
    public void ProposerAction_SiAucuneAutreActionNeConvient_DoitSeCoucher()
    {
        // Arrange
        var joueur = new JoueurHumain("Claire", 100)
        {
            Main = new CartesMain(
                new Carte(RangCarte.Deux, Couleur.Carreau),
                new Carte(RangCarte.Sept, Couleur.Trèfle))
        };
        var communes = JoueurTestHelper.CreerCartesCommunes(
            new Carte(RangCarte.Dix, Couleur.Pique),
            new Carte(RangCarte.Neuf, Couleur.Trèfle),
            new Carte(RangCarte.Huit, Couleur.Coeur));
        var partie = JoueurTestHelper.CreerPartieAvecJoueur(joueur, joueur.Main, communes);
        var contexte = new ContexteDeJeu(partie, joueur, new List<TypeActionJeu> { TypeActionJeu.SeCoucher });
        var strategie = new StrategieConservatrice();

        // Act
        var action = strategie.ProposerAction(contexte);

        // Assert
        Assert.AreEqual(TypeActionJeu.SeCoucher, action.TypeAction, "La stratégie conservatrice doit abandonner en dernier recours.");
    }
}
