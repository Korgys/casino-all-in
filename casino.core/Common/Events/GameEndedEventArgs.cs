namespace casino.core.Common.Events;

/// <summary>
/// Provides data for the event that is raised when a game has ended, 
/// including the winner's name and the total pot amount.
/// </summary>
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
