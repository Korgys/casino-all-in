using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;

namespace casino.core.tests.Games.Poker.Players;

[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeNameAndChips()
    {
        // Arrange
        const string name = "Alice";
        const int chips = 150;

        // Act
        var player = new Player(name, chips);

        // Assert
        Assert.AreEqual(name, player.Name, "The name should match the constructor value.");
        Assert.AreEqual(chips, player.Chips, "The chip count should match the constructor value.");
    }

    [TestMethod]
    public void Chips_WhenNegative_ShouldBeClampedToZero()
    {
        // Arrange
        var player = new Player("Bob", 50);

        // Act
        player.Chips = -25;

        // Assert
        Assert.AreEqual(0, player.Chips, "Chip count cannot be negative.");
    }

    [TestMethod]
    public void HumanPlayer_ShouldUseBasePlayerBehavior()
    {
        // Act
        var player = new HumanPlayer("Elena", 75);

        // Assert
        Assert.AreEqual("Elena", player.Name, "The human player name should come from the constructor.");
        Assert.AreEqual(75, player.Chips, "The chip count should be initialized by the base constructor.");
    }

    [TestMethod]
    public void ComputerPlayer_WithoutStrategy_ShouldUseRandomStrategy()
    {
        // Act
        var player = new ComputerPlayer("Bot", 120);

        // Assert
        Assert.IsInstanceOfType(player.Strategy, typeof(RandomStrategy), "The default strategy should be random.");
    }

    [TestMethod]
    public void ComputerPlayer_WithStrategy_ShouldUseProvidedStrategy()
    {
        // Arrange
        var strategy = new ConservativeStrategy();

        // Act
        var player = new ComputerPlayer("Bot", 120, strategy);

        // Assert
        Assert.AreSame(strategy, player.Strategy, "The provided strategy should be used as-is.");
    }
}
