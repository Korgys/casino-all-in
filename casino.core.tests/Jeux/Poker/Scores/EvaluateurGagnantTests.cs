using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;
using casino.core.Jeux.Poker.Scores;
using casino.core.tests.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.tests.Jeux.Poker.Scores;

[TestClass]
public class EvaluateurGagnantTests
{
    [TestMethod]
    public void DeterminerGagnantParMain_QuandTousLesJoueursSeCouchent_DoitLeverArgumentException()
    {
        // Arrange
        var cartesCommunes = Communes(
            C(RangCarte.Deux, Couleur.Coeur),
            C(RangCarte.Trois, Couleur.Carreau),
            C(RangCarte.Quatre, Couleur.Pique),
            C(RangCarte.Cinq, Couleur.Trefle),
            C(RangCarte.Six, Couleur.Coeur)
        );

        var joueurs = new List<Joueur>
            {
                CreerJoueurHumain("J1", C(C(RangCarte.As, Couleur.Coeur), C(RangCarte.Roi, Couleur.Carreau)), TypeActionJeu.SeCoucher),
                CreerJoueurHumain("J2", C(C(RangCarte.Dame, Couleur.Coeur), C(RangCarte.Valet, Couleur.Carreau)), TypeActionJeu.SeCoucher)
            };

        // Act + Assert
        Assert.Throws<ArgumentException>(() => EvaluateurGagnant.DeterminerGagnantParMain(joueurs, cartesCommunes));
    }

    [TestMethod]
    public void DeterminerGagnantParMain_DoitIgnorerLesJoueursCouches()
    {
        // Arrange
        // Cartes communes neutres
        var cartesCommunes = Communes(
            C(RangCarte.Deux, Couleur.Coeur),
            C(RangCarte.Trois, Couleur.Carreau),
            C(RangCarte.Quatre, Couleur.Pique),
            C(RangCarte.Huit, Couleur.Trefle),
            C(RangCarte.Neuf, Couleur.Coeur)
        );

        // J1 (couché) aurait un gros jeu (As + Roi)
        var j1 = CreerJoueurHumain(
            "J1",
            C(C(RangCarte.As, Couleur.Coeur), C(RangCarte.Roi, Couleur.Carreau)),
            TypeActionJeu.SeCoucher);

        // J2 (en jeu) a un jeu plus faible
        var j2 = CreerJoueurHumain(
            "J2",
            C(C(RangCarte.Dame, Couleur.Coeur), C(RangCarte.Valet, Couleur.Carreau)),
            TypeActionJeu.Miser);

        var joueurs = new List<Joueur> { j1, j2 };

        // Act
        var gagnant = EvaluateurGagnant.DeterminerGagnantParMain(joueurs, cartesCommunes);

        // Assert
        Assert.AreSame(j2, gagnant, "Ignorer les joueurs couchés même s'ils auraient une meilleure main.");
    }

    [TestMethod]
    public void DeterminerGagnantParMain_DoitChoisirLaMainLaPlusForte_ParRang()
    {
        // Arrange
        // Cartes communes : 2-3-4-5-9 (permet une suite A-2-3-4-5 si le joueur a un As)
        var cartesCommunes = Communes(
            C(RangCarte.Deux, Couleur.Coeur),
            C(RangCarte.Trois, Couleur.Carreau),
            C(RangCarte.Quatre, Couleur.Pique),
            C(RangCarte.Cinq, Couleur.Trefle),
            C(RangCarte.Neuf, Couleur.Coeur)
        );

        // J1 : possède un As => fait une Suite (wheel) A-2-3-4-5
        var j1 = CreerJoueurHumain(
            "J1",
            C(C(RangCarte.As, Couleur.Pique), C(RangCarte.Dame, Couleur.Coeur)),
            TypeActionJeu.Miser);

        // J2 : pas d'As => souvent juste CarteHaute / Paire faible selon règles
        var j2 = CreerJoueurHumain(
            "J2",
            C(C(RangCarte.Roi, Couleur.Pique), C(RangCarte.Valet, Couleur.Carreau)),
            TypeActionJeu.Miser);

        var joueurs = new List<Joueur> { j1, j2 };

        // Act
        var gagnant = EvaluateurGagnant.DeterminerGagnantParMain(joueurs, cartesCommunes);

        // Assert
        Assert.AreSame(j1, gagnant, "Choisir le joueur avec la main la plus forte (RangMain le plus élevé).");
    }

