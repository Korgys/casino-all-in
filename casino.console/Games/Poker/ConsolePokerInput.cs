using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using ActionModel = casino.core.Games.Poker.Actions.GameAction;

namespace casino.console.Games.Poker;

/// <summary>
/// Class responsible for managing user input for the poker game in the console.
/// </summary>
public class ConsolePokerInput
{
    public static ActionModel GetPlayerAction(ActionRequest request)
    {
        var state = (PokerGameState)request.TableState;
        var player = state.Players.First(j => j.Name == request.PlayerName);

        var choice = ReadActionChoice(request.AvailableActions, request.MinimumBet);

        return choice switch
        {
            PokerTypeAction.Bet => new ActionModel(choice, request.MinimumBet),
            PokerTypeAction.Raise => new ActionModel(choice, ReadRaiseAmount(player.Chips, state.CurrentBet, player.Contribution)),
            _ => new ActionModel(choice, 0),
        };
    }

    public static bool AskContinueNewGame()
    {
        Console.Write("\nVoulez-vous continuer une nouvelle partie ? (o/n) : ");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        return answer is "o" or "oui" or "y" or "yes";
    }

    private static PokerTypeAction ReadActionChoice(IReadOnlyList<PokerTypeAction> availableActions, int minimumBet)
    {
        while (true)
        {
            ConsolePokerRenderer.RenderAvailableActions(availableActions, minimumBet);

            Console.Write("Quel est votre choix ? ");
            if (!int.TryParse(Console.ReadLine(), out var raw))
                continue;

            if (availableActions.Any(a => (int)a == raw))
                return (PokerTypeAction)raw;
        }
    }

    private static int ReadRaiseAmount(int maxChips, int actualBet, int currentContribution)
    {
        while (true)
        {
            Console.Write("Saisissez la mise totale cible (pas l'incrément) : ");
            if (!int.TryParse(Console.ReadLine(), out var amount))
                continue;

            if (amount > actualBet && amount - currentContribution <= maxChips)
                return amount;
        }
    }
}
