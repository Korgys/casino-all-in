using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using ActionModel = casino.core.Games.Poker.Actions.ActionJeu;

namespace casino.console.Games.Poker;

/// <summary>
/// Class responsible for managing user input for the poker game in the console.
/// </summary>
public class ConsolePokerInput
{
    public static ActionModel GetPlayerAction(RequeteAction request)
    {
        var state = (PokerGameState)request.TableState;
        var player = state.Players.First(j => j.Name == request.PlayerName);

        var choice = ReadActionChoice(request.ActionsPossibles, request.MinimumBet);

        return choice switch
        {
            TypeActionJeu.Miser => new ActionModel(choice, request.MinimumBet),
            TypeActionJeu.Relancer => new ActionModel(choice, ReadRaiseAmount(player.Chips, state.CurrentBet)),
            _ => new ActionModel(choice, 0),
        };
    }

    public static bool AskContinueNewGame()
    {
        Console.Write("\nVoulez-vous continuer une nouvelle partie ? (o/n) : ");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        return answer is "o" or "oui" or "y" or "yes";
    }

    private static TypeActionJeu ReadActionChoice(IReadOnlyList<TypeActionJeu> availableActions, int minimumBet)
    {
        while (true)
        {
            ConsolePokerRenderer.RenderAvailableActions(availableActions, minimumBet);

            Console.Write("Quel est votre choix ? ");
            if (!int.TryParse(Console.ReadLine(), out var raw))
                continue;

            if (availableActions.Any(a => (int)a == raw))
                return (TypeActionJeu)raw;
        }
    }

    private static int ReadRaiseAmount(int maxChips, int actualBet)
    {
        while (true)
        {
            Console.Write("De combien voulez-vous relancer ? ");
            if (!int.TryParse(Console.ReadLine(), out var amount))
                continue;
            // Allow raising only if the amount is greater than 0, less than or equal to the player's chips, 
            // and greater than the current bet.
            if (amount > 0 && amount <= maxChips && actualBet < amount)
                return amount;
        }
    }
}
