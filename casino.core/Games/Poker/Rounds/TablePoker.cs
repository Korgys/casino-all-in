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
    public int NumberOfRoundsPlayed { get; private set; } = 0;

    public void StartRound(List<Player> players, IDeck deck)
    {
        players.ForEach(j => j.Reset());

        Players = players.ToList();
        int startingBet = Round?.StartingBet ?? 10;
        int roundPlayed = Round?.NumberOfRoundsPlayed ?? 0;
        Round = new Round(Players, deck, roundPlayed)
        {
            StartingBet = startingBet
        };
        _playerInitialIndex = (_playerInitialIndex + 1) % this.Players.Count;
        TurnManager = new TurnManager(Round, _playerInitialIndex);
    }

    public List<PokerTypeAction> GetAvailableActions(Player player)
        => Round.GetAvailableActions(player).OrderBy(a => (int)a).ToList();

    public void ProcessPlayerAction(Player player, Actions.GameAction action)
    {
        TurnManager.ExecutePlayerAction(player, action);
    }

    public Player GetPlayerToAct()
        => TurnManager.GetPlayerToAct();
}
