using casino.core.Games.Poker.Cards;
using System.Linq;

namespace casino.core.Games.Blackjack;

public static class BlackjackScoreCalculator
{
    public static int Calculate(IReadOnlyList<Card> cards)
    {
        var total = 0;
        var aces = 0;

        foreach (var rank in cards.Select(card => card.Rank))
        {
            switch (rank)
            {
                case CardRank.Valet:
                case CardRank.Dame:
                case CardRank.Roi:
                    total += 10;
                    break;
                case CardRank.As:
                    total += 11;
                    aces++;
                    break;
                default:
                    total += (int)rank;
                    break;
            }
        }

        while (total > 21 && aces > 0)
        {
            total -= 10;
            aces--;
        }

        return total;
    }
}
