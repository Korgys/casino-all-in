using casino.core.Common.Events;

namespace casino.core;

/// <summary>
/// Defines the contract for a runnable game.
/// </summary>
public interface IGame
{
    string Name { get; }

    event EventHandler<GamePhaseEventArgs>? PhaseAdvanced;
    event EventHandler<PotUpdatedEventArgs>? PotUpdated;
    event EventHandler<GameEndedEventArgs>? GameEnded;
    event EventHandler<GameStateEventArgs>? StateUpdated;

    /// <summary>
    /// Runs the game.
    /// </summary>
    void Run();
}
