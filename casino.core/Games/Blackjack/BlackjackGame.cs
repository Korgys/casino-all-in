using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Blackjack;

public class BlackjackGame : GameBase
{
    private readonly Func<IDeck> _deckFactory;
    private readonly Func<BlackjackGameState, BlackjackAction> _playerActionSelector;
    private readonly Func<bool> _continuePlaying;

    private IDeck _deck = null!;
    private readonly List<Card> _playerCards = new();
    private readonly List<Card> _dealerCards = new();

    public BlackjackGame(
        Func<BlackjackGameState, BlackjackAction> playerActionSelector,
        Func<bool> continuePlaying,
        Func<IDeck>? deckFactory = null)
        : base("Blackjack")
    {
        _playerActionSelector = playerActionSelector;
        _continuePlaying = continuePlaying;
        _deckFactory = deckFactory ?? (() => new Deck());
    }

    protected override void InitializeGame()
    {
        _deck = _deckFactory();
        _deck.Shuffle();
    }

    protected override void ExecuteGameLoop()
    {
        var playAgain = true;

        while (playAgain)
        {
            PlayRound();
            playAgain = _continuePlaying();
        }
    }

    private void PlayRound()
    {
        _playerCards.Clear();
        _dealerCards.Clear();

        DealInitialCards();
        PublishState("Votre tour.", isRoundOver: false, hideDealerHoleCard: true);

        while (BlackjackScoreCalculator.Calculate(_playerCards) < 21)
        {
            var action = _playerActionSelector(BuildState("Choisissez votre action (hit/stand).", false, true));
            if (action == BlackjackAction.Stand)
                break;

            _playerCards.Add(_deck.DrawCard());
            PublishState("Carte tirée.", isRoundOver: false, hideDealerHoleCard: true);
        }

        var playerScore = BlackjackScoreCalculator.Calculate(_playerCards);
        if (playerScore > 21)
        {
            EndRound("Le croupier");
            return;
        }

        while (BlackjackScoreCalculator.Calculate(_dealerCards) < 17)
            _dealerCards.Add(_deck.DrawCard());

        EndRound(ResolveWinner());
    }

    private string ResolveWinner()
    {
        var playerScore = BlackjackScoreCalculator.Calculate(_playerCards);
        var dealerScore = BlackjackScoreCalculator.Calculate(_dealerCards);

        if (dealerScore > 21)
            return "Player";

        if (playerScore > dealerScore)
            return "Player";

        if (dealerScore > playerScore)
            return "Le croupier";

        return "Égalité";
    }

    private void EndRound(string winnerName)
    {
        var status = winnerName == "Égalité" ? "Push : égalité parfaite." : $"Gagnant: {winnerName}";
        PublishState(status, isRoundOver: true, hideDealerHoleCard: false);
        OnGameEnded(winnerName, 0);
    }

    private void DealInitialCards()
    {
        _playerCards.Add(_deck.DrawCard());
        _dealerCards.Add(_deck.DrawCard());
        _playerCards.Add(_deck.DrawCard());
        _dealerCards.Add(_deck.DrawCard());
    }

    private BlackjackGameState BuildState(string status, bool isRoundOver, bool hideDealerHoleCard)
    {
        return new BlackjackGameState
        {
            PlayerCards = _playerCards.ToList(),
            DealerCards = _dealerCards.ToList(),
            IsDealerHoleCardHidden = hideDealerHoleCard,
            IsRoundOver = isRoundOver,
            StatusMessage = status
        };
    }

    private void PublishState(string status, bool isRoundOver, bool hideDealerHoleCard)
        => OnStateUpdated(BuildState(status, isRoundOver, hideDealerHoleCard));
}
