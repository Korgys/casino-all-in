using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using System.Collections.Generic;
using System.Linq;

namespace casino.core.Games.Poker.Rounds;

public class TablePoker
{
    public string Name { get; set; }
    public Round Round { get; private set; }
    public List<Player> Players { get; private set; }
    public TurnManager TurnManager { get; private set; }
    public int PlayerInitialIndex => TurnManager?.PlayerInitialIndex ?? _playerInitialIndex;
    public int CurrentPlayerIndex => TurnManager?.PlayerActuelIndex ?? _playerInitialIndex;
    public int PlayerActuelIndex => CurrentPlayerIndex;
    private int _playerInitialIndex = -1;

    public void DemarrerRound(List<Player> Players, IDeck deck)
    {
        Players.ForEach(j => j.Reset());

        this.Players = Players;
        Round = new Round(this.Players, deck);
        _playerInitialIndex = (_playerInitialIndex + 1) % this.Players.Count;
        TurnManager = new TurnManager(Round, _playerInitialIndex);
    }

    public List<TypeGameAction> GetAvailableActions(Player Player)
        => Round.GetAvailableActions(Player).OrderBy(a => (int)a).ToList();

    public void TraiterActionPlayer(Player Player, Actions.GameAction choix)
    {
        TurnManager.TraiterActionPlayer(Player, choix);
    }

    public Player GetPlayerToAct()
        => TurnManager.GetPlayerToAct();
}