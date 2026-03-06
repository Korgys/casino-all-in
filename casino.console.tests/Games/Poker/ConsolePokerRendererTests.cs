using System.Reflection;
using casino.console.Games.Poker;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;

namespace casino.console.tests.Games.Poker;

[TestClass]
public class ConsolePokerRendererTests
{
    [TestMethod]
    public void NewRound_DoesNotReuseProbabilityFromPreviousRound()
    {
        var renderer = new ConsolePokerRenderer();

        var alice = CreatePlayer("Alice", isFolded: false);
        var foldedBob = CreatePlayer("Bob", isFolded: true);
        var firstRoundState = CreateState(new[] { alice, foldedBob }, currentPlayer: "Alice", phase: "PreFlop");

        InvokeWriteScoreAndProbabilityOfVictory(renderer, alice, firstRoundState, currentPlayerName: "Alice");

        renderer.ResetRoundCache();

        var output = new StringWriter();
        var originalOut = Console.Out;

        try
        {
            Console.SetOut(output);
            var activeBob = CreatePlayer("Bob", isFolded: false);
            var newRoundState = CreateState(new[] { alice, activeBob }, currentPlayer: "Bob", phase: "PreFlop");
            InvokeWriteScoreAndProbabilityOfVictory(renderer, alice, newRoundState, currentPlayerName: "Bob");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.DoesNotContain(output.ToString(), "%", "A new round should not reuse a cached probability from a previous round.");
    }

    private static PokerPlayerState CreatePlayer(string name, bool isFolded)
    {
        return new PokerPlayerState(
            Name: name,
            Chips: 100,
            Contribution: 0,
            IsHuman: true,
            IsFolded: isFolded,
            LastAction: PokerTypeAction.None,
            Hand: new HandCards(new Card(CardRank.As, Suit.Hearts), new Card(CardRank.Roi, Suit.Hearts)),
            IsWinner: false);
    }

    private static PokerGameState CreateState(IReadOnlyList<PokerPlayerState> players, string currentPlayer, string phase)
    {
        return new PokerGameState(
            Phase: phase,
            StartingBet: 10,
            Pot: 20,
            CurrentBet: 10,
            CommunityCards: new TableCards(),
            Players: players,
            CurrentPlayer: currentPlayer);
    }

    private static void InvokeWriteScoreAndProbabilityOfVictory(
        ConsolePokerRenderer renderer,
        PokerPlayerState player,
        PokerGameState state,
        string currentPlayerName)
    {
        var method = typeof(ConsolePokerRenderer).GetMethod(
            "WriteScoreAndProbabilityOfVictory",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method, "Could not find score/probability rendering method.");
        method.Invoke(renderer, new object[] { player, state, currentPlayerName });
    }
}
