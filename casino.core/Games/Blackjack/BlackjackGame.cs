using casino.core.Games.Poker.Cards;
using casino.core.Properties.Languages;

namespace casino.core.Games.Blackjack;

public class BlackjackGame : GameBase
{
    private readonly Func<IDeck> _deckFactory;
    private readonly Func<BlackjackGameState, BlackjackAction> _playerActionSelector;
    private readonly Func<bool> _continuePlaying;

    private IDeck _deck = null!;
    private readonly List<Card> _playerCards = new();
    private readonly List<Card> _dealerCards = new();
    private int _playerWins;
    private int _dealerWins;
    private int _pushes;

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
        _playerWins = 0;
        _dealerWins = 0;
        _pushes = 0;
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
        PublishState(T("BlackjackPlayerTurn"), isRoundOver: false, hideDealerHoleCard: true, BlackjackRoundOutcome.InProgress);

        while (BlackjackScoreCalculator.Calculate(_playerCards) < 21)
        {
            var action = _playerActionSelector(BuildState(T("BlackjackChooseAction"), false, true, BlackjackRoundOutcome.InProgress));
            if (action == BlackjackAction.Stand)
                break;

            _playerCards.Add(_deck.DrawCard());
            PublishState(T("BlackjackCardDrawn"), isRoundOver: false, hideDealerHoleCard: true, BlackjackRoundOutcome.InProgress);
        }

        var playerScore = BlackjackScoreCalculator.Calculate(_playerCards);
        if (playerScore > 21)
        {
            EndRound(T("BlackjackDealer"), BlackjackRoundOutcome.DealerWin);
            return;
        }

        while (BlackjackScoreCalculator.Calculate(_dealerCards) < 17)
            _dealerCards.Add(_deck.DrawCard());

        var outcome = ResolveWinner();
        EndRound(outcome.WinnerName, outcome.RoundOutcome);
    }

    private (string WinnerName, BlackjackRoundOutcome RoundOutcome) ResolveWinner()
    {
        var playerScore = BlackjackScoreCalculator.Calculate(_playerCards);
        var dealerScore = BlackjackScoreCalculator.Calculate(_dealerCards);

        if (dealerScore > 21 || playerScore > dealerScore)
            return (T("BlackjackPlayer"), BlackjackRoundOutcome.PlayerWin);

        if (dealerScore > playerScore)
            return (T("BlackjackDealer"), BlackjackRoundOutcome.DealerWin);

        return (T("BlackjackPushWinner"), BlackjackRoundOutcome.Push);
    }

    private void EndRound(string winnerName, BlackjackRoundOutcome roundOutcome)
    {
        switch (roundOutcome)
        {
            case BlackjackRoundOutcome.PlayerWin:
                _playerWins++;
                break;
            case BlackjackRoundOutcome.DealerWin:
                _dealerWins++;
                break;
            case BlackjackRoundOutcome.Push:
                _pushes++;
                break;
        }

        var status = roundOutcome switch
        {
            BlackjackRoundOutcome.PlayerWin => T("BlackjackRoundWonByPlayer"),
            BlackjackRoundOutcome.DealerWin => T("BlackjackRoundWonByDealer"),
            BlackjackRoundOutcome.Push => T("BlackjackRoundPush"),
            _ => string.Empty
        };

        PublishState(status, isRoundOver: true, hideDealerHoleCard: false, roundOutcome);
        OnGameEnded(winnerName, 0);
    }

    private void DealInitialCards()
    {
        _playerCards.Add(_deck.DrawCard());
        _dealerCards.Add(_deck.DrawCard());
        _playerCards.Add(_deck.DrawCard());
        _dealerCards.Add(_deck.DrawCard());
    }

    private BlackjackGameState BuildState(string status, bool isRoundOver, bool hideDealerHoleCard, BlackjackRoundOutcome roundOutcome)
    {
        return new BlackjackGameState
        {
            PlayerCards = _playerCards.ToList(),
            DealerCards = _dealerCards.ToList(),
            IsDealerHoleCardHidden = hideDealerHoleCard,
            IsRoundOver = isRoundOver,
            StatusMessage = status,
            RoundOutcome = roundOutcome,
            PlayerWins = _playerWins,
            DealerWins = _dealerWins,
            Pushes = _pushes
        };
    }

    private void PublishState(string status, bool isRoundOver, bool hideDealerHoleCard, BlackjackRoundOutcome roundOutcome)
        => OnStateUpdated(BuildState(status, isRoundOver, hideDealerHoleCard, roundOutcome));

    private static string T(string key)
        => Resources.ResourceManager.GetString(key, Resources.Culture) ?? key;
}
