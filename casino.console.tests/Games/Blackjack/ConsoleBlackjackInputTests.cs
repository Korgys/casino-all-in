using casino.console.Games.Blackjack;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker.Cards;

namespace casino.console.tests.Games.Blackjack;

[TestClass]
public class ConsoleBlackjackInputTests
{
    [TestMethod]
    public void GetPlayerAction_ReturnsHit_ForNumericChoice()
    {
        var state = CreateState();
        var originalIn = Console.In;

        try
        {
            Console.SetIn(new StringReader("1\n"));

            var action = ConsoleBlackjackInput.GetPlayerAction(state);

            Assert.AreEqual(BlackjackAction.Hit, action);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    [TestMethod]
    public void GetPlayerAction_ReturnsStand_AfterInvalidChoice()
    {
        var state = CreateState();
        var originalIn = Console.In;

        try
        {
            Console.SetIn(new StringReader("x\n2\n"));

            var action = ConsoleBlackjackInput.GetPlayerAction(state);

            Assert.AreEqual(BlackjackAction.Stand, action);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    [TestMethod]
    public void AskContinueNewGame_ReturnsTrue_ForGermanYesAlias()
    {
        var originalIn = Console.In;

        try
        {
            Console.SetIn(new StringReader("ja\n"));

            var result = ConsoleBlackjackInput.AskContinueNewGame();

            Assert.IsTrue(result);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    private static BlackjackGameState CreateState() => new()
    {
        PlayerCards = [new Card(CardRank.As, Suit.Spades), new Card(CardRank.Huit, Suit.Hearts)],
        DealerCards = [new Card(CardRank.Roi, Suit.Clubs), new Card(CardRank.Six, Suit.Diamonds)],
        IsDealerHoleCardHidden = true,
        IsRoundOver = false,
        StatusMessage = "Votre tour.",
        RoundOutcome = BlackjackRoundOutcome.InProgress,
        PlayerWins = 0,
        DealerWins = 0,
        Pushes = 0
    };
}
