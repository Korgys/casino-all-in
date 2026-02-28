using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;
using System;

namespace casino.core.Games.Poker.Actions.Commands;

public class BetCommand : IPlayerCommand
{
    private readonly Player _player;
    private readonly int _montant;

    public BetCommand(Player Player, int montant)
    {
        _player = Player;
        _montant = montant;
    }

    public void Execute(Partie partie)
    {
        if (_montant <= 0)
        {
            throw new ArgumentException("Le montant de mise doit être supérieur à zéro.");
        }

        if (partie.CurrentBet > _montant)
        {
            throw new InvalidOperationException("La mise ne peut pas être inférieure à la mise de départ/actuelle.");
        }

        var misePlayer = partie.GetBetFor(_player);
        var difference = _montant - misePlayer;

        if (difference <= 0)
        {
            throw new InvalidOperationException("La mise doit augmenter la contribution du Player.");
        }

        if (difference > _player.Chips)
        {
            throw new InvalidOperationException("Le Player n'a pas assez de jetons pour miser autant.");
        }

        _player.LastAction = TypeActionJeu.Miser;
        // Mettre à jour la contribution du Player et le pot en fonction de la différence,
        // pas du montant total, pour gérer les mises partielles déjà effectuées.
        _player.Chips -= difference;
        partie.SetBetFor(_player, _montant);
        partie.CurrentBet = _montant;
        partie.Pot += difference;
    }
}
