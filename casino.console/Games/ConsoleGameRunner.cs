using casino.console.Games.Blackjack;
using casino.console.Games.Poker;
using casino.console.Games.Roulette;
using casino.console.Games.Slots;
using casino.core;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker;
using casino.core.Games.Roulette;
using casino.core.Games.Slots;

namespace casino.console.Games;

/// <summary>
/// Runs a game and connects core state updates to console renderers.
/// </summary>
public sealed class ConsoleGameRunner
{
    private readonly Action<PokerGameState> renderPokerTable;
    private readonly Action<BlackjackGameState> renderBlackjackTable;
    private readonly Action<SlotMachineGameState> renderSlotMachineTable;
    private readonly Action<RouletteGameState> renderRouletteTable;
    private readonly Action requestPokerFullRefresh;

    public ConsoleGameRunner()
        : this(
            new ConsolePokerRenderer().RenderTable,
            ConsoleBlackjackRenderer.RenderTable,
            ConsoleSlotMachineRenderer.RenderTable,
            ConsoleRouletteRenderer.RenderTable,
            ConsolePokerRenderer.RequestFullRefresh)
    {
    }

    internal ConsoleGameRunner(
        Action<PokerGameState> renderPokerTable,
        Action<BlackjackGameState> renderBlackjackTable,
        Action<SlotMachineGameState> renderSlotMachineTable,
        Action<RouletteGameState> renderRouletteTable,
        Action requestPokerFullRefresh)
    {
        this.renderPokerTable = renderPokerTable;
        this.renderBlackjackTable = renderBlackjackTable;
        this.renderSlotMachineTable = renderSlotMachineTable;
        this.renderRouletteTable = renderRouletteTable;
        this.requestPokerFullRefresh = requestPokerFullRefresh;
    }

    public void Run(IGame game)
    {
        var refreshPokerTableAfterRoundEnd = false;

        game.GameEnded += (_, _) => refreshPokerTableAfterRoundEnd = true;

        game.StateUpdated += (_, e) =>
        {
            if (e.State is PokerGameState state)
            {
                if (refreshPokerTableAfterRoundEnd)
                {
                    requestPokerFullRefresh();
                    refreshPokerTableAfterRoundEnd = false;
                }

                renderPokerTable(state);
            }

            if (e.State is BlackjackGameState blackjackState)
            {
                renderBlackjackTable(blackjackState);
            }

            if (e.State is SlotMachineGameState slotMachineState)
            {
                renderSlotMachineTable(slotMachineState);
            }

            if (e.State is RouletteGameState rouletteState)
            {
                renderRouletteTable(rouletteState);
            }
        };

        game.Run();
    }
}
