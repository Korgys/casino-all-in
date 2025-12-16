using System.Linq;
using casino.core;
using casino.core.Jeux.Poker;
using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Joueurs;
using casino.core.Jeux.Poker.Joueurs.Strategies;

namespace casino.console;

public class ConsoleGameFactory : IGameFactory
{
    public IGame? Create(string gameName, Func<RequeteAction, ActionJeu> humanActionSelector, Func<bool> continuePlaying)
    {
        return gameName.ToLower() switch
        {
            "poker" => CreatePoker(humanActionSelector, continuePlaying),
            _ => null
        };
    }

    public IGame CreatePoker(Func<RequeteAction, ActionJeu> humanActionSelector, Func<bool> continuePlaying)
    {
        var joueur = new JoueurHumain("Player", 1000);
        var joueurs = new List<Joueur>
        {
            joueur,
            new JoueurOrdi("Ordi Agressif", 1000, new StrategieAgressive()),
            new JoueurOrdi("Ordi Conserv", 1000, new StrategieConservatrice()),
            new JoueurOrdi("Ordi Random", 1000, new StrategieRandom())
        };

        return new PokerGame(
            joueur,
            joueurs.Skip(1),
            () => new JeuDeCartes(),
            humanActionSelector,
            continuePlaying);
    }
}
