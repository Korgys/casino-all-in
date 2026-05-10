using casino.console.Games;
using casino.console.Games.Blackjack;
using casino.console.Games.Poker;
using casino.console.Games.Roulette;
using casino.console.Games.Slots;
using casino.core;

namespace casino.console.Cli;

internal static class ConsoleGameBuilder
{
    public static IGame Create(ConsoleGameFactory factory, CasinoCliCommand command)
    {
        return command.Game switch
        {
            CasinoGameKind.Poker => factory.CreatePoker(
                ConsolePokerInput.GetPlayerAction,
                ConsolePokerInput.AskContinueNewGame,
                command.PokerSetup),
            CasinoGameKind.Blackjack => factory.CreateBlackjack(
                ConsoleBlackjackInput.GetPlayerAction,
                ConsoleBlackjackInput.AskContinueNewGame),
            CasinoGameKind.SlotMachine => factory.CreateSlotMachine(
                ConsoleSlotMachineInput.GetBet,
                ConsoleSlotMachineInput.AskContinueNewGame),
            CasinoGameKind.Roulette => factory.CreateRoulette(
                ConsoleRouletteInput.GetBet,
                ConsoleRouletteInput.AskContinueNewGame),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command.Game, "Unknown game kind.")
        };
    }
}
