using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Scores
{
    [TestClass]
    public class ScoreEvaluatorTests
    {
        [TestMethod]
        public void EvaluateScore_RoyalFlush_ShouldReturnRoyalFlushWithAce()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Ace, Suit.Hearts),
                    Card(CardRank.King, Suit.Hearts)
                ),
                board: Board(
                    Card(CardRank.Queen, Suit.Hearts),
                    Card(CardRank.Jack, Suit.Hearts),
                    Card(CardRank.Ten, Suit.Hearts),
                    Card(CardRank.Two, Suit.Clubs),
                    Card(CardRank.Three, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.RoyalFlush, CardRank.Ace);
        }

        [TestMethod]
        public void EvaluateScore_WheelStraightFlush_ShouldReturnStraightFlushWithFive()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Ace, Suit.Spades),
                    Card(CardRank.Two, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Three, Suit.Spades),
                    Card(CardRank.Four, Suit.Spades),
                    Card(CardRank.Five, Suit.Spades),
                    Card(CardRank.Nine, Suit.Diamonds),
                    Card(CardRank.Queen, Suit.Hearts)
                )
            );

            AssertScore(score, HandRank.StraightFlush, CardRank.Five);
        }

        [TestMethod]
        public void EvaluateScore_FourOfAKind_ShouldReturnFourOfAKindValue()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.King, Suit.Hearts),
                    Card(CardRank.King, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.King, Suit.Diamonds),
                    Card(CardRank.King, Suit.Clubs),
                    Card(CardRank.Ten, Suit.Hearts),
                    Card(CardRank.Two, Suit.Clubs),
                    Card(CardRank.Three, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.FourOfAKind, CardRank.King, CardRank.Ten);
        }

        [TestMethod]
        public void EvaluateScore_FullHouse_ShouldReturnTripsValue()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Queen, Suit.Hearts),
                    Card(CardRank.Queen, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Queen, Suit.Diamonds),
                    Card(CardRank.Two, Suit.Clubs),
                    Card(CardRank.Two, Suit.Hearts),
                    Card(CardRank.Nine, Suit.Spades),
                    Card(CardRank.Jack, Suit.Diamonds)
                )
            );

            AssertScore(score, HandRank.FullHouse, CardRank.Queen, CardRank.Two);
        }

        [TestMethod]
        public void EvaluateScore_Flush_ShouldReturnHighestFlushCard()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Ace, Suit.Diamonds),
                    Card(CardRank.Nine, Suit.Diamonds)
                ),
                board: Board(
                    Card(CardRank.Two, Suit.Diamonds),
                    Card(CardRank.Four, Suit.Diamonds),
                    Card(CardRank.Eight, Suit.Diamonds),
                    Card(CardRank.King, Suit.Hearts),
                    Card(CardRank.Ten, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.Flush, CardRank.Ace, CardRank.Nine, CardRank.Eight, CardRank.Four, CardRank.Two);
        }

        [TestMethod]
        public void EvaluateScore_Straight_ShouldReturnHighCard()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Five, Suit.Hearts),
                    Card(CardRank.Six, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Seven, Suit.Diamonds),
                    Card(CardRank.Eight, Suit.Clubs),
                    Card(CardRank.Nine, Suit.Hearts),
                    Card(CardRank.Ace, Suit.Clubs),
                    Card(CardRank.Two, Suit.Hearts)
                )
            );

            AssertScore(score, HandRank.Straight, CardRank.Nine);
        }

        [TestMethod]
        public void EvaluateScore_ThreeOfAKind_ShouldReturnTripsValue()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Seven, Suit.Hearts),
                    Card(CardRank.Seven, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Seven, Suit.Diamonds),
                    Card(CardRank.Ace, Suit.Clubs),
                    Card(CardRank.Ten, Suit.Hearts),
                    Card(CardRank.Two, Suit.Clubs),
                    Card(CardRank.Three, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.ThreeOfAKind, CardRank.Seven, CardRank.Ace, CardRank.Ten);
        }

        [TestMethod]
        public void EvaluateScore_TwoPair_ShouldReturnHighestPair()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Queen, Suit.Hearts),
                    Card(CardRank.Five, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Queen, Suit.Diamonds),
                    Card(CardRank.Five, Suit.Hearts),
                    Card(CardRank.Ace, Suit.Clubs),
                    Card(CardRank.Two, Suit.Clubs),
                    Card(CardRank.Three, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.TwoPair, CardRank.Queen, CardRank.Five, CardRank.Ace);
        }

        [TestMethod]
        public void EvaluateScore_OnePair_ShouldReturnPairValue()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Jack, Suit.Hearts),
                    Card(CardRank.Jack, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Ace, Suit.Diamonds),
                    Card(CardRank.Ten, Suit.Clubs),
                    Card(CardRank.Nine, Suit.Hearts),
                    Card(CardRank.Two, Suit.Clubs),
                    Card(CardRank.Three, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.OnePair, CardRank.Jack, CardRank.Ace, CardRank.Ten, CardRank.Nine);
        }

        [TestMethod]
        public void EvaluateScore_HighCard_ShouldReturnHighestRank()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Ace, Suit.Hearts),
                    Card(CardRank.Ten, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Nine, Suit.Diamonds),
                    Card(CardRank.Eight, Suit.Clubs),
                    Card(CardRank.Six, Suit.Hearts),
                    Card(CardRank.Four, Suit.Spades),
                    Card(CardRank.Two, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.HighCard, CardRank.Ace, CardRank.Ten, CardRank.Nine, CardRank.Eight, CardRank.Six);
        }

        // =========================
        // Tie-breakers / edge cases / priorities
        // =========================

        [TestMethod]
        public void EvaluateScore_FourOfAKind_ShouldAddBestKicker()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.King, Suit.Hearts),
                    Card(CardRank.King, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.King, Suit.Diamonds),
                    Card(CardRank.King, Suit.Clubs),
                    Card(CardRank.Ace, Suit.Diamonds),
                    Card(CardRank.Queen, Suit.Clubs),
                    Card(CardRank.Two, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.FourOfAKind, CardRank.King, CardRank.Ace);
        }

        [TestMethod]
        public void EvaluateScore_FullHouse_WithTwoTrips_ShouldUseBestTrips()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Ace, Suit.Hearts),
                    Card(CardRank.Ace, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Ace, Suit.Diamonds),
                    Card(CardRank.King, Suit.Hearts),
                    Card(CardRank.King, Suit.Spades),
                    Card(CardRank.King, Suit.Diamonds),
                    Card(CardRank.Two, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.FullHouse, CardRank.Ace, CardRank.King);
        }

        [TestMethod]
        public void EvaluateScore_FullHouse_ShouldUseBestAvailablePair()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Queen, Suit.Hearts),
                    Card(CardRank.Queen, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Queen, Suit.Diamonds),
                    Card(CardRank.Ace, Suit.Clubs),
                    Card(CardRank.Ace, Suit.Hearts),
                    Card(CardRank.Two, Suit.Clubs),
                    Card(CardRank.Two, Suit.Hearts)
                )
            );

            AssertScore(score, HandRank.FullHouse, CardRank.Queen, CardRank.Ace);
        }

        [TestMethod]
        public void EvaluateScore_Flush_ShouldTakeTopFiveOfSixFlushCards()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Ace, Suit.Diamonds),
                    Card(CardRank.King, Suit.Diamonds)
                ),
                board: Board(
                    Card(CardRank.Queen, Suit.Diamonds),
                    Card(CardRank.Nine, Suit.Diamonds),
                    Card(CardRank.Four, Suit.Diamonds),
                    Card(CardRank.Two, Suit.Diamonds),
                    Card(CardRank.Ten, Suit.Spades)
                )
            );

            // Value = top1, kickers = top2..top5
            AssertScore(score, HandRank.Flush, CardRank.Ace,
                CardRank.King, CardRank.Queen, CardRank.Nine, CardRank.Four);
        }

        [TestMethod]
        public void EvaluateScore_WheelStraight_ShouldReturnStraightWithFive()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Ace, Suit.Hearts),
                    Card(CardRank.Two, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Three, Suit.Diamonds),
                    Card(CardRank.Four, Suit.Clubs),
                    Card(CardRank.Five, Suit.Hearts),
                    Card(CardRank.Queen, Suit.Clubs),
                    Card(CardRank.Nine, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.Straight, CardRank.Five);
        }

        [TestMethod]
        public void EvaluateScore_Straight_WithDuplicates_ShouldReturnCorrectHighCard()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Six, Suit.Hearts),
                    Card(CardRank.Six, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Five, Suit.Diamonds),
                    Card(CardRank.Seven, Suit.Clubs),
                    Card(CardRank.Eight, Suit.Hearts),
                    Card(CardRank.Nine, Suit.Spades),
                    Card(CardRank.Ace, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.Straight, CardRank.Nine);
        }

        [TestMethod]
        public void EvaluateScore_ThreeOfAKind_ShouldUseTwoBestKickers()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Seven, Suit.Hearts),
                    Card(CardRank.Seven, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Seven, Suit.Diamonds),
                    Card(CardRank.Ace, Suit.Clubs),
                    Card(CardRank.King, Suit.Hearts),
                    Card(CardRank.Queen, Suit.Spades),
                    Card(CardRank.Two, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.ThreeOfAKind, CardRank.Seven, CardRank.Ace, CardRank.King);
        }

        [TestMethod]
        public void EvaluateScore_TwoPair_ShouldIncludeLowPairAndKicker()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Queen, Suit.Hearts),
                    Card(CardRank.Five, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Queen, Suit.Diamonds),
                    Card(CardRank.Five, Suit.Hearts),
                    Card(CardRank.Ace, Suit.Clubs),
                    Card(CardRank.King, Suit.Clubs),
                    Card(CardRank.Two, Suit.Spades)
                )
            );

            // Score uses value = high pair and kickers = [low pair, kicker].
            AssertScore(score, HandRank.TwoPair, CardRank.Queen, CardRank.Five, CardRank.Ace);
        }

        [TestMethod]
        public void EvaluateScore_OnePair_ShouldUseThreeBestKickers()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Jack, Suit.Hearts),
                    Card(CardRank.Jack, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Ace, Suit.Diamonds),
                    Card(CardRank.King, Suit.Clubs),
                    Card(CardRank.Nine, Suit.Hearts),
                    Card(CardRank.Eight, Suit.Spades),
                    Card(CardRank.Two, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.OnePair, CardRank.Jack, CardRank.Ace, CardRank.King, CardRank.Nine);
        }

        [TestMethod]
        public void EvaluateScore_HighCard_ShouldReturnTopFiveRanksAsKickers()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.Ace, Suit.Hearts),
                    Card(CardRank.Four, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.King, Suit.Diamonds),
                    Card(CardRank.Ten, Suit.Clubs),
                    Card(CardRank.Nine, Suit.Hearts),
                    Card(CardRank.Seven, Suit.Spades),
                    Card(CardRank.Two, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.HighCard, CardRank.Ace,
                CardRank.King, CardRank.Ten, CardRank.Nine, CardRank.Seven);
        }

        [TestMethod]
        public void EvaluateScore_TwoPair_ShouldIgnoreUnrelatedDuplicatesForKickerSelection()
        {
            var score = Eval(
                hand: Hand(
                    Card(CardRank.King, Suit.Hearts),
                    Card(CardRank.King, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Queen, Suit.Hearts),
                    Card(CardRank.Queen, Suit.Spades),
                    Card(CardRank.Ace, Suit.Clubs),
                    Card(CardRank.Ace, Suit.Diamonds),
                    Card(CardRank.Two, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.TwoPair, CardRank.Ace, CardRank.King, CardRank.Queen);
        }

        // =========================
        // Helpers
        // =========================

        private static Score Eval(HandCards hand, TableCards board)
            => ScoreEvaluator.EvaluateScore(hand, board);

        private static void AssertScore(Score score, HandRank expectedRank, CardRank expectedValue, params CardRank[] expectedKickers)
        {
            Assert.AreEqual(expectedRank, score.Rank);
            Assert.AreEqual(expectedValue, score.CardValue);

            CollectionAssert.AreEqual(expectedKickers, score.Kickers.ToArray(),
                $"Expected kickers: [{string.Join(", ", expectedKickers)}] / Actual: [{string.Join(", ", score.Kickers)}]");
        }

        private static Card Card(CardRank rank, Suit suit)
            => new Card(rank, suit);

        private static HandCards Hand(Card a, Card b)
            => new HandCards(a, b);

        private static TableCards Board(Card a, Card b, Card c, Card d, Card e)
            => new TableCards
            {
                Flop1 = a,
                Flop2 = b,
                Flop3 = c,
                Turn = d,
                River = e
            };
    }
}
