using System.Linq;
using casino.console.Games.Blackjack;
using casino.console.Games.Poker;
using casino.core;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Players.Strategies;

namespace casino.console.Games;

/// <summary>
/// Provides a factory for creating console-based game instances that implement the IGame interface.
/// </summary>
public class ConsoleGameFactory : IGameFactory
{
    public IGame? Create(string gameName, Func<ActionRequest, GameAction> humanActionSelector, Func<bool> continuePlaying)
    {
        return gameName.ToLower() switch
        {
            "poker" => CreatePoker(humanActionSelector, continuePlaying),
            "blackjack" => CreateBlackjack(ConsoleBlackjackInput.GetPlayerAction, ConsoleBlackjackInput.AskContinueNewGame),
            _ => null
        };
    }

    public IGame CreatePoker(
        Func<ActionRequest, GameAction> humanActionSelector,
        Func<bool> continuePlaying,
        PokerGameSetup? setup = null)
    {
        var configuration = setup ?? PokerGameSetup.CreateDefault();
        var players = new List<Player>
        {
            new HumanPlayer("Player", configuration.InitialChips)
        };

        for (var index = 0; index < configuration.Opponents.Count; index++)
        {
            var opponent = configuration.Opponents[index];
            players.Add(new ComputerPlayer(
                $"Ordi {index + 1} ({opponent.Label})",
                configuration.InitialChips,
                CreateStrategy(opponent.Difficulty)));
        }

        return new PokerGame(players, () => new Deck(), humanActionSelector, continuePlaying, new ConsoleWaitStrategy());
    }

    public IGame CreateBlackjack(Func<BlackjackGameState, BlackjackAction> humanActionSelector, Func<bool> continuePlaying)
    {
        return new BlackjackGame(humanActionSelector, continuePlaying);
    }

    private static IPlayerStrategy CreateStrategy(PokerDifficulty difficulty)
    {
        return difficulty switch
        {
            PokerDifficulty.Easy => new RandomStrategy(),
            PokerDifficulty.Medium => new ConservativeStrategy(),
            PokerDifficulty.Hard => new OpportunisticStrategy(),
            PokerDifficulty.Expert => new AggressiveStrategy(),
            _ => new RandomStrategy()
        };
    }
}
