using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using System;
using System.Linq;

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
        {
            return _round.Players[CurrentPlayerIndex];
        }

        PositionOnAvailablePlayer();

        if (!_round.IsInProgress())
        {
            return _round.Players[CurrentPlayerIndex];
        }

        if (NoPlayerCanAct())
        {
            throw new InvalidOperationException("No active player at the table.");
        }

        return _round.Players[CurrentPlayerIndex];
    }

    public void ExecutePlayerAction(Player player, GameAction action)
    {
        _round.ApplyAction(player, action);
        EndPlayerTurn();
    }

    private void EndPlayerTurn()
    {
        if (!_round.IsInProgress())
        {
            return;
        }

        if (_round.IsBettingRoundClosed())
        {
            AdvanceUntilNextPhaseWithActions();
            return;
        }

        MoveIndexToNextPlayer();

        if (NoPlayerCanAct())
        {
            AdvanceUntilNextPhaseWithActions();
        }
    }

    private void PositionOnAvailablePlayer()
    {
        if (NoPlayerCanAct())
        {
            AdvanceUntilNextPhaseWithActions();
            return;
        }

        if (!PlayerCanAct(_round.Players[CurrentPlayerIndex]))
        {
            MoveIndexToNextPlayer();
        }
    }

    private void AdvanceUntilNextPhaseWithActions()
    {
        while (_round.IsInProgress())
        {
            if (!NoPlayerCanAct() && !_round.IsBettingRoundClosed())
            {
                MoveIndexToNextPlayer();
                return;
            }

            _round.AdvancePhase();

            if (!_round.IsInProgress())
            {
                return;
            }

            CurrentPlayerIndex = InitialPlayerIndex;

            if (!NoPlayerCanAct())
            {
                if (!PlayerCanAct(_round.Players[CurrentPlayerIndex]))
                {
                    MoveIndexToNextPlayer();
                }

                return;
            }
        }
    }

    private void MoveIndexToNextPlayer()
    {
        var attempts = 0;
        do
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _round.Players.Count;
            attempts++;
        }
        while (!PlayerCanAct(_round.Players[CurrentPlayerIndex]) && attempts <= _round.Players.Count);
    }

    private bool PlayerCanAct(Player player)
    {
        if (player.LastAction == PokerTypeAction.Fold)
            return false;

        if (player.LastAction == PokerTypeAction.AllIn)
            return false;

        return _round.GetAvailableActions(player).Any();
    }

    private bool NoPlayerCanAct() => !_round.Players.Any(PlayerCanAct);
}