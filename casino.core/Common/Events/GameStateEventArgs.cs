namespace casino.core.Common.Events;

/// <summary>
/// Provides data for the event that is raised when a game state changes,
/// </summary>
public class GameStateEventArgs : EventArgs
{
    public GameStateEventArgs(object state)
    {
        State = state;
    }

    public object State { get; }
}
