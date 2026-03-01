using casino.core.Games.Poker.Rounds;

namespace casino.core.Games.Poker.Actions;

public class ActionRequest
{
    public ActionRequest(string playerName, IReadOnlyList<TypeGameAction> actionsPossibles, int minimumBet, int currentBet, int pot, object tableState)
    {
        PlayerName = playerName;
        AvailableActions = actionsPossibles;
        MinimumBet = minimumBet;
        CurrentBet = currentBet;
        Pot = pot;
        TableState = tableState;
    }

    public string PlayerName { get; }

    public IReadOnlyList<TypeGameAction> AvailableActions { get; }

    public int MinimumBet { get; }

    public int CurrentBet { get; }

    public int Pot { get; }

    public object TableState { get; }
}
