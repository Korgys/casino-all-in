namespace casino.core.Common.Events;

public class GamePhaseEventArgs : EventArgs
{
    public GamePhaseEventArgs(string phase)
    {
        Phase = phase;
    }

    public string Phase { get; }
}
