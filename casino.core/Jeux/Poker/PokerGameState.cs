using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Parties;

namespace casino.core.Jeux.Poker;

public record PokerPlayerState(
    string Nom,
    int Jetons,
    bool EstHumain,
    bool EstCouche,
    TypeActionJeu DerniereAction,
    CartesMain? Main,
    bool EstGagnant);

public record PokerGameState(
    string Phase,
    int MiseDeDepart,
    int Pot,
    int MiseActuelle,
    CartesCommunes CartesCommunes,
    IReadOnlyList<PokerPlayerState> Joueurs,
    string JoueurActuel);
