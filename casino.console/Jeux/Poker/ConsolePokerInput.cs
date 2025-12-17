using casino.core.Jeux.Poker;
using casino.core.Jeux.Poker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using ActionModel = casino.core.Jeux.Poker.Actions.ActionJeu;

namespace casino.console.jeux.Poker;

public class ConsolePokerInput
{
    private readonly ConsolePokerRenderer _renderer;

    public ConsolePokerInput(ConsolePokerRenderer renderer)
    {
        _renderer = renderer;
    }

    public ActionModel GetPlayerAction(RequeteAction request)
    {
        var state = (PokerGameState)request.EtatTable;
        var player = state.Joueurs.First(j => j.Nom == request.JoueurNom);

        var choice = ReadActionChoice(request.ActionsPossibles, request.MiseMinimum);

        return choice switch
        {
            TypeActionJeu.Miser => new ActionModel(choice, request.MiseMinimum),
            TypeActionJeu.Relancer => new ActionModel(choice, ReadRaiseAmount(player.Jetons)),
            _ => new ActionModel(choice, 0),
        };
    }

    public static bool AskContinueNewGame()
    {
        Console.Write("\nVoulez-vous continuer une nouvelle partie ? (o/n) : ");
        var answer = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        return answer is "o" or "oui" or "y" or "yes";
    }

    private static TypeActionJeu ReadActionChoice(IReadOnlyList<TypeActionJeu> possibleActions, int minimumBet)
    {
        while (true)
        {
            ConsolePokerRenderer.RenderPossibleActions(possibleActions, minimumBet);

            Console.Write("Quel est votre choix ? ");
            if (!int.TryParse(Console.ReadLine(), out var raw))
                continue;

            if (possibleActions.Any(a => (int)a == raw))
                return (TypeActionJeu)raw;
        }
    }

    private static int ReadRaiseAmount(int maxJetons)
    {
        while (true)
        {
            Console.Write("De combien voulez-vous relancer ? ");
            if (!int.TryParse(Console.ReadLine(), out var amount))
                continue;

            if (amount > 0 && amount <= maxJetons)
                return amount;
        }
    }
}
