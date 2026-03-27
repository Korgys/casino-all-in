namespace casino.core.Games.Poker.Actions;

public class ActionRequest
{
    public ActionRequest(string playerName, IReadOnlyList<PokerTypeAction> availableActions, int minimumBet, int currentBet, int pot, object tableState)
    {
        PlayerName = playerName;
        AvailableActions = availableActions;
        MinimumBet = minimumBet;
        CurrentBet = currentBet;
        Pot = pot;
        TableState = tableState;
    }

    public string PlayerName { get; }

    public IReadOnlyList<PokerTypeAction> AvailableActions { get; }

    public int MinimumBet { get; }

    public int CurrentBet { get; }

    public int Pot { get; }

    public object TableState { get; }
}
