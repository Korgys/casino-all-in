using casino.core.Games.Poker.Cards;

namespace casino.core.Games.Blackjack;

public static class BlackjackScoreCalculator
{
    public static int Calculate(IReadOnlyList<Card> cards)
    {
        var total = 0;
        var aces = 0;

        foreach (var card in cards)
        {
            switch (card.Rank)
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
                    total += (int)card.Rank;
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
