using casino.core.Games.Poker.Parties;

namespace casino.core.Games.Poker.Actions;

public class RequeteAction
{
    public RequeteAction(string playerName, IReadOnlyList<TypeActionJeu> actionsPossibles, int minimumBet, int currentBet, int pot, object tableState)
    {
        PlayerName = playerName;
        ActionsPossibles = actionsPossibles;
        MinimumBet = minimumBet;
        CurrentBet = currentBet;
        Pot = pot;
        TableState = tableState;
    }

    public string PlayerName { get; }

    public IReadOnlyList<TypeActionJeu> ActionsPossibles { get; }

    public int MinimumBet { get; }

    public int CurrentBet { get; }

    public int Pot { get; }

    public object TableState { get; }
}
