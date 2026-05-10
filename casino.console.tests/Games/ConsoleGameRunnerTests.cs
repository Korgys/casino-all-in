using casino.console.Games;
using casino.core;
using casino.core.Common.Events;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Roulette;
using casino.core.Games.Slots;

namespace casino.console.tests.Games;

[TestClass]
public class ConsoleGameRunnerTests
{
    [TestMethod]
    public void Run_CallsGameRun()
    {
        var game = new FakeGame();
        var runner = CreateRunner();

        runner.Run(game);

        Assert.AreEqual(1, game.RunCount);
    }

    [TestMethod]
    public void Run_RoutesStateUpdatesToMatchingRenderer()
    {
        var pokerRenders = 0;
        var blackjackRenders = 0;
        var slotRenders = 0;
        var rouletteRenders = 0;
        var game = new FakeGame
        {
            OnRun = fakeGame =>
            {
                fakeGame.RaiseState(CreatePokerState());
                fakeGame.RaiseState(new BlackjackGameState
                {
                    PlayerCards = [],
                    DealerCards = [],
                    IsDealerHoleCardHidden = false,
                    IsRoundOver = false,
                    StatusMessage = string.Empty,
                    RoundOutcome = BlackjackRoundOutcome.InProgress,
                    PlayerWins = 0,
                    DealerWins = 0,
                    Pushes = 0
                });
                fakeGame.RaiseState(new SlotMachineGameState());
                fakeGame.RaiseState(new RouletteGameState());
            }
        };
        var runner = new ConsoleGameRunner(
            _ => pokerRenders++,
            _ => blackjackRenders++,
            _ => slotRenders++,
            _ => rouletteRenders++,
            () => { });

        runner.Run(game);

        Assert.AreEqual(1, pokerRenders);
        Assert.AreEqual(1, blackjackRenders);
        Assert.AreEqual(1, slotRenders);
        Assert.AreEqual(1, rouletteRenders);
    }

    [TestMethod]
    public void Run_RefreshesPokerTableAfterGameEndedBeforeNextPokerRender()
    {
        var pokerRenders = 0;
        var pokerRefreshes = 0;
        var game = new FakeGame
        {
            OnRun = fakeGame =>
            {
                fakeGame.RaiseState(CreatePokerState());
                fakeGame.RaiseGameEnded();
                fakeGame.RaiseState(CreatePokerState());
            }
        };
        var runner = new ConsoleGameRunner(
            _ => pokerRenders++,
            _ => { },
            _ => { },
            _ => { },
            () => pokerRefreshes++);

        runner.Run(game);

        Assert.AreEqual(2, pokerRenders);
        Assert.AreEqual(1, pokerRefreshes);
    }

    private static ConsoleGameRunner CreateRunner()
    {
        return new ConsoleGameRunner(_ => { }, _ => { }, _ => { }, _ => { }, () => { });
    }

    private static PokerGameState CreatePokerState()
    {
        var player = new PokerPlayerState(
            Name: "Alice",
            Chips: 1000,
            Contribution: 0,
            IsHuman: true,
            IsFolded: false,
            LastAction: PokerTypeAction.None,
            Hand: new HandCards(new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts)),
            IsWinner: false);

        return new PokerGameState(
            Phase: "Pre-Flop",
            StartingBet: 10,
            Pot: 0,
            CurrentBet: 0,
            CommunityCards: new TableCards(),
            Players: [player],
            CurrentPlayer: player.Name);
    }

    private sealed class FakeGame : IGame
    {
        public string Name => "fake";

        public int RunCount { get; private set; }

        public Action<FakeGame>? OnRun { get; init; }

        public event EventHandler<GamePhaseEventArgs>? PhaseAdvanced
        {
            add { }
            remove { }
        }

        public event EventHandler<PotUpdatedEventArgs>? PotUpdated
        {
            add { }
            remove { }
        }

        public event EventHandler<GameEndedEventArgs>? GameEnded;
        public event EventHandler<GameStateEventArgs>? StateUpdated;

        public void Run()
        {
            RunCount++;
            OnRun?.Invoke(this);
        }

        public void RaiseState(object state)
        {
            StateUpdated?.Invoke(this, new GameStateEventArgs(state));
        }

        public void RaiseGameEnded()
        {
            GameEnded?.Invoke(this, new GameEndedEventArgs("Alice", 0));
        }
    }
}
