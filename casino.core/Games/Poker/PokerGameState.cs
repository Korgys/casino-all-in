using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;

namespace casino.core.Games.Poker;

public record PokerPlayerState(
    string Name,
    int Chips,
    bool IsHuman,
    bool IsFolded,
    TypeActionJeu LastAction,
    HandCards? Hand,
    bool IsWinner);

public record PokerGameState(
    string Phase,
    int StartingBet,
    int Pot,
    int CurrentBet,
    TableCards CommunityCards,
    IReadOnlyList<PokerPlayerState> Players,
    string CurrentPlayer);
