namespace casino.core.Common.Events;

/// <summary>
/// Provides data for the event that is raised when a game phase changes.
/// </summary>
public class GamePhaseEventArgs : EventArgs
{
    public GamePhaseEventArgs(string phase)
    {
        Phase = phase;
    }

    public string Phase { get; }
}
