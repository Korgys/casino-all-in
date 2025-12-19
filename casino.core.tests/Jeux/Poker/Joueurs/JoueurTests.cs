using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Joueurs.Strategies;

namespace casino.core.tests.Jeux.Poker.Joueurs;

[TestClass]
public class JoueurTests
{
    [TestMethod]
    public void Constructeur_DoitInitialiserNomEtJetons()
    {
        // Arrange
        const string nom = "Alice";
        const int jetons = 150;

        // Act
        var joueur = new Joueur(nom, jetons);

        // Assert
        Assert.AreEqual(nom, joueur.Nom, "Le nom doit correspondre au paramètre du constructeur.");
        Assert.AreEqual(jetons, joueur.Jetons, "Le nombre de jetons doit correspondre au paramètre du constructeur.");
    }

    [TestMethod]
    public void Jetons_Negatifs_DoitEtreRameneAZero()
    {
        // Arrange
        var joueur = new Joueur("Bob", 50);

        // Act
        joueur.Jetons = -25;

        // Assert
        Assert.AreEqual(0, joueur.Jetons, "Le nombre de jetons ne peut pas être négatif.");
    }

    [TestMethod]
    public void JoueurHumain_DoitHerediterDuComportementDeBase()
    {
        // Act
        var joueur = new JoueurHumain("Elena", 75);

        // Assert
        Assert.AreEqual("Elena", joueur.Nom, "Le nom du joueur humain doit provenir du constructeur.");
        Assert.AreEqual(75, joueur.Jetons, "Le nombre de jetons doit être initialisé via le constructeur de base.");
    }

    [TestMethod]
    public void JoueurOrdi_SansStrategie_DoitUtiliserStrategieRandom()
    {
        // Act
        var joueur = new JoueurOrdi("Bot", 120);

        // Assert
        Assert.IsInstanceOfType(joueur.Strategie, typeof(StrategieRandom), "La stratégie par défaut doit être aléatoire.");
    }

    [TestMethod]
    public void JoueurOrdi_AvecStrategie_DoitUtiliserCelleFournie()
    {
        // Arrange
        var strategie = new StrategieConservatrice();

        // Act
        var joueur = new JoueurOrdi("Bot", 120, strategie);

        // Assert
        Assert.AreSame(strategie, joueur.Strategie, "La stratégie fournie doit être utilisée telle quelle.");
    }
}
