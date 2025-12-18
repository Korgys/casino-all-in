namespace casino.core.Common.Events;

public class GameEndedEventArgs : EventArgs
{
    public GameEndedEventArgs(string winnerName, int pot)
    {
        WinnerName = winnerName;
        Pot = pot;
    }

    public string WinnerName { get; }

    public int Pot { get; }
}
