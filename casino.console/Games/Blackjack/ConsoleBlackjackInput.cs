using casino.console.Games.Commons;
using casino.console.Localization;
using casino.core.Games.Blackjack;

namespace casino.console.Games.Blackjack;

/// <summary>
/// Provides console input helpers for blackjack.
/// </summary>
public static class ConsoleBlackjackInput
{
    /// <summary>
    /// Gets the next player action from console input.
    /// </summary>
    /// <param name="_">The current game state.</param>
    /// <returns>The selected blackjack action.</returns>
    public static BlackjackAction GetPlayerAction(BlackjackGameState _)
    {
        while (true)
        {
            RenderAvailableActions();
            Console.Write(ConsoleText.ActionChoicePrompt);
            var choice = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

            if (ConsoleInputAliases.IsHit(choice) || choice is "z" or "ziehen")
                return BlackjackAction.Hit;

            if (ConsoleInputAliases.IsStand(choice) || choice is "stehen")
                return BlackjackAction.Stand;
        }
    }

    /// <summary>
    /// Asks the player whether to start a new round.
    /// </summary>
    /// <returns><see langword="true"/> when the player wants to continue; otherwise <see langword="false"/>.</returns>
    public static bool AskContinueNewGame()
    {
        Console.Write($"\n{ConsoleText.BlackjackContinuePrompt}");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        return ConsoleInputAliases.IsYes(answer);
    }

    /// <summary>
    /// Renders the list of available blackjack actions.
    /// </summary>
    private static void RenderAvailableActions()
    {
        Console.WriteLine();
        var frameWidth = ConsoleLayout.ResolveContentWidth(46);
        Console.WriteLine("┌" + new string('─', frameWidth) + "┐");
        ConsoleLayout.WriteFramedLine($" {ConsoleText.BlackjackActionsTitle} ", frameWidth, '│', '│');
        ConsoleLayout.WriteFramedLine($" 1. {ConsoleText.BlackjackHit} ", frameWidth, '│', '│');
        ConsoleLayout.WriteFramedLine($" 2. {ConsoleText.BlackjackStand} ", frameWidth, '│', '│');
        Console.WriteLine("└" + new string('─', frameWidth) + "┘");
    }
}
