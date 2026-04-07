using casino.console.Games.Poker;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;

namespace casino.console.tests.Games.Poker;

[TestClass]
public class ConsolePokerInputTests
{
    [TestMethod]
    public void AskContinueNewGame_ReturnsTrue_ForYesAliases()
    {
        AssertAskContinue("o\n", expected: true);
        AssertAskContinue("oui\n", expected: true);
        AssertAskContinue("y\n", expected: true);
        AssertAskContinue("yes\n", expected: true);
        AssertAskContinue("j\n", expected: true);
        AssertAskContinue("ja\n", expected: true);
    }

    [TestMethod]
    public void AskContinueNewGame_ReturnsFalse_ForNo()
    {
        AssertAskContinue("n\n", expected: false);
    }

    [TestMethod]
    public void GetPlayerAction_ReturnsBetWithMinimumBetAmount()
    {
        var request = CreateRequest(
            playerName: "Alice",
            availableActions: [PokerTypeAction.Check, PokerTypeAction.Bet],
            minimumBet: 25,
            currentBet: 25,
            contribution: 0,
            chips: 100);

        var originalIn = Console.In;
        try
        {
            Console.SetIn(new StringReader($"{(int)PokerTypeAction.Bet}\n"));

            var action = ConsolePokerInput.GetPlayerAction(request);

            Assert.AreEqual(PokerTypeAction.Bet, action.TypeAction);
            Assert.AreEqual(25, action.Amount);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    [TestMethod]
    public void GetPlayerAction_ReadRaiseAmount_IgnoresInvalidEntriesUntilValid()
    {
        var request = CreateRequest(
            playerName: "Alice",
            availableActions: [PokerTypeAction.Raise, PokerTypeAction.Call],
            minimumBet: 10,
            currentBet: 20,
            contribution: 5,
            chips: 30);

        var originalIn = Console.In;
        try
        {
            Console.SetIn(new StringReader($"999\n{(int)PokerTypeAction.Raise}\nabc\n20\n36\n35\n"));

            var action = ConsolePokerInput.GetPlayerAction(request);

            Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
            Assert.AreEqual(35, action.Amount);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    [TestMethod]
    public void GetPlayerAction_InvalidChoice_RefreshesActionMenu()
    {
        var request = CreateRequest(
            playerName: "Alice",
            availableActions: [PokerTypeAction.Fold, PokerTypeAction.Check],
            minimumBet: 10,
            currentBet: 10,
            contribution: 0,
            chips: 100);

        var originalIn = Console.In;
        var originalOut = Console.Out;
        var writer = new StringWriter();

        try
        {
            Console.SetIn(new StringReader($"999\n{(int)PokerTypeAction.Check}\n"));
            Console.SetOut(writer);

            var action = ConsolePokerInput.GetPlayerAction(request);

            Assert.AreEqual(PokerTypeAction.Check, action.TypeAction);
            Assert.AreEqual(2, CountOccurrences(writer.ToString(), "┌"));
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    public void GetPlayerAction_RaiseAmount_AcceptsBoundaryAtMaximumAllowedTarget()
    {
        var request = CreateRequest(
            playerName: "Alice",
            availableActions: [PokerTypeAction.Raise],
            minimumBet: 10,
            currentBet: 20,
            contribution: 5,
            chips: 30);

        var originalIn = Console.In;
        try
        {
            Console.SetIn(new StringReader($"{(int)PokerTypeAction.Raise}\n20\n36\n35\n"));

            var action = ConsolePokerInput.GetPlayerAction(request);

            Assert.AreEqual(PokerTypeAction.Raise, action.TypeAction);
            Assert.AreEqual(35, action.Amount);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    private static ActionRequest CreateRequest(
        string playerName,
        IReadOnlyList<PokerTypeAction> availableActions,
        int minimumBet,
        int currentBet,
        int contribution,
        int chips)
    {
        var player = new PokerPlayerState(
            Name: playerName,
            Chips: chips,
            Contribution: contribution,
            IsHuman: true,
            IsFolded: false,
            LastAction: PokerTypeAction.None,
            Hand: new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts)),
            IsWinner: false);

        var state = new PokerGameState(
            Phase: "Turn",
            StartingBet: minimumBet,
            Pot: 100,
            CurrentBet: currentBet,
            CommunityCards: new TableCards(),
            Players: [player],
            CurrentPlayer: playerName);

        return new ActionRequest(playerName, availableActions, minimumBet, currentBet, 100, state);
    }

    private static void AssertAskContinue(string input, bool expected)
    {
        var originalIn = Console.In;
        try
        {
            Console.SetIn(new StringReader(input));
            var result = ConsolePokerInput.AskContinueNewGame();
            Assert.AreEqual(expected, result);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    private static int CountOccurrences(string source, string value)
    {
        var count = 0;
        var index = 0;

        while ((index = source.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}
