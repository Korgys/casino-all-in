namespace casino.core.Common.Events;

public class PotUpdatedEventArgs : EventArgs
{
    public PotUpdatedEventArgs(int pot, int miseActuelle)
    {
        Pot = pot;
        MiseActuelle = miseActuelle;
    }

    public int Pot { get; }

    public int MiseActuelle { get; }
}
