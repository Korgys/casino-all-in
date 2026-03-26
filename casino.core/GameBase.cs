using casino.core.Common.Events;

namespace casino.core;

/// <summary>
/// Provides base behavior for game implementations.
/// </summary>
public abstract class GameBase : IGame
{
    /// <summary>
    /// Initializes a new game base instance.
    /// </summary>
    /// <param name="name">The game display name.</param>
    protected GameBase(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public event EventHandler<GamePhaseEventArgs>? PhaseAdvanced;
    public event EventHandler<PotUpdatedEventArgs>? PotUpdated;
    public event EventHandler<GameEndedEventArgs>? GameEnded;
    public event EventHandler<GameStateEventArgs>? StateUpdated;

    /// <summary>
    /// Runs the game lifecycle.
    /// </summary>
    public void Run()
    {
        InitializeGame();
        ExecuteGameLoop();
    }

    /// <summary>
    /// Initializes game state before the loop starts.
    /// </summary>
    protected abstract void InitializeGame();

    /// <summary>
    /// Executes the main game loop.
    /// </summary>
    protected abstract void ExecuteGameLoop();

    /// <summary>
    /// Raises the phase advanced event.
    /// </summary>
    /// <param name="phase">The new phase name.</param>
    protected void OnPhaseAdvanced(string phase)
        => PhaseAdvanced?.Invoke(this, new GamePhaseEventArgs(phase));

    /// <summary>
    /// Raises the pot updated event.
    /// </summary>
    /// <param name="pot">The total pot amount.</param>
    /// <param name="currentBet">The current bet amount.</param>
    protected void OnPotUpdated(int pot, int currentBet)
        => PotUpdated?.Invoke(this, new PotUpdatedEventArgs(pot, currentBet));

    /// <summary>
    /// Raises the game ended event.
    /// </summary>
    /// <param name="winnerName">The winner name.</param>
    /// <param name="pot">The final pot amount.</param>
    protected void OnGameEnded(string winnerName, int pot)
        => GameEnded?.Invoke(this, new GameEndedEventArgs(winnerName, pot));

    /// <summary>
    /// Raises the state updated event.
    /// </summary>
    /// <param name="state">The current game state object.</param>
    protected void OnStateUpdated(object state)
        => StateUpdated?.Invoke(this, new GameStateEventArgs(state));
}
