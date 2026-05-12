using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Scores;

namespace casino.core.tests.Games.Poker.Scores
{
    [TestClass]
    public class ScoreEvaluatorTests
    {
        [TestMethod]
        public void EvaluerScore_QuinteFlushRoyale_RetourneQuinteFlushRoyale_As()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_QuinteFlush_Wheel_RetourneQuinteFlush_5()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Carre_RetourneCarre_ValeurDuCarre()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Full_RetourneFull_ValeurDuBrelan()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Suit_RetourneSuit_PlusHauteCarteDeLaSuit()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Suite_RetourneSuite_CarteHaute()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Brelan_RetourneBrelan_ValeurDuBrelan()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_DoublePaire_RetourneDoublePaire_PaireLaPlusHaute()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Paire_RetournePaire_ValeurDeLaPaire()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_CarteHaute_RetourneCarteHaute_MaxRang()
        {
            var score = Eval(
                main: Hand(
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
        // Départages / cas limites / priorités
        // =========================

        [TestMethod]
        public void EvaluerScore_Carre_AjouteLeMeilleurKicker()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Full_DeuxBrelans_RetourneFull_MeilleurBrelanCommeTrips()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Full_ChoisitLaMeilleurePaireDispo()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Suit_PrendTop5Sur6CartesDeLaSuit()
        {
            var score = Eval(
                main: Hand(
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

            // Valeur = top1, Kickers = top2..top5
            AssertScore(score, HandRank.Flush, CardRank.Ace,
                CardRank.King, CardRank.Queen, CardRank.Nine, CardRank.Four);
        }

        [TestMethod]
        public void EvaluerScore_Suite_Wheel_RetourneSuite_5()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Suite_AvecDoublons_RetourneBonneCarteHaute()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_Brelan_ChoisitLesDeuxMeilleursKickers()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_DoublePaire_ContientPaireBasseEtKicker()
        {
            var score = Eval(
                main: Hand(
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

            // Dans ton Score: Valeur = paire haute; Kickers = [paire basse, kicker]
            AssertScore(score, HandRank.TwoPair, CardRank.Queen, CardRank.Five, CardRank.Ace);
        }

        [TestMethod]
        public void EvaluerScore_Paire_ChoisitLesTroisMeilleursKickers()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_CarteHaute_RetourneTop5RangsEnKickers()
        {
            var score = Eval(
                main: Hand(
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
        public void EvaluerScore_TwoPair_IgnoreUnrelatedDuplicatesForKickerSelection()
        {
            var score = Eval(
                main: Hand(
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

        private static Score Eval(HandCards main, TableCards board)
            => ScoreEvaluator.EvaluateScore(main, board);

        private static void AssertScore(Score score, HandRank expectedRank, CardRank expectedValue, params CardRank[] expectedKickers)
        {
            Assert.AreEqual(expectedRank, score.Rank);
            Assert.AreEqual(expectedValue, score.CardValue);

            CollectionAssert.AreEqual(expectedKickers, score.Kickers.ToArray(),
                $"Kickers attendus: [{string.Join(", ", expectedKickers)}] / Obtenus: [{string.Join(", ", score.Kickers)}]");
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
