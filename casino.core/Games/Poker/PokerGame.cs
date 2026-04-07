using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;

namespace casino.core.Games.Poker;

/// <summary>
/// Represents a poker game workflow.
/// </summary>
public class PokerGame : GameBase
{
    private readonly Func<bool> _continuePlaying;
    private readonly Func<ActionRequest, Actions.GameAction> _humanActionSelector;
    private readonly Func<IDeck> _deckFactory;
    private readonly IWaitStrategy _waitStrategy;
    private readonly TablePoker _table;
    private readonly List<HumanPlayer> _humanPlayers;
    private readonly List<Player> _players;

    /// <summary>
    /// Initializes a new poker game instance.
    /// </summary>
    /// <param name="players">The players participating in the game.</param>
    /// <param name="deckFactory">Creates a deck for each round.</param>
    /// <param name="humanActionSelector">Selects a human player action.</param>
    /// <param name="continuePlaying">Determines whether the session continues.</param>
    /// <param name="waitStrategy">Provides wait behavior for computer turns.</param>
    public PokerGame(
        IEnumerable<Player> players,
        Func<IDeck> deckFactory,
        Func<ActionRequest, Actions.GameAction> humanActionSelector,
        Func<bool> continuePlaying,
        IWaitStrategy waitStrategy)
        : base("Poker")
    {
        _players = players.ToList();
        _humanPlayers = _players.OfType<HumanPlayer>().ToList();
        _deckFactory = deckFactory;
        _humanActionSelector = humanActionSelector;
        _continuePlaying = continuePlaying;
        _waitStrategy = waitStrategy;
        _table = new TablePoker { Name = "Table Poker" };
    }

    /// <summary>
    /// Initializes game state before play starts.
    /// </summary>
    protected override void InitializeGame()
    {
        OnStateUpdated(BuildPokerGameState());
    }

    /// <summary>
    /// Runs poker rounds until the session ends.
    /// </summary>
    protected override void ExecuteGameLoop()
    {
        while (_humanPlayers.Any(j => j.Chips > 0) && _players.Any(j => j.Chips > 0))
        {
            var deck = _deckFactory();
            _table.StartRound(_players, deck);

            OnPhaseAdvanced(_table.Round.Phase.ToString());
            OnPotUpdated(_table.Round.Pot, _table.Round.CurrentBet);
            OnStateUpdated(BuildPokerGameState());

            PlayRound();

            var winners = _table.Round.Winners;

            var winnersLabel = (winners is null || winners.Count == 0)
                ? _players[0].Name
                : string.Join(", ", winners.Select(g => g.Name));

            OnGameEnded(winnersLabel, _table.Round.Pot);
            OnStateUpdated(BuildPokerGameState());

            if (!_humanPlayers.Any(j => j.Chips > 0) || !_players.Any(j => j.Chips > 0))
                break;

            if (!_continuePlaying())
                break;
        }
    }

    /// <summary>
    /// Plays the current round until it ends.
    /// </summary>
    private void PlayRound()
    {
        while (_table.Round.IsInProgress())
        {
            OnStateUpdated(BuildPokerGameState());

            var phaseBeforeAction = _table.Round.Phase;
            var currentPlayer = _table.GetPlayerToAct();
            var availableActions = _table.GetAvailableActions(currentPlayer);

            if (currentPlayer is HumanPlayer human)
            {
                var context = new ActionRequest(
                    human.Name,
                    availableActions,
                    _table.Round.StartingBet,
                    _table.Round.CurrentBet,
                    _table.Round.Pot,
                    BuildPokerGameState());

                var action = _humanActionSelector(context);
                _table.ProcessPlayerAction(human, action);
            }
            else if (currentPlayer is ComputerPlayer computer)
            {
                _waitStrategy.Wait();
                var context = new GameContext(_table.Round, computer, availableActions);
                var action = computer.Strategy.DecideAction(context);
                _table.ProcessPlayerAction(computer, action);
            }

            if (phaseBeforeAction != _table.Round.Phase)
                OnPhaseAdvanced(_table.Round.Phase.ToString());

            OnPotUpdated(_table.Round.Pot, _table.Round.CurrentBet);
        }
    }

    /// <summary>
    /// Builds the current poker game state snapshot.
    /// </summary>
    /// <returns>The current poker game state.</returns>
    private PokerGameState BuildPokerGameState()
    {
        var round = _table.Round;

        return new PokerGameState(
            round?.Phase.ToString() ?? string.Empty,
            round?.StartingBet ?? 0,
            round?.Pot ?? 0,
            round?.CurrentBet ?? 0,
            round?.CommunityCards ?? new TableCards(),
            _players.Select(j => new PokerPlayerState(
                j.Name,
                j.Chips,
                round?.GetBetFor(j) ?? 0,
                j is HumanPlayer,
                j.LastAction == Actions.PokerTypeAction.Fold,
                j.LastAction,
                j.Hand,
                round?.Winners?.Any(g => g.Name == j.Name) == true)).ToList(),
            round == null || _table.TurnManager == null ? string.Empty : _table.GetPlayerToAct().Name);
    }
}
