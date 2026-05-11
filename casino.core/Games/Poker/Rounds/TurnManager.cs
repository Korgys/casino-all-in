using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;

namespace casino.core.Games.Poker.Rounds;

public class TurnManager
{
    private readonly Round _round;

    public int InitialPlayerIndex { get; }
    public int CurrentPlayerIndex { get; private set; }

    public TurnManager(Round round, int initialPlayerIndex)
    {
        _round = round ?? throw new ArgumentNullException(nameof(round));
        InitialPlayerIndex = initialPlayerIndex;
        CurrentPlayerIndex = initialPlayerIndex;
    }

    public Player GetPlayerToAct()
    {
        if (!_round.IsInProgress())
            return _round.Players[CurrentPlayerIndex];

        TransitionToNextActingState();

        if (!_round.IsInProgress())
            return _round.Players[CurrentPlayerIndex];

        var playerToAct = _round.Players[CurrentPlayerIndex];
        if (!IsPlayerEligibleToActNow(playerToAct))
            throw new InvalidOperationException("No active player at the table.");

        return playerToAct;
    }

    public void ExecutePlayerAction(Player player, GameAction action)
    {
        _round.ApplyAction(player, action);
        EndPlayerTurn();
    }

    private void EndPlayerTurn()
    {
        if (!_round.IsInProgress())
            return;

        if (_round.IsBettingRoundClosed())
        {
            AdvanceToNextPhaseAndReselectActor();
            TransitionToNextActingState();
            return;
        }

        MoveIndexToNextEligiblePlayer();
        TransitionToNextActingState();
    }

    private void TransitionToNextActingState()
    {
        while (_round.IsInProgress())
        {
            if (_round.IsBettingRoundClosed())
            {
                AdvanceToNextPhaseAndReselectActor();
                continue;
            }

            if (NoPlayerCanAct())
            {
                if (_round.Players.Any(player => !player.IsFolded() && !player.IsAllIn()))
                    throw new InvalidOperationException("No active player at the table.");

                AdvanceToNextPhaseAndReselectActor();
                continue;
            }

            if (!IsPlayerEligibleToActNow(_round.Players[CurrentPlayerIndex]))
                MoveIndexToNextEligiblePlayer();

            if (!IsPlayerEligibleToActNow(_round.Players[CurrentPlayerIndex]))
                throw new InvalidOperationException("No active player at the table.");

            return;
        }
    }

    private void AdvanceToNextPhaseAndReselectActor()
    {
        _round.AdvancePhase();

        if (!_round.IsInProgress())
            return;

        CurrentPlayerIndex = InitialPlayerIndex;

        if (!NoPlayerCanAct() && !IsPlayerEligibleToActNow(_round.Players[CurrentPlayerIndex]))
            MoveIndexToNextEligiblePlayer();
    }

    private void MoveIndexToNextEligiblePlayer()
    {
        var attempts = 0;
        do
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _round.Players.Count;
            attempts++;
        }
        while (!IsPlayerEligibleToActNow(_round.Players[CurrentPlayerIndex]) && attempts < _round.Players.Count);
    }

    private bool IsPlayerEligibleToActNow(Player player)
    {
        if (!_round.IsInProgress())
            return false;

        if (player.LastAction == PokerTypeAction.Fold)
            return false;

        if (player.LastAction == PokerTypeAction.AllIn)
            return false;

        return _round.GetAvailableActions(player).Any();
    }

    private bool NoPlayerCanAct()
    {
        foreach (var player in _round.Players)
        {
            if (IsPlayerEligibleToActNow(player))
                return false;
        }

        return true;
    }
}
