using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;

namespace casino.core.tests.Games.Poker.Players;

[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void Constructeur_DoitInitialiserNomEtJetons()
    {
        // Arrange
        const string name = "Alice";
        const int chips = 150;

        // Act
        var Player = new Player(name, chips);

        // Assert
        Assert.AreEqual(name, Player.Name, "Le nom doit correspondre au paramètre du constructeur.");
        Assert.AreEqual(chips, Player.Chips, "Le nombre de jetons doit correspondre au paramètre du constructeur.");
    }

    [TestMethod]
    public void Jetons_Negatifs_DoitEtreRameneAZero()
    {
        // Arrange
        var Player = new Player("Bob", 50);

        // Act
        Player.Chips = -25;

        // Assert
        Assert.AreEqual(0, Player.Chips, "Le nombre de jetons ne peut pas être négatif.");
    }

    [TestMethod]
    public void PlayerHumain_DoitHerediterDuComportementDeBase()
    {
        // Act
        var Player = new PlayerHumain("Elena", 75);

        // Assert
        Assert.AreEqual("Elena", Player.Name, "Le nom du Player humain doit provenir du constructeur.");
        Assert.AreEqual(75, Player.Chips, "Le nombre de jetons doit être initialisé via le constructeur de base.");
    }

    [TestMethod]
    public void PlayerOrdi_SansStrategie_DoitUtiliserStrategieRandom()
    {
        // Act
        var Player = new PlayerOrdi("Bot", 120);

        // Assert
        Assert.IsInstanceOfType(Player.Strategie, typeof(StrategieRandom), "La stratégie par défaut doit être aléatoire.");
    }

    [TestMethod]
    public void PlayerOrdi_AvecStrategie_DoitUtiliserCelleFournie()
    {
        // Arrange
        var strategie = new StrategieConservatrice();

        // Act
        var Player = new PlayerOrdi("Bot", 120, strategie);

        // Assert
        Assert.AreSame(strategie, Player.Strategie, "La stratégie fournie doit être utilisée telle quelle.");
    }
}
