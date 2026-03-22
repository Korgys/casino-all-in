using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Blackjack;

public class BlackjackGameState
{
    public required IReadOnlyList<Card> PlayerCards { get; init; }
    public required IReadOnlyList<Card> DealerCards { get; init; }
    public required bool IsDealerHoleCardHidden { get; init; }
    public required bool IsRoundOver { get; init; }
    public required string StatusMessage { get; init; }
    public required BlackjackRoundOutcome RoundOutcome { get; init; }
    public required int PlayerWins { get; init; }
    public required int DealerWins { get; init; }
    public required int Pushes { get; init; }
}
