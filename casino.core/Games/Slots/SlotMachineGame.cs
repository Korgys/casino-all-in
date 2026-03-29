using casino.core.Common.Utils;
using casino.core.Properties.Languages;

namespace casino.core.Games.Slots;

public class SlotMachineGame : GameBase
{
    private static readonly SlotSymbol[] Symbols =
    [
        SlotSymbol.Cherry,
        SlotSymbol.Lemon,
        SlotSymbol.Bell,
        SlotSymbol.Diamond,
        SlotSymbol.Star,
        SlotSymbol.Seven,
        SlotSymbol.Bar
    ];

    private readonly Func<SlotMachineGameState, int> _betSelector;
    private readonly Func<bool> _continuePlaying;
    private readonly IRandom _random;
    private readonly Action<int> _pause;
    private readonly int _startingCredits;
    private readonly int _animationFrames;

    private int _credits;
    private int _totalSpins;
    private int _winningSpins;
    private int _biggestPayout;
    private IReadOnlyList<SlotSymbol> _reels = [SlotSymbol.Cherry, SlotSymbol.Seven, SlotSymbol.Bar];

    public SlotMachineGame(
        Func<SlotMachineGameState, int> betSelector,
        Func<bool> continuePlaying,
        IRandom? random = null,
        Action<int>? pause = null,
        int startingCredits = 100,
        int animationFrames = 12)
        : base("Slot Machine")
    {
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
        _reels = [SlotSymbol.Cherry, SlotSymbol.Seven, SlotSymbol.Bar];

        PublishState(BuildState(0, 0, false, false, T("SlotWelcome")));
    }

    protected override void ExecuteGameLoop()
    {
        while (_credits > 0)
        {
            var state = BuildState(0, 0, false, false, T("SlotChooseBet"));
            var bet = NormalizeBet(_betSelector(state));

            PlaySpin(bet);

            if (_credits <= 0)
                break;

            if (!_continuePlaying())
                break;
        }

        var finalMessage = _credits > 0
            ? T("SlotThanks")
            : T("SlotNoCredits");

        PublishState(BuildState(0, 0, false, true, finalMessage));
        OnGameEnded(T("SlotGameName"), _credits);
    }

    private void PlaySpin(int bet)
    {
        _credits -= bet;

        for (var frame = 0; frame < _animationFrames; frame++)
        {
            _reels = GenerateSpin();
            PublishState(BuildState(bet, 0, true, false, string.Format(T("SlotReelsSpinning"), frame + 1, _animationFrames)));
            _pause(80);
        }

        _reels = GenerateSpin();
        _totalSpins++;

        var payout = SlotMachinePayoutCalculator.Calculate(_reels, bet);
        _credits += payout;

        if (payout > 0)
        {
            _winningSpins++;
            _biggestPayout = Math.Max(_biggestPayout, payout);
        }

        var jackpot = _reels.All(symbol => symbol == SlotSymbol.Seven);
        PublishState(BuildState(bet, payout, false, true, BuildResultMessage(payout, bet, jackpot)));
    }

    private SlotMachineGameState BuildState(int currentBet, int payout, bool isSpinning, bool isRoundOver, string statusMessage)
    {
        return new SlotMachineGameState
        {
            Reels = _reels.ToList(),
            Credits = _credits,
            CurrentBet = currentBet,
            LastPayout = payout,
            MinBet = _credits > 0 ? 1 : 0,
            MaxBet = _credits > 0 ? Math.Min(10, _credits) : 0,
            TotalSpins = _totalSpins,
            WinningSpins = _winningSpins,
            BiggestPayout = _biggestPayout,
            IsSpinning = isSpinning,
            IsRoundOver = isRoundOver,
            IsJackpot = _reels.All(symbol => symbol == SlotSymbol.Seven),
            StatusMessage = statusMessage
        };
    }

    private IReadOnlyList<SlotSymbol> GenerateSpin()
        => [NextSymbol(), NextSymbol(), NextSymbol()];

    private SlotSymbol NextSymbol() => Symbols[_random.Next(Symbols.Length)];

    private int NormalizeBet(int requestedBet)
    {
        var maxBet = Math.Min(10, _credits);
        return Math.Clamp(requestedBet, 1, maxBet);
    }


    private void PublishState(SlotMachineGameState state)
        => OnStateUpdated(state);

    private static string BuildResultMessage(int payout, int bet, bool jackpot)
    {
        if (jackpot)
            return string.Format(T("SlotJackpotWin"), payout);

        if (payout >= bet * 6)
            return string.Format(T("SlotSuperComboWin"), payout);

        if (payout > 0)
            return string.Format(T("SlotRegularWin"), payout);

        return T("SlotNoWin");
    }

    private static string T(string key)
        => Resources.ResourceManager.GetString(key, Resources.Culture) ?? key;
}
