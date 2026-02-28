using casino.core;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Parties;

namespace casino.core.Games.Poker;

public class PokerGame : GameBase
{
    private readonly Func<bool> _continuePlaying;
    private readonly Func<RequeteAction, Actions.ActionJeu> _humanActionSelector;
    private readonly Func<IDeck> _deckFactory;
    private readonly TablePoker _table;
    private readonly List<PlayerHumain> _playersHumain;
    private readonly List<Player> _players;

    public PokerGame(
        IEnumerable<Player> Players,
        Func<IDeck> deckFactory,
        Func<RequeteAction, Actions.ActionJeu> humanActionSelector,
        Func<bool> continuePlaying)
        : base("Poker")
    {
        _players = Players.ToList();
        _playersHumain = _players.OfType<PlayerHumain>().ToList();
        _deckFactory = deckFactory;
        _humanActionSelector = humanActionSelector;
        _continuePlaying = continuePlaying;
        _table = new TablePoker { Name = "Table Poker" };
    }

    protected override void InitializeGame()
    {
        OnStateUpdated(CreerEtatTable());
    }

    protected override void ExecuteGameLoop()
    {
        while (_playersHumain.Any(j => j.Chips > 0))
        {
            var deck = _deckFactory();
            _table.DemarrerPartie(_players, deck);

            OnPhaseAdvanced(_table.Partie.Phase.ToString());
            OnPotUpdated(_table.Partie.Pot, _table.Partie.CurrentBet);
            OnStateUpdated(CreerEtatTable());

            JouerPartie();

            var winners = _table.Partie.Winners;

            var gagnantsLabel = (winners is null || winners.Count == 0)
                ? _players.First().Name
                : string.Join(", ", winners.Select(g => g.Name));

            OnGameEnded(gagnantsLabel, _table.Partie.Pot);
            OnStateUpdated(CreerEtatTable());

            // Si plus aucun humain n'a de jetons, fin du jeu.
            if (!_playersHumain.Any(j => j.Chips > 0))
                break;

            // Si on ne veut pas continuer, fin du jeu.
            if (!_continuePlaying())
                break;
        }
    }

    private void JouerPartie()
    {
        while (_table.Partie.EnCours())
        {
            OnStateUpdated(CreerEtatTable());

            var phaseAvantAction = _table.Partie.Phase;
            var PlayerActuel = _table.ObtenirPlayerQuiDoitJouer();
            var actionsPossibles = _table.ObtenirActionsPossibles(PlayerActuel);

            if (PlayerActuel is PlayerHumain humain)
            {
                var contexte = new RequeteAction(
                    humain.Name,
                    actionsPossibles,
                    _table.Partie.StartingBet,
                    _table.Partie.CurrentBet,
                    _table.Partie.Pot,
                    CreerEtatTable());

                var action = _humanActionSelector(contexte);
                _table.TraiterActionPlayer(humain, action);
            }
            else if (PlayerActuel is PlayerOrdi ordi)
            {
                Thread.Sleep(Random.Shared.Next(500, 1500));
                var contexte = new ContexteDeJeu(_table.Partie, ordi, actionsPossibles);
                var action = ordi.Strategie.ProposerAction(contexte);
                _table.TraiterActionPlayer(ordi, action);
            }

            if (phaseAvantAction != _table.Partie.Phase)
                OnPhaseAdvanced(_table.Partie.Phase.ToString());

            OnPotUpdated(_table.Partie.Pot, _table.Partie.CurrentBet);
        }
    }

    protected override void ResolveGame() { }
    protected override void CleanupGame() { }

    private PokerGameState CreerEtatTable()
    {
        var partie = _table.Partie;

        return new PokerGameState(
            partie?.Phase.ToString() ?? string.Empty,
            partie?.StartingBet ?? 0,
            partie?.Pot ?? 0,
            partie?.CurrentBet ?? 0,
            partie?.CommunityCards ?? new TableCards(),
            _players.Select(j => new PokerPlayerState(
                j.Name,
                j.Chips,
                j is PlayerHumain,
                j.LastAction == Actions.TypeActionJeu.SeCoucher,
                j.LastAction,
                j.Hand,
                partie?.Winners?.Any(g => g.Name == j.Name) == true)).ToList(),
            partie == null || _table.GestionnaireDeTour == null ? string.Empty : _table.ObtenirPlayerQuiDoitJouer().Name);
    }
}
