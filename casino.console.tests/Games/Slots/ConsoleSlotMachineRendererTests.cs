using casino.console.Games.Slots;
using casino.core.Games.Slots;

namespace casino.console.tests.Games.Slots;

[TestClass]
public class ConsoleSlotMachineRendererTests
{
    [TestMethod]
    public void RenderTable_ShowsMachineAndAnimation_ForJackpotState()
    {
        ConsoleSlotMachineRenderer.Pause = _ => { };

        var state = new SlotMachineGameState
        {
            Reels = [SlotSymbol.Seven, SlotSymbol.Seven, SlotSymbol.Seven],
            Credits = 120,
            CurrentBet = 5,
            LastPayout = 100,
            TotalSpins = 1,
            WinningSpins = 1,
            BiggestPayout = 100,
            IsRoundOver = true,
            IsJackpot = true,
            StatusMessage = "💥 JACKPOT ! Triple 7 ! Vous gagnez 100 crédits."
        };

        ConsoleSlotMachineRenderer.RenderTable(state);
    }
}
