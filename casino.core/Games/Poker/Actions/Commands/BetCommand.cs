using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using System;

namespace casino.core.Games.Poker.Actions.Commands;

public class BetCommand : IPlayerCommand
{
    private readonly Player _player;
    private readonly int _amount;

    public BetCommand(Player player, int amount)
    {
        _player = player;
        _amount = amount;
    }

    public void Execute(Round round)
    {
        if (_amount <= 0)
        {
            throw new ArgumentException("Le montant de mise doit être supérieur à zéro.");
        }

        if (round.CurrentBet > _amount)
        {
            throw new InvalidOperationException("La mise ne peut pas être inférieure à la mise de départ/actuelle.");
        }

        var currentBet = round.GetBetFor(_player);
        var diff = _amount - currentBet;

        if (diff <= 0)
        {
            throw new InvalidOperationException("La mise doit augmenter la contribution du Player.");
        }

        if (diff > _player.Chips)
        {
            throw new InvalidOperationException("Le Player n'a pas assez de jetons pour miser autant.");
        }

        _player.LastAction = PokerTypeAction.Bet;
        // Mettre à jour la contribution du Player et le pot en fonction de la différence,
        // pas du montant total, pour gérer les mises partielles déjà effectuées.
        _player.Chips -= diff;
        round.SetBetFor(_player, _amount);
        round.CurrentBet = _amount;
        round.Pot += diff;
    }
}
