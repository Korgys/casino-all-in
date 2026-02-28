namespace casino.core.Common.Events;

/// <summary>
/// Provides data for the event that occurs when the pot 
/// or the current bet is updated in a game session.
/// </summary>
public class PotUpdatedEventArgs : EventArgs
{
    public PotUpdatedEventArgs(int pot, int currentBet)
    {
        Pot = pot;
        CurrentBet = currentBet;
    }

    public int Pot { get; }

    public int CurrentBet { get; }
}
