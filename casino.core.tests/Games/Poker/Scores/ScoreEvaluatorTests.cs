using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                    Card(CardRank.As, Suit.Hearts),
                    Card(CardRank.Roi, Suit.Hearts)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Hearts),
                    Card(CardRank.Valet, Suit.Hearts),
                    Card(CardRank.Dix, Suit.Hearts),
                    Card(CardRank.Deux, Suit.Clubs),
                    Card(CardRank.Trois, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.RoyalFlush, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_QuinteFlush_Wheel_RetourneQuinteFlush_5()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Spades),
                    Card(CardRank.Deux, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Trois, Suit.Spades),
                    Card(CardRank.Quatre, Suit.Spades),
                    Card(CardRank.Cinq, Suit.Spades),
                    Card(CardRank.Neuf, Suit.Diamonds),
                    Card(CardRank.Dame, Suit.Hearts)
                )
            );

            AssertScore(score, HandRank.StraightFlush, CardRank.Cinq);
        }

        [TestMethod]
        public void EvaluerScore_Carre_RetourneCarre_ValeurDuCarre()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Roi, Suit.Hearts),
                    Card(CardRank.Roi, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Roi, Suit.Diamonds),
                    Card(CardRank.Roi, Suit.Clubs),
                    Card(CardRank.Dix, Suit.Hearts),
                    Card(CardRank.Deux, Suit.Clubs),
                    Card(CardRank.Trois, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.FourOfAKind, CardRank.Roi, CardRank.Dix);
        }

        [TestMethod]
        public void EvaluerScore_Full_RetourneFull_ValeurDuBrelan()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Dame, Suit.Hearts),
                    Card(CardRank.Dame, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Diamonds),
                    Card(CardRank.Deux, Suit.Clubs),
                    Card(CardRank.Deux, Suit.Hearts),
                    Card(CardRank.Neuf, Suit.Spades),
                    Card(CardRank.Valet, Suit.Diamonds)
                )
            );

            AssertScore(score, HandRank.FullHouse, CardRank.Dame, CardRank.Deux);
        }

        [TestMethod]
        public void EvaluerScore_Suit_RetourneSuit_PlusHauteCarteDeLaSuit()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Diamonds),
                    Card(CardRank.Neuf, Suit.Diamonds)
                ),
                board: Board(
                    Card(CardRank.Deux, Suit.Diamonds),
                    Card(CardRank.Quatre, Suit.Diamonds),
                    Card(CardRank.Huit, Suit.Diamonds),
                    Card(CardRank.Roi, Suit.Hearts),
                    Card(CardRank.Dix, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.Flush, CardRank.As, CardRank.Neuf, CardRank.Huit, CardRank.Quatre, CardRank.Deux);
        }

        [TestMethod]
        public void EvaluerScore_Suite_RetourneSuite_CarteHaute()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Cinq, Suit.Hearts),
                    Card(CardRank.Six, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Sept, Suit.Diamonds),
                    Card(CardRank.Huit, Suit.Clubs),
                    Card(CardRank.Neuf, Suit.Hearts),
                    Card(CardRank.As, Suit.Clubs),
                    Card(CardRank.Deux, Suit.Hearts)
                )
            );

            AssertScore(score, HandRank.Straight, CardRank.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_Brelan_RetourneBrelan_ValeurDuBrelan()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Sept, Suit.Hearts),
                    Card(CardRank.Sept, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Sept, Suit.Diamonds),
                    Card(CardRank.As, Suit.Clubs),
                    Card(CardRank.Dix, Suit.Hearts),
                    Card(CardRank.Deux, Suit.Clubs),
                    Card(CardRank.Trois, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.ThreeOfAKind, CardRank.Sept, CardRank.As, CardRank.Dix);
        }

        [TestMethod]
        public void EvaluerScore_DoublePaire_RetourneDoublePaire_PaireLaPlusHaute()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Dame, Suit.Hearts),
                    Card(CardRank.Cinq, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Diamonds),
                    Card(CardRank.Cinq, Suit.Hearts),
                    Card(CardRank.As, Suit.Clubs),
                    Card(CardRank.Deux, Suit.Clubs),
                    Card(CardRank.Trois, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.TwoPair, CardRank.Dame, CardRank.Cinq, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_Paire_RetournePaire_ValeurDeLaPaire()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Valet, Suit.Hearts),
                    Card(CardRank.Valet, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.As, Suit.Diamonds),
                    Card(CardRank.Dix, Suit.Clubs),
                    Card(CardRank.Neuf, Suit.Hearts),
                    Card(CardRank.Deux, Suit.Clubs),
                    Card(CardRank.Trois, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.OnePair, CardRank.Valet, CardRank.As, CardRank.Dix, CardRank.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_CarteHaute_RetourneCarteHaute_MaxRang()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Hearts),
                    Card(CardRank.Dix, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Neuf, Suit.Diamonds),
                    Card(CardRank.Huit, Suit.Clubs),
                    Card(CardRank.Six, Suit.Hearts),
                    Card(CardRank.Quatre, Suit.Spades),
                    Card(CardRank.Deux, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.HighCard, CardRank.As, CardRank.Dix, CardRank.Neuf, CardRank.Huit, CardRank.Six);
        }

        // =========================
        // Départages / cas limites / priorités
        // =========================

        [TestMethod]
        public void EvaluerScore_Carre_AjouteLeMeilleurKicker()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Roi, Suit.Hearts),
                    Card(CardRank.Roi, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Roi, Suit.Diamonds),
                    Card(CardRank.Roi, Suit.Clubs),
                    Card(CardRank.As, Suit.Diamonds),
                    Card(CardRank.Dame, Suit.Clubs),
                    Card(CardRank.Deux, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.FourOfAKind, CardRank.Roi, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_Full_DeuxBrelans_RetourneFull_MeilleurBrelanCommeTrips()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Hearts),
                    Card(CardRank.As, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.As, Suit.Diamonds),
                    Card(CardRank.Roi, Suit.Hearts),
                    Card(CardRank.Roi, Suit.Spades),
                    Card(CardRank.Roi, Suit.Diamonds),
                    Card(CardRank.Deux, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.FullHouse, CardRank.As, CardRank.Roi);
        }

        [TestMethod]
        public void EvaluerScore_Full_ChoisitLaMeilleurePaireDispo()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Dame, Suit.Hearts),
                    Card(CardRank.Dame, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Diamonds),
                    Card(CardRank.As, Suit.Clubs),
                    Card(CardRank.As, Suit.Hearts),
                    Card(CardRank.Deux, Suit.Clubs),
                    Card(CardRank.Deux, Suit.Hearts)
                )
            );

            AssertScore(score, HandRank.FullHouse, CardRank.Dame, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_Suit_PrendTop5Sur6CartesDeLaSuit()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Diamonds),
                    Card(CardRank.Roi, Suit.Diamonds)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Diamonds),
                    Card(CardRank.Neuf, Suit.Diamonds),
                    Card(CardRank.Quatre, Suit.Diamonds),
                    Card(CardRank.Deux, Suit.Diamonds),
                    Card(CardRank.Dix, Suit.Spades)
                )
            );

            // Valeur = top1, Kickers = top2..top5
            AssertScore(score, HandRank.Flush, CardRank.As,
                CardRank.Roi, CardRank.Dame, CardRank.Neuf, CardRank.Quatre);
        }

        [TestMethod]
        public void EvaluerScore_Suite_Wheel_RetourneSuite_5()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Hearts),
                    Card(CardRank.Deux, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Trois, Suit.Diamonds),
                    Card(CardRank.Quatre, Suit.Clubs),
                    Card(CardRank.Cinq, Suit.Hearts),
                    Card(CardRank.Dame, Suit.Clubs),
                    Card(CardRank.Neuf, Suit.Spades)
                )
            );

            AssertScore(score, HandRank.Straight, CardRank.Cinq);
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
                    Card(CardRank.Cinq, Suit.Diamonds),
                    Card(CardRank.Sept, Suit.Clubs),
                    Card(CardRank.Huit, Suit.Hearts),
                    Card(CardRank.Neuf, Suit.Spades),
                    Card(CardRank.As, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.Straight, CardRank.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_Brelan_ChoisitLesDeuxMeilleursKickers()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Sept, Suit.Hearts),
                    Card(CardRank.Sept, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Sept, Suit.Diamonds),
                    Card(CardRank.As, Suit.Clubs),
                    Card(CardRank.Roi, Suit.Hearts),
                    Card(CardRank.Dame, Suit.Spades),
                    Card(CardRank.Deux, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.ThreeOfAKind, CardRank.Sept, CardRank.As, CardRank.Roi);
        }

        [TestMethod]
        public void EvaluerScore_DoublePaire_ContientPaireBasseEtKicker()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Dame, Suit.Hearts),
                    Card(CardRank.Cinq, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Diamonds),
                    Card(CardRank.Cinq, Suit.Hearts),
                    Card(CardRank.As, Suit.Clubs),
                    Card(CardRank.Roi, Suit.Clubs),
                    Card(CardRank.Deux, Suit.Spades)
                )
            );

            // Dans ton Score: Valeur = paire haute; Kickers = [paire basse, kicker]
            AssertScore(score, HandRank.TwoPair, CardRank.Dame, CardRank.Cinq, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_Paire_ChoisitLesTroisMeilleursKickers()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Valet, Suit.Hearts),
                    Card(CardRank.Valet, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.As, Suit.Diamonds),
                    Card(CardRank.Roi, Suit.Clubs),
                    Card(CardRank.Neuf, Suit.Hearts),
                    Card(CardRank.Huit, Suit.Spades),
                    Card(CardRank.Deux, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.OnePair, CardRank.Valet, CardRank.As, CardRank.Roi, CardRank.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_CarteHaute_RetourneTop5RangsEnKickers()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Hearts),
                    Card(CardRank.Quatre, Suit.Spades)
                ),
                board: Board(
                    Card(CardRank.Roi, Suit.Diamonds),
                    Card(CardRank.Dix, Suit.Clubs),
                    Card(CardRank.Neuf, Suit.Hearts),
                    Card(CardRank.Sept, Suit.Spades),
                    Card(CardRank.Deux, Suit.Clubs)
                )
            );

            AssertScore(score, HandRank.HighCard, CardRank.As,
                CardRank.Roi, CardRank.Dix, CardRank.Neuf, CardRank.Sept);
        }

        // =========================
        // Helpers
        // =========================

        private static Score Eval(HandCards main, TableCards board)
            => ScoreEvaluator.EvaluateScore(main, board);

        private static void AssertScore(Score score, HandRank expectedRang, CardRank expectedValeur, params CardRank[] expectedKickers)
        {
            Assert.AreEqual(expectedRang, score.Rank);
            Assert.AreEqual(expectedValeur, score.CardValue);

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
