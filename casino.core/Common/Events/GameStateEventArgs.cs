namespace casino.core.Common.Events;

public class GameStateEventArgs : EventArgs
{
    public GameStateEventArgs(object state)
    {
        State = state;
    }

    public object State { get; }
}
