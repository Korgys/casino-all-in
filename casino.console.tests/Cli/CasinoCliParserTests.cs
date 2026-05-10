using casino.console.Cli;
using casino.core.Games.Poker;

namespace casino.console.tests.Cli;

[TestClass]
public class CasinoCliParserTests
{
    [TestMethod]
    public void Parse_BlackjackCommand_ReturnsBlackjack()
    {
        var result = CasinoCliParser.Parse(["blackjack"]);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Command);
        Assert.AreEqual(CasinoGameKind.Blackjack, result.Command.Game);
        Assert.IsNull(result.Command.PokerSetup);
    }

    [TestMethod]
    public void Parse_PokerWithoutOptions_UsesFactoryDefaultSetupLater()
    {
        var result = CasinoCliParser.Parse(["poker"]);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Command);
        Assert.AreEqual(CasinoGameKind.Poker, result.Command.Game);
        Assert.IsNull(result.Command.PokerSetup);
    }

    [TestMethod]
    public void Parse_PokerWithShortOptions_BuildsSetup()
    {
        var result = CasinoCliParser.Parse(["poker", "-p", "4", "-d", "4", "-c", "1000"]);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Command?.PokerSetup);
        Assert.AreEqual(4, result.Command.PokerSetup.PlayerCount);
        Assert.AreEqual(1000, result.Command.PokerSetup.InitialChips);
        Assert.HasCount(3, result.Command.PokerSetup.Opponents);
        Assert.IsTrue(result.Command.PokerSetup.Opponents.All(o => o.Difficulty == PokerDifficulty.Medium));
    }

    [TestMethod]
    public void Parse_PokerWithInlineLongOptions_BuildsSetup()
    {
        var result = CasinoCliParser.Parse(["poker", "--players=2", "--difficulty=6", "--chips=5000"]);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Command?.PokerSetup);
        Assert.AreEqual(2, result.Command.PokerSetup.PlayerCount);
        Assert.AreEqual(5000, result.Command.PokerSetup.InitialChips);
        Assert.HasCount(1, result.Command.PokerSetup.Opponents);
        Assert.AreEqual(PokerDifficulty.VeryHard, result.Command.PokerSetup.Opponents[0].Difficulty);
    }

    [TestMethod]
    public void Parse_UnknownGame_ReturnsError()
    {
        var result = CasinoCliParser.Parse(["baccarat"]);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Unknown game 'baccarat'.", result.Error);
    }

    [TestMethod]
    public void Parse_NonPokerOptions_ReturnsError()
    {
        var result = CasinoCliParser.Parse(["blackjack", "-p", "4"]);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Game 'blackjack' does not support options.", result.Error);
    }

    [TestMethod]
    public void Parse_InvalidPokerPlayerRange_ReturnsError()
    {
        var result = CasinoCliParser.Parse(["poker", "-p", "9"]);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Option 'players' must be between 2 and 6.", result.Error);
    }

    [TestMethod]
    public void Parse_MissingPokerOptionValue_ReturnsError()
    {
        var result = CasinoCliParser.Parse(["poker", "--chips"]);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Option '--chips' requires a value.", result.Error);
    }

    [TestMethod]
    public void Parse_Help_ReturnsHelp()
    {
        var result = CasinoCliParser.Parse(["help"]);

        Assert.IsTrue(result.IsHelp);
        Assert.IsFalse(result.IsSuccess);
    }
}