    [TestMethod]
    public void DeterminerGagnantParMain_QuandMemeRang_DoitDepartagerParValeur()
    {
        // Arrange
        // Cartes communes : une Dame + des cartes basses
        var cartesCommunes = Communes(
            C(RangCarte.Dame, Couleur.Coeur),
            C(RangCarte.Deux, Couleur.Carreau),
            C(RangCarte.Cinq, Couleur.Pique),
            C(RangCarte.Huit, Couleur.Trefle),
            C(RangCarte.Neuf, Couleur.Coeur)
        );

        // J1 : Paire de Dames (Dame en main + Dame en communes) => Valeur paire = Dame
        var j1 = CreerJoueurHumain(
            "J1",
            C(C(RangCarte.Dame, Couleur.Pique), C(RangCarte.Trois, Couleur.Coeur)),
            TypeActionJeu.Miser);

        // J2 : Paire de 9 (9 en main + 9 en communes) => Valeur paire = 9
        var j2 = CreerJoueurHumain(
            "J2",
            C(C(RangCarte.Neuf, Couleur.Pique), C(RangCarte.Quatre, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var joueurs = new List<Joueur> { j1, j2 };

        // Act
        var gagnant = EvaluateurGagnant.DeterminerGagnantParMain(joueurs, cartesCommunes);

        // Assert
        Assert.AreSame(j1, gagnant, "À rang égal, choisir la meilleure valeur (paire de Dames > paire de 9).");
    }

    [TestMethod]
    public void DeterminerGagnantParMain_QuandMemeRangEtMemeValeur_DoitUtiliserLeTieBreakerSommeTop5()
    {
        // Arrange
        // Cartes communes : As + 7 + 6 + 5 + 2 (donne une paire d'As pour quiconque a un As en main)
        var cartesCommunes = Communes(
            C(RangCarte.As, Couleur.Pique),
            C(RangCarte.Sept, Couleur.Carreau),
            C(RangCarte.Six, Couleur.Pique),
            C(RangCarte.Cinq, Couleur.Trefle),
            C(RangCarte.Deux, Couleur.Coeur)
        );

        // Deux joueurs ont la même paire (As) donc Rang = Paire, Valeur = As.
        // On force le tie-breaker via les kickers : K > Q donc somme top5 plus élevée pour J1.
        var j1 = CreerJoueurHumain(
            "J1",
            C(C(RangCarte.As, Couleur.Coeur), C(RangCarte.Roi, Couleur.Carreau)),
            TypeActionJeu.Miser);

        var j2 = CreerJoueurHumain(
            "J2",
            C(C(RangCarte.As, Couleur.Carreau), C(RangCarte.Dame, Couleur.Coeur)),
            TypeActionJeu.Miser);

        var joueurs = new List<Joueur> { j1, j2 };

        // Act
        var gagnant = EvaluateurGagnant.DeterminerGagnantParMain(joueurs, cartesCommunes);

        // Assert
        Assert.AreSame(j1, gagnant, "À rang et valeur égaux, départager par la somme des 5 meilleurs rangs.");
    }

    private static Carte C(RangCarte rang, Couleur couleur)
        => new Carte(rang, couleur);

    private static CartesMain C(Carte a, Carte b)
        => new CartesMain(a, b);

    private static CartesCommunes Communes(Carte a, Carte b, Carte c, Carte d, Carte e)
        => new CartesCommunes { Flop1 = a, Flop2 = b, Flop3 = c, Turn = d, River = e };

    private static Joueur CreerJoueurHumain(string nom, CartesMain main, TypeActionJeu derniereAction)
    {
        var j = new JoueurHumain(nom, 1000);

        // Affecter la main.
        j.Main = main;

        // Petit hack pour définir la dernière action car cellle-ci est en internal set
        Partie partie = new Partie(new List<Joueur> { j }, new FakeDeck(new List<Carte>()));
        ActionJeu actionJeu = new ActionJeu(derniereAction, derniereAction == TypeActionJeu.Miser || derniereAction == TypeActionJeu.Relancer ? 10 : 0);
        partie.AppliquerAction(j, actionJeu);
        j = (JoueurHumain)partie.Joueurs.First();

        return j;
    }
}
