using casino.core;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;

namespace casino.core.Games.Poker;

public class PokerGame : GameBase
{
    private readonly Func<bool> _continuePlaying;
    private readonly Func<ActionRequest, Actions.GameAction> _humanActionSelector;
    private readonly Func<IDeck> _deckFactory;
    private readonly TablePoker _table;
    private readonly List<HumanPlayer> _playersHumain;
    private readonly List<Player> _players;

    public PokerGame(
        IEnumerable<Player> Players,
        Func<IDeck> deckFactory,
        Func<ActionRequest, Actions.GameAction> humanActionSelector,
        Func<bool> continuePlaying)
        : base("Poker")
    {
        _players = Players.ToList();
        _playersHumain = _players.OfType<HumanPlayer>().ToList();
        _deckFactory = deckFactory;
        _humanActionSelector = humanActionSelector;
        _continuePlaying = continuePlaying;
        _table = new TablePoker { Name = "Table Poker" };
    }

    protected override void InitializeGame()
    {
        OnStateUpdated(BuildPokerGameState());
    }

    protected override void ExecuteGameLoop()
    {
        while (_playersHumain.Any(j => j.Chips > 0))
        {
            var deck = _deckFactory();
            _table.DemarrerRound(_players, deck);

            OnPhaseAdvanced(_table.Round.Phase.ToString());
            OnPotUpdated(_table.Round.Pot, _table.Round.CurrentBet);
            OnStateUpdated(BuildPokerGameState());

            PlayRound();

            var winners = _table.Round.Winners;

            var gagnantsLabel = (winners is null || winners.Count == 0)
                ? _players.First().Name
                : string.Join(", ", winners.Select(g => g.Name));

            OnGameEnded(gagnantsLabel, _table.Round.Pot);
            OnStateUpdated(BuildPokerGameState());

            // Si plus aucun humain n'a de jetons, fin du jeu.
            if (!_playersHumain.Any(j => j.Chips > 0))
                break;

            // Si on ne veut pas continuer, fin du jeu.
            if (!_continuePlaying())
                break;
        }
    }

    private void PlayRound()
    {
        while (_table.Round.IsInProgress())
        {
            OnStateUpdated(BuildPokerGameState());

            var phaseAvantAction = _table.Round.Phase;
            var PlayerActuel = _table.GetPlayerToAct();
            var actionsPossibles = _table.GetAvailableActions(PlayerActuel);

            if (PlayerActuel is HumanPlayer humain)
            {
                var contexte = new ActionRequest(
                    humain.Name,
                    actionsPossibles,
                    _table.Round.StartingBet,
                    _table.Round.CurrentBet,
                    _table.Round.Pot,
                    BuildPokerGameState());

                var action = _humanActionSelector(contexte);
                _table.TraiterActionPlayer(humain, action);
            }
            else if (PlayerActuel is ComputerPlayer ordi)
            {
                Thread.Sleep(Random.Shared.Next(500, 1500));
                var contexte = new GameContext(_table.Round, ordi, actionsPossibles);
                var action = ordi.Strategy.DecideAction(contexte);
                _table.TraiterActionPlayer(ordi, action);
            }

            if (phaseAvantAction != _table.Round.Phase)
                OnPhaseAdvanced(_table.Round.Phase.ToString());

            OnPotUpdated(_table.Round.Pot, _table.Round.CurrentBet);
        }
    }

    private PokerGameState BuildPokerGameState()
    {
        var partie = _table.Round;

        return new PokerGameState(
            partie?.Phase.ToString() ?? string.Empty,
            partie?.StartingBet ?? 0,
            partie?.Pot ?? 0,
            partie?.CurrentBet ?? 0,
            partie?.CommunityCards ?? new TableCards(),
            _players.Select(j => new PokerPlayerState(
                j.Name,
                j.Chips,
                j is HumanPlayer,
                j.LastAction == Actions.PokerTypeAction.Fold,
                j.LastAction,
                j.Hand,
                partie?.Winners?.Any(g => g.Name == j.Name) == true)).ToList(),
            partie == null || _table.TurnManager == null ? string.Empty : _table.GetPlayerToAct().Name);
    }
}
