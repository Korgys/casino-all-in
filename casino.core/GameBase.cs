using casino.core.Common.Events;

namespace casino.core;

public abstract class GameBase : IGame
{
    protected GameBase(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public event EventHandler<GamePhaseEventArgs>? PhaseAdvanced;
    public event EventHandler<PotUpdatedEventArgs>? PotUpdated;
    public event EventHandler<GameEndedEventArgs>? GameEnded;
    public event EventHandler<GameStateEventArgs>? StateUpdated;

    public void Run()
    {
        InitializeGame();
        ExecuteGameLoop();
        ResolveGame();
        CleanupGame();
    }

    protected abstract void InitializeGame();
    protected abstract void ExecuteGameLoop();
    protected abstract void ResolveGame();
    protected abstract void CleanupGame();

    protected void OnPhaseAdvanced(string phase)
        => PhaseAdvanced?.Invoke(this, new GamePhaseEventArgs(phase));

    protected void OnPotUpdated(int pot, int miseActuelle)
        => PotUpdated?.Invoke(this, new PotUpdatedEventArgs(pot, miseActuelle));

    protected void OnGameEnded(string winnerName, int pot)
        => GameEnded?.Invoke(this, new GameEndedEventArgs(winnerName, pot));

    protected void OnStateUpdated(object state)
        => StateUpdated?.Invoke(this, new GameStateEventArgs(state));
}
