using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Blackjack;

public class BlackjackGameState
{
    public required IReadOnlyList<Card> PlayerCards { get; init; }
    public required IReadOnlyList<Card> DealerCards { get; init; }
    public required bool IsDealerHoleCardHidden { get; init; }
    public required bool IsRoundOver { get; init; }
    public required string StatusMessage { get; init; }
}
