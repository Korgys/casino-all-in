using casino.core.Common.Utils;
using casino.core.Properties.Languages;

namespace casino.core.Games.Roulette;

public class RouletteGame : GameBase
{
    private static readonly HashSet<int> RedNumbers =
    [
        1, 3, 5, 7, 9,
        12, 14, 16, 18,
        19, 21, 23, 25, 27,
        30, 32, 34, 36
    ];

    private readonly Func<RouletteGameState, RouletteBet> _betSelector;
    private readonly Func<bool> _continuePlaying;
    private readonly IRandom _random;
    private readonly Action<int> _pause;
    private readonly int _startingCredits;
    private readonly int _animationFrames;

    private int _credits;
    private int _totalSpins;
    private int _winningSpins;
    private int _biggestPayout;
    private int? _currentPocket;
    private RouletteBet _currentBet = new(RouletteBetKind.Red, 0);

    public RouletteGame(
        Func<RouletteGameState, RouletteBet> betSelector,
        Func<bool> continuePlaying,
        IRandom? random = null,
        Action<int>? pause = null,
        int startingCredits = 100,
        int animationFrames = 10)
        : base("Roulette")
    {
        ArgumentNullException.ThrowIfNull(betSelector);
        ArgumentNullException.ThrowIfNull(continuePlaying);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(startingCredits);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(animationFrames);

        _betSelector = betSelector;
        _continuePlaying = continuePlaying;
        _random = random ?? new CasinoRandom();
        _pause = pause ?? Thread.Sleep;
        _startingCredits = startingCredits;
        _animationFrames = animationFrames;
    }

    protected override void InitializeGame()
    {
        _credits = _startingCredits;
        _totalSpins = 0;
        _winningSpins = 0;
        _biggestPayout = 0;
        _currentPocket = null;
        _currentBet = new RouletteBet(RouletteBetKind.Red, 0);

        PublishState(BuildState(0, false, false, T("RouletteWelcome")));
    }

    protected override void ExecuteGameLoop()
    {
        while (_credits > 0)
        {
            var requestedBet = _betSelector(BuildState(0, false, false, T("RouletteChooseBet")));
            PlayRound(NormalizeBet(requestedBet));

            if (_credits <= 0 || !_continuePlaying())
                break;
        }

        var finalMessage = _credits > 0
            ? T("RouletteThanks")
            : T("RouletteNoCredits");

        PublishState(BuildState(0, false, true, finalMessage));
        OnGameEnded(T("RouletteGameName"), _credits);
    }

    private void PlayRound(RouletteBet bet)
    {
        _currentBet = bet;
        _credits -= bet.Amount;

        for (var frame = 0; frame < _animationFrames; frame++)
        {
            _currentPocket = _random.Next(37);
            PublishState(BuildState(0, true, false, string.Format(T("RouletteWheelSpinning"), frame + 1, _animationFrames)));
            _pause(80);
        }

        _currentPocket = _random.Next(37);
        _totalSpins++;

        var payout = CalculatePayout(bet, _currentPocket.Value);
        _credits += payout;

        if (payout > 0)
        {
            _winningSpins++;
            _biggestPayout = Math.Max(_biggestPayout, payout);
        }

        PublishState(BuildState(payout, false, true, BuildResultMessage(bet, _currentPocket.Value, payout)));
    }

    private RouletteGameState BuildState(int payout, bool isSpinning, bool isRoundOver, string statusMessage)
    {
        return new RouletteGameState
        {
            Credits = _credits,
            CurrentBet = _currentBet.Amount,
            LastPayout = payout,
            MinBet = _credits > 0 ? 1 : 0,
            MaxBet = _credits > 0 ? Math.Min(20, _credits) : 0,
            TotalSpins = _totalSpins,
            WinningSpins = _winningSpins,
            BiggestPayout = _biggestPayout,
            CurrentPocket = _currentPocket,
            BetKind = _currentBet.Kind,
            SelectedNumber = _currentBet.Number,
            BetSummary = _currentBet.Amount > 0 ? DescribeBet(_currentBet) : "-",
            IsSpinning = isSpinning,
            IsRoundOver = isRoundOver,
            IsWinningBet = payout > 0,
            StatusMessage = statusMessage
        };
    }

    private RouletteBet NormalizeBet(RouletteBet? requestedBet)
    {
        var bet = requestedBet ?? new RouletteBet(RouletteBetKind.Red, 1);
        var amount = Math.Clamp(bet.Amount, 1, Math.Min(20, _credits));

        return bet.Kind switch
        {
            RouletteBetKind.Number => new RouletteBet(RouletteBetKind.Number, amount, Math.Clamp(bet.Number ?? 0, 0, 36)),
            RouletteBetKind.Red => new RouletteBet(RouletteBetKind.Red, amount),
            RouletteBetKind.Black => new RouletteBet(RouletteBetKind.Black, amount),
            RouletteBetKind.Even => new RouletteBet(RouletteBetKind.Even, amount),
            RouletteBetKind.Odd => new RouletteBet(RouletteBetKind.Odd, amount),
            _ => new RouletteBet(RouletteBetKind.Red, amount)
        };
    }

    private int CalculatePayout(RouletteBet bet, int pocket)
    {
        var won = bet.Kind switch
        {
            RouletteBetKind.Number => pocket == bet.Number,
            RouletteBetKind.Red => pocket != 0 && IsRed(pocket),
            RouletteBetKind.Black => pocket != 0 && !IsRed(pocket),
            RouletteBetKind.Even => pocket != 0 && pocket % 2 == 0,
            RouletteBetKind.Odd => pocket % 2 == 1,
            _ => false
        };

        if (!won)
            return 0;

        return bet.Kind == RouletteBetKind.Number ? bet.Amount * 36 : bet.Amount * 2;
    }

    private string BuildResultMessage(RouletteBet bet, int pocket, int payout)
    {
        if (payout <= 0)
            return string.Format(T("RouletteLose"), DescribePocket(pocket));

        return bet.Kind switch
        {
            RouletteBetKind.Number => string.Format(T("RouletteWinNumber"), pocket, payout),
            RouletteBetKind.Red or RouletteBetKind.Black => string.Format(T("RouletteWinColor"), DescribePocket(pocket), payout),
            RouletteBetKind.Even or RouletteBetKind.Odd => string.Format(T("RouletteWinParity"), DescribePocket(pocket), payout),
            _ => string.Format(T("RouletteWinColor"), DescribePocket(pocket), payout)
        };
    }

    private string DescribeBet(RouletteBet bet)
    {
        return bet.Kind switch
        {
            RouletteBetKind.Number => string.Format(T("RouletteBetStraightNumber"), bet.Number ?? 0),
            RouletteBetKind.Red => T("RouletteBetRed"),
            RouletteBetKind.Black => T("RouletteBetBlack"),
            RouletteBetKind.Even => T("RouletteBetEven"),
            RouletteBetKind.Odd => T("RouletteBetOdd"),
            _ => T("RouletteBetRed")
        };
    }

    private string DescribePocket(int pocket)
    {
        if (pocket == 0)
            return string.Format(T("RoulettePocketGreen"), pocket);

        var color = IsRed(pocket) ? T("RouletteColorRed") : T("RouletteColorBlack");
        return string.Format(T("RoulettePocketColored"), pocket, color);
    }

    private static bool IsRed(int pocket) => RedNumbers.Contains(pocket);

    private void PublishState(RouletteGameState state) => OnStateUpdated(state);

    private static string T(string key)
        => Resources.ResourceManager.GetString(key, Resources.Culture) ?? key;
}
