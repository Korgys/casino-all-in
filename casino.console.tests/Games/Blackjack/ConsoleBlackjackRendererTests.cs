using casino.console.Games.Blackjack;
using casino.core.Games.Blackjack;
using casino.core.Games.Poker.Cards;

namespace casino.console.tests.Games.Blackjack;

[TestClass]
public class ConsoleBlackjackRendererTests
{
    [TestMethod]
    public void RenderTable_ShowsStatsAndWinBanner_WhenPlayerWinsRound()
    {
        var state = new BlackjackGameState
        {
            PlayerCards = [new Card(CardRank.As, Suit.Spades), new Card(CardRank.Roi, Suit.Hearts)],
            DealerCards = [new Card(CardRank.Neuf, Suit.Clubs), new Card(CardRank.Sept, Suit.Diamonds)],
            IsDealerHoleCardHidden = false,
            IsRoundOver = true,
            StatusMessage = "Bravo ! Vous remportez cette manche.",
            RoundOutcome = BlackjackRoundOutcome.PlayerWin,
            PlayerWins = 2,
            DealerWins = 1,
            Pushes = 0
        };

        ConsoleBlackjackRenderer.RenderTable(state);
    }
}