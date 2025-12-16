using casino.core.Events;

namespace casino.core;

public interface IGame
{
    string Name { get; }

    event EventHandler<GamePhaseEventArgs>? PhaseAdvanced;
    event EventHandler<PotUpdatedEventArgs>? PotUpdated;
    event EventHandler<GameEndedEventArgs>? GameEnded;
    event EventHandler<GameStateEventArgs>? StateUpdated;

    void Run();
}
