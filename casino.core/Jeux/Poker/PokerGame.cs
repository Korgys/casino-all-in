using casino.core;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Parties;

namespace casino.core.Jeux.Poker;

public class PokerGame : GameBase
{
    private readonly Func<bool> _continuePlaying;
    private readonly Func<RequeteAction, Actions.ActionJeu> _humanActionSelector;
    private readonly Func<IDeck> _deckFactory;
    private readonly TablePoker _table;
    private readonly List<JoueurHumain> _joueursHumain;
    private readonly List<Joueur> _joueurs;

    public PokerGame(
        IEnumerable<Joueur> joueurs,
        Func<IDeck> deckFactory,
        Func<RequeteAction, Actions.ActionJeu> humanActionSelector,
        Func<bool> continuePlaying)
        : base("Poker")
    {
        _joueurs = joueurs.ToList();
        _joueursHumain = _joueurs.OfType<JoueurHumain>().ToList();
        _deckFactory = deckFactory;
        _humanActionSelector = humanActionSelector;
        _continuePlaying = continuePlaying;
        _table = new TablePoker { Nom = "Table Poker" };
    }

    protected override void InitializeGame()
    {
        OnStateUpdated(CreerEtatTable());
    }

    protected override void ExecuteGameLoop()
    {
        while (_joueursHumain.Any(j => j.Jetons > 0))
        {
            var deck = _deckFactory();
            _table.DemarrerPartie(_joueurs, deck);

            OnPhaseAdvanced(_table.Partie.Phase.ToString());
            OnPotUpdated(_table.Partie.Pot, _table.Partie.MiseActuelle);
            OnStateUpdated(CreerEtatTable());

            JouerPartie();

            var gagnants = _table.Partie.Gagnants;

            var gagnantsLabel = (gagnants is null || gagnants.Count == 0)
                ? _joueurs.First().Nom
                : string.Join(", ", gagnants.Select(g => g.Nom));

            OnGameEnded(gagnantsLabel, _table.Partie.Pot);
            OnStateUpdated(CreerEtatTable());

            // Si plus aucun humain n'a de jetons, fin du jeu.
            if (!_joueursHumain.Any(j => j.Jetons > 0))
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
            var joueurActuel = _table.ObtenirJoueurQuiDoitJouer();
            var actionsPossibles = _table.ObtenirActionsPossibles(joueurActuel);

            if (joueurActuel is JoueurHumain humain)
            {
                var contexte = new RequeteAction(
                    humain.Nom,
                    actionsPossibles,
                    _table.Partie.MiseDeDepart,
                    _table.Partie.MiseActuelle,
                    _table.Partie.Pot,
                    CreerEtatTable());

                var action = _humanActionSelector(contexte);
                _table.TraiterActionJoueur(humain, action);
            }
            else if (joueurActuel is JoueurOrdi ordi)
            {
                Thread.Sleep(Random.Shared.Next(500, 1500));
                var contexte = new ContexteDeJeu(_table.Partie, ordi, actionsPossibles);
                var action = ordi.Strategie.ProposerAction(contexte);
                _table.TraiterActionJoueur(ordi, action);
            }

            if (phaseAvantAction != _table.Partie.Phase)
                OnPhaseAdvanced(_table.Partie.Phase.ToString());

            OnPotUpdated(_table.Partie.Pot, _table.Partie.MiseActuelle);
        }
    }

    protected override void ResolveGame() { }
    protected override void CleanupGame() { }

    private PokerGameState CreerEtatTable()
    {
        var partie = _table.Partie;

        return new PokerGameState(
            partie?.Phase.ToString() ?? string.Empty,
            partie?.MiseDeDepart ?? 0,
            partie?.Pot ?? 0,
            partie?.MiseActuelle ?? 0,
            partie?.CartesCommunes ?? new CartesCommunes(),
            _joueurs.Select(j => new PokerPlayerState(
                j.Nom,
                j.Jetons,
                j is JoueurHumain,
                j.DerniereAction == Actions.TypeActionJeu.SeCoucher,
                j.DerniereAction,
                j.Main,
                partie?.Gagnants?.Any(g => g.Nom == j.Nom) == true)).ToList(),
            partie == null ? string.Empty : _table.ObtenirJoueurQuiDoitJouer().Nom);
    }
}
