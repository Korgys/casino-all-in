using System.Globalization;
using System.Resources;
using casino.core.Games.Poker;

namespace casino.console.Localization;

internal static class ConsoleText
{
    private static readonly ResourceManager ResourceManager = new("casino.console.Localization.ConsoleResources", typeof(ConsoleText).Assembly);

    private static string Get(string key)
        => ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;

    public static string MainMenuChoice => Get(nameof(MainMenuChoice));
    public static string PressAnyKeyToQuit => Get(nameof(PressAnyKeyToQuit));
    public static string MenuPoker => Get(nameof(MenuPoker));
    public static string MenuBlackjack => Get(nameof(MenuBlackjack));
    public static string MenuSlotMachine => Get(nameof(MenuSlotMachine));
    public static string MenuLanguages => Get(nameof(MenuLanguages));
    public static string MenuQuit => Get(nameof(MenuQuit));
    public static string MenuBack => Get(nameof(MenuBack));
    public static string LanguageMenuTitle => Get(nameof(LanguageMenuTitle));
    public static string LanguageMenuChoice => Get(nameof(LanguageMenuChoice));
    public static string CurrentLanguageLabel => Get(nameof(CurrentLanguageLabel));
    public static string LanguageFrench => Get(nameof(LanguageFrench));
    public static string LanguageEnglish => Get(nameof(LanguageEnglish));
    public static string LanguageGerman => Get(nameof(LanguageGerman));
    public static string LanguageSpanish => Get(nameof(LanguageSpanish));
    public static string LanguageJapanese => Get(nameof(LanguageJapanese));
    public static string LanguageSimplifiedChinese => Get(nameof(LanguageSimplifiedChinese));
    public static string LanguageRussian => Get(nameof(LanguageRussian));
    public static string PokerSettingsTitle => Get(nameof(PokerSettingsTitle));
    public static string ContinuePokerPrompt => Get(nameof(ContinuePokerPrompt));
    public static string ActionChoicePrompt => Get(nameof(ActionChoicePrompt));
    public static string RaiseTargetPrompt => Get(nameof(RaiseTargetPrompt));
    public static string InvalidNumberInput => Get(nameof(InvalidNumberInput));
    public static string TypeNumberMenuHelp => Get(nameof(TypeNumberMenuHelp));
    public static string InvalidDifficulty => Get(nameof(InvalidDifficulty));
    public static string BetLabel => Get(nameof(BetLabel));
    public static string PotLabel => Get(nameof(PotLabel));
    public static string CurrentBetLabel => Get(nameof(CurrentBetLabel));
    public static string TableLabel => Get(nameof(TableLabel));
    public static string TotalLabel => Get(nameof(TotalLabel));
    public static string BlackjackTitle => Get(nameof(BlackjackTitle));
    public static string BlackjackDealer => Get(nameof(BlackjackDealer));
    public static string BlackjackYou => Get(nameof(BlackjackYou));
    public static string BlackjackStats => Get(nameof(BlackjackStats));
    public static string BlackjackWins => Get(nameof(BlackjackWins));
    public static string BlackjackLosses => Get(nameof(BlackjackLosses));
    public static string BlackjackPushes => Get(nameof(BlackjackPushes));
    public static string BlackjackWinAnimation => Get(nameof(BlackjackWinAnimation));
    public static string BlackjackContinuePrompt => Get(nameof(BlackjackContinuePrompt));
    public static string BlackjackActionsTitle => Get(nameof(BlackjackActionsTitle));
    public static string BlackjackHit => Get(nameof(BlackjackHit));
    public static string BlackjackStand => Get(nameof(BlackjackStand));
    public static string SlotContinuePrompt => Get(nameof(SlotContinuePrompt));
    public static string SlotMachineHeader => Get(nameof(SlotMachineHeader));
    public static string SlotPanelTitle => Get(nameof(SlotPanelTitle));
    public static string SlotCredits => Get(nameof(SlotCredits));
    public static string SlotMinMaxBet => Get(nameof(SlotMinMaxBet));
    public static string SlotLastWin => Get(nameof(SlotLastWin));
    public static string SlotSpins => Get(nameof(SlotSpins));
    public static string SlotWinningSpins => Get(nameof(SlotWinningSpins));
    public static string SlotBestWin => Get(nameof(SlotBestWin));
    public static string SlotJackpotAnimation => Get(nameof(SlotJackpotAnimation));
    public static string SlotWinAnimation => Get(nameof(SlotWinAnimation));
    public static string WinnerTag => Get(nameof(WinnerTag));
    public static string PokerDifficultyBeginner => Get(nameof(PokerDifficultyBeginner));
    public static string PokerDifficultyVeryEasy => Get(nameof(PokerDifficultyVeryEasy));
    public static string PokerDifficultyEasy => Get(nameof(PokerDifficultyEasy));
    public static string PokerDifficultyMedium => Get(nameof(PokerDifficultyMedium));
    public static string PokerDifficultyHard => Get(nameof(PokerDifficultyHard));
    public static string PokerDifficultyVeryHard => Get(nameof(PokerDifficultyVeryHard));

    public static string InitialChipsPrompt(int min, int max) => string.Format(Get(nameof(InitialChipsPrompt)), min, max);
    public static string PlayerCountPrompt(int min, int max) => string.Format(Get(nameof(PlayerCountPrompt)), min, max);
    public static string DifficultyPrompt(string defaultValue) => string.Format(Get(nameof(DifficultyPrompt)), defaultValue);
    public static string RangeError(int min, int max) => string.Format(Get(nameof(RangeError)), min, max);
    public static string SlotBetPrompt(int bet) => string.Format(Get(nameof(SlotBetPrompt)), bet);
    public static string RaiseTargetPromptWithConstraints(int currentBet, int contribution, int maxAllowed) =>
        string.Format(Get(nameof(RaiseTargetPromptWithConstraints)), currentBet, contribution, maxAllowed);
    public static string RaiseAmountUnavailable(int currentBet, int maxAllowed) =>
        string.Format(Get(nameof(RaiseAmountUnavailable)), currentBet, maxAllowed);
    public static string ActionUnavailable(int choice) => string.Format(Get(nameof(ActionUnavailable)), choice);
    public static string InputCanceledUsingDefault(string defaultValue) => string.Format(Get(nameof(InputCanceledUsingDefault)), defaultValue);

    public static string PokerDifficultyLabel(PokerDifficulty difficulty) => difficulty switch
    {
        PokerDifficulty.Beginner => PokerDifficultyBeginner,
        PokerDifficulty.VeryEasy => PokerDifficultyVeryEasy,
        PokerDifficulty.Easy => PokerDifficultyEasy,
        PokerDifficulty.Medium => PokerDifficultyMedium,
        PokerDifficulty.Hard => PokerDifficultyHard,
        PokerDifficulty.VeryHard => PokerDifficultyVeryHard,
        _ => difficulty.ToString()
    };
}
