using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;

namespace casino.core.Games.Poker.Rounds;

public class TablePoker
{
    public string Name { get; set; } = string.Empty;
    public Round Round { get; private set; } = null!;
    public IReadOnlyList<Player> Players { get; private set; } = new List<Player>();
    public TurnManager TurnManager { get; private set; } = null!;
    public int InitialPlayerIndex => TurnManager?.InitialPlayerIndex ?? _playerInitialIndex;
    public int CurrentPlayerIndex => TurnManager?.CurrentPlayerIndex ?? _playerInitialIndex;
    private int _playerInitialIndex = -1;

    public void DemarrerRound(List<Player> Players, IDeck deck)
    {
        Players.ForEach(j => j.Reset());

        this.Players = Players.ToList();
        Round = new Round(this.Players, deck);
        _playerInitialIndex = (_playerInitialIndex + 1) % this.Players.Count;
        TurnManager = new TurnManager(Round, _playerInitialIndex);
    }

    public List<PokerTypeAction> GetAvailableActions(Player Player)
        => Round.GetAvailableActions(Player).OrderBy(a => (int)a).ToList();

    public void TraiterActionPlayer(Player Player, Actions.GameAction choix)
    {
        TurnManager.ExecutePlayerAction(Player, choix);
    }

    public Player GetPlayerToAct()
        => TurnManager.GetPlayerToAct();
}