using casino.core.Games.Blackjack;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;

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
    /// <param name="humanActionSelector">The callback used to choose a poker action.</param>
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
}
