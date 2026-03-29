using casino.console.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Properties.Languages;

namespace casino.console.tests.Games.Poker;

[TestClass]
public class ConsolePokerRendererAvailableActionsTests
{
    [TestMethod]
    public void RenderAvailableActions_ShowsBetAmountAndActionLabels()
    {
        var actions = new[] { PokerTypeAction.Check, PokerTypeAction.Bet, PokerTypeAction.Fold };
        var output = new StringWriter();
        var originalOut = Console.Out;

        try
        {
            Console.SetOut(output);
            ConsolePokerRenderer.RenderAvailableActions(actions, minimumBet: 40);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var text = output.ToString();
        Assert.Contains(Resources.Check, text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(Resources.Bet, text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("40", text, "Bet action should display minimum bet amount.");
    }
}
