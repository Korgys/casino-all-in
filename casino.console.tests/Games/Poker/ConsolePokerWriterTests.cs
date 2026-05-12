using System.Reflection;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;

namespace casino.console.tests.Games.Poker;

[TestClass]
public class ConsolePokerWriterTests
{
    [TestMethod]
    public void WritePlayerName_ShouldWritePlayerNameForFoldedAndHumanPlayers()
    {
        var output = CaptureConsole(() =>
        {
            InvokeWriter("WritePlayerName", CreatePlayer("Alice", isHuman: true, isFolded: false, chips: 100, PokerTypeAction.None));
            Console.Write('|');
            InvokeWriter("WritePlayerName", CreatePlayer("Bob", isHuman: false, isFolded: true, chips: 0, PokerTypeAction.Fold));
        });

        Assert.AreEqual("Alice|Bob", output);
    }

    [TestMethod]
    public void WriteAmount_WriteCard_WriteHand_AndWriteCommunityCards_ShouldRenderExpectedText()
    {
        var hand = new HandCards(new Card(CardRank.Ace, Suit.Hearts), new Card(CardRank.King, Suit.Spades));
        var tableCards = new TableCards
        {
            Flop1 = new Card(CardRank.Queen, Suit.Clubs),
            Flop2 = new Card(CardRank.Jack, Suit.Diamonds),
            Turn = new Card(CardRank.Ten, Suit.Hearts)
        };

        var output = CaptureConsole(() =>
        {
            InvokeWriter("WriteAmount", 120);
            Console.Write('|');
            InvokeWriter("WriteCard", new Card(CardRank.Ace, Suit.Hearts));
            Console.Write('|');
            InvokeWriter("WriteHand", hand);
            Console.Write('|');
            InvokeWriter("WriteCommunityCards", tableCards);
        });

        Assert.AreEqual("120c|A♥|A♥ K♠|Q♣ J♦ 10♥", output);
    }

    private static PokerPlayerState CreatePlayer(string name, bool isHuman, bool isFolded, int chips, PokerTypeAction lastAction)
    {
        return new PokerPlayerState(
            Name: name,
            Chips: chips,
            Contribution: 0,
            IsHuman: isHuman,
            IsFolded: isFolded,
            LastAction: lastAction,
            Hand: new HandCards(new Card(CardRank.Ace, Suit.Spades), new Card(CardRank.King, Suit.Hearts)),
            IsWinner: false);
    }

    private static void InvokeWriter(string methodName, params object[] arguments)
    {
        var type = typeof(Program).Assembly.GetType("casino.console.Games.Poker.ConsolePokerWriter", throwOnError: true)!;
        var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        method.Invoke(null, arguments);
    }

    private static string CaptureConsole(Action action)
    {
        var originalOut = Console.Out;
        var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
