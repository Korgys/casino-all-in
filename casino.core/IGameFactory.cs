using casino.core.Games.Blackjack;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Slots;

namespace casino.core;

/// <summary>
/// Defines factory methods for creating games.
/// </summary>
public interface IGameFactory
{
    /// <summary>
    /// Creates a game from its name.
    /// </summary>
    /// <param name="gameName">The game name.</param>
    /// <param name="humanActionSelector">The callback used to choose a poker action when <paramref name="gameName"/> is <c>poker</c>.</param>
    /// <param name="continuePlaying">The callback that indicates whether to continue.</param>
    /// <returns>The created game or <see langword="null"/> when the name is unknown.</returns>
    IGame? Create(string gameName, Func<ActionRequest, GameAction> humanActionSelector, Func<bool> continuePlaying);

    /// <summary>
    /// Creates a poker game.
    /// </summary>
    /// <param name="humanActionSelector">The callback used to choose a poker action.</param>
    /// <param name="continuePlaying">The callback that indicates whether to continue.</param>
    /// <param name="setup">Optional poker game setup.</param>
    /// <returns>A poker game instance.</returns>
    IGame CreatePoker(Func<ActionRequest, GameAction> humanActionSelector, Func<bool> continuePlaying, PokerGameSetup? setup = null);

    /// <summary>
    /// Creates a blackjack game.
    /// </summary>
    /// <param name="humanActionSelector">The callback used to choose a blackjack action.</param>
    /// <param name="continuePlaying">The callback that indicates whether to continue.</param>
    /// <returns>A blackjack game instance.</returns>
    IGame CreateBlackjack(Func<BlackjackGameState, BlackjackAction> humanActionSelector, Func<bool> continuePlaying);

    /// <summary>
    /// Creates a slot machine game.
    /// </summary>
    /// <param name="betSelector">The callback used to select the bet amount.</param>
    /// <param name="continuePlaying">The callback that indicates whether to continue.</param>
    /// <returns>A slot machine game instance.</returns>
    IGame CreateSlotMachine(Func<SlotMachineGameState, int> betSelector, Func<bool> continuePlaying);
}
