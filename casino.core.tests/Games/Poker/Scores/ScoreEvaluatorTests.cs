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
                    Card(CardRank.As, Suit.Coeur),
                    Card(CardRank.Roi, Suit.Coeur)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Coeur),
                    Card(CardRank.Valet, Suit.Coeur),
                    Card(CardRank.Dix, Suit.Coeur),
                    Card(CardRank.Deux, Suit.Trefle),
                    Card(CardRank.Trois, Suit.Pique)
                )
            );

            AssertScore(score, HandRank.QuinteFlushRoyale, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_QuinteFlush_Wheel_RetourneQuinteFlush_5()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Pique),
                    Card(CardRank.Deux, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Trois, Suit.Pique),
                    Card(CardRank.Quatre, Suit.Pique),
                    Card(CardRank.Cinq, Suit.Pique),
                    Card(CardRank.Neuf, Suit.Carreau),
                    Card(CardRank.Dame, Suit.Coeur)
                )
            );

            AssertScore(score, HandRank.QuinteFlush, CardRank.Cinq);
        }

        [TestMethod]
        public void EvaluerScore_Carre_RetourneCarre_ValeurDuCarre()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Roi, Suit.Coeur),
                    Card(CardRank.Roi, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Roi, Suit.Carreau),
                    Card(CardRank.Roi, Suit.Trefle),
                    Card(CardRank.Dix, Suit.Coeur),
                    Card(CardRank.Deux, Suit.Trefle),
                    Card(CardRank.Trois, Suit.Pique)
                )
            );

            AssertScore(score, HandRank.Carre, CardRank.Roi, CardRank.Dix);
        }

        [TestMethod]
        public void EvaluerScore_Full_RetourneFull_ValeurDuBrelan()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Dame, Suit.Coeur),
                    Card(CardRank.Dame, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Carreau),
                    Card(CardRank.Deux, Suit.Trefle),
                    Card(CardRank.Deux, Suit.Coeur),
                    Card(CardRank.Neuf, Suit.Pique),
                    Card(CardRank.Valet, Suit.Carreau)
                )
            );

            AssertScore(score, HandRank.Full, CardRank.Dame, CardRank.Deux);
        }

        [TestMethod]
        public void EvaluerScore_Suit_RetourneSuit_PlusHauteCarteDeLaSuit()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Carreau),
                    Card(CardRank.Neuf, Suit.Carreau)
                ),
                board: Board(
                    Card(CardRank.Deux, Suit.Carreau),
                    Card(CardRank.Quatre, Suit.Carreau),
                    Card(CardRank.Huit, Suit.Carreau),
                    Card(CardRank.Roi, Suit.Coeur),
                    Card(CardRank.Dix, Suit.Pique)
                )
            );

            AssertScore(score, HandRank.Suit, CardRank.As, CardRank.Neuf, CardRank.Huit, CardRank.Quatre, CardRank.Deux);
        }

        [TestMethod]
        public void EvaluerScore_Suite_RetourneSuite_CarteHaute()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Cinq, Suit.Coeur),
                    Card(CardRank.Six, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Sept, Suit.Carreau),
                    Card(CardRank.Huit, Suit.Trefle),
                    Card(CardRank.Neuf, Suit.Coeur),
                    Card(CardRank.As, Suit.Trefle),
                    Card(CardRank.Deux, Suit.Coeur)
                )
            );

            AssertScore(score, HandRank.Suite, CardRank.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_Brelan_RetourneBrelan_ValeurDuBrelan()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Sept, Suit.Coeur),
                    Card(CardRank.Sept, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Sept, Suit.Carreau),
                    Card(CardRank.As, Suit.Trefle),
                    Card(CardRank.Dix, Suit.Coeur),
                    Card(CardRank.Deux, Suit.Trefle),
                    Card(CardRank.Trois, Suit.Pique)
                )
            );

            AssertScore(score, HandRank.Brelan, CardRank.Sept, CardRank.As, CardRank.Dix);
        }

        [TestMethod]
        public void EvaluerScore_DoublePaire_RetourneDoublePaire_PaireLaPlusHaute()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Dame, Suit.Coeur),
                    Card(CardRank.Cinq, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Carreau),
                    Card(CardRank.Cinq, Suit.Coeur),
                    Card(CardRank.As, Suit.Trefle),
                    Card(CardRank.Deux, Suit.Trefle),
                    Card(CardRank.Trois, Suit.Pique)
                )
            );

            AssertScore(score, HandRank.DoublePaire, CardRank.Dame, CardRank.Cinq, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_Paire_RetournePaire_ValeurDeLaPaire()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Valet, Suit.Coeur),
                    Card(CardRank.Valet, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.As, Suit.Carreau),
                    Card(CardRank.Dix, Suit.Trefle),
                    Card(CardRank.Neuf, Suit.Coeur),
                    Card(CardRank.Deux, Suit.Trefle),
                    Card(CardRank.Trois, Suit.Pique)
                )
            );

            AssertScore(score, HandRank.Paire, CardRank.Valet, CardRank.As, CardRank.Dix, CardRank.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_CarteHaute_RetourneCarteHaute_MaxRang()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Coeur),
                    Card(CardRank.Dix, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Neuf, Suit.Carreau),
                    Card(CardRank.Huit, Suit.Trefle),
                    Card(CardRank.Six, Suit.Coeur),
                    Card(CardRank.Quatre, Suit.Pique),
                    Card(CardRank.Deux, Suit.Trefle)
                )
            );

            AssertScore(score, HandRank.CarteHaute, CardRank.As, CardRank.Dix, CardRank.Neuf, CardRank.Huit, CardRank.Six);
        }

        // =========================
        // Départages / cas limites / priorités
        // =========================

        [TestMethod]
        public void EvaluerScore_Carre_AjouteLeMeilleurKicker()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Roi, Suit.Coeur),
                    Card(CardRank.Roi, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Roi, Suit.Carreau),
                    Card(CardRank.Roi, Suit.Trefle),
                    Card(CardRank.As, Suit.Carreau),
                    Card(CardRank.Dame, Suit.Trefle),
                    Card(CardRank.Deux, Suit.Trefle)
                )
            );

            AssertScore(score, HandRank.Carre, CardRank.Roi, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_Full_DeuxBrelans_RetourneFull_MeilleurBrelanCommeTrips()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Coeur),
                    Card(CardRank.As, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.As, Suit.Carreau),
                    Card(CardRank.Roi, Suit.Coeur),
                    Card(CardRank.Roi, Suit.Pique),
                    Card(CardRank.Roi, Suit.Carreau),
                    Card(CardRank.Deux, Suit.Trefle)
                )
            );

            AssertScore(score, HandRank.Full, CardRank.As, CardRank.Roi);
        }

        [TestMethod]
        public void EvaluerScore_Full_ChoisitLaMeilleurePaireDispo()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Dame, Suit.Coeur),
                    Card(CardRank.Dame, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Carreau),
                    Card(CardRank.As, Suit.Trefle),
                    Card(CardRank.As, Suit.Coeur),
                    Card(CardRank.Deux, Suit.Trefle),
                    Card(CardRank.Deux, Suit.Coeur)
                )
            );

            AssertScore(score, HandRank.Full, CardRank.Dame, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_Suit_PrendTop5Sur6CartesDeLaSuit()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Carreau),
                    Card(CardRank.Roi, Suit.Carreau)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Carreau),
                    Card(CardRank.Neuf, Suit.Carreau),
                    Card(CardRank.Quatre, Suit.Carreau),
                    Card(CardRank.Deux, Suit.Carreau),
                    Card(CardRank.Dix, Suit.Pique)
                )
            );

            // Valeur = top1, Kickers = top2..top5
            AssertScore(score, HandRank.Suit, CardRank.As,
                CardRank.Roi, CardRank.Dame, CardRank.Neuf, CardRank.Quatre);
        }

        [TestMethod]
        public void EvaluerScore_Suite_Wheel_RetourneSuite_5()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Coeur),
                    Card(CardRank.Deux, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Trois, Suit.Carreau),
                    Card(CardRank.Quatre, Suit.Trefle),
                    Card(CardRank.Cinq, Suit.Coeur),
                    Card(CardRank.Dame, Suit.Trefle),
                    Card(CardRank.Neuf, Suit.Pique)
                )
            );

            AssertScore(score, HandRank.Suite, CardRank.Cinq);
        }

        [TestMethod]
        public void EvaluerScore_Suite_AvecDoublons_RetourneBonneCarteHaute()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Six, Suit.Coeur),
                    Card(CardRank.Six, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Cinq, Suit.Carreau),
                    Card(CardRank.Sept, Suit.Trefle),
                    Card(CardRank.Huit, Suit.Coeur),
                    Card(CardRank.Neuf, Suit.Pique),
                    Card(CardRank.As, Suit.Trefle)
                )
            );

            AssertScore(score, HandRank.Suite, CardRank.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_Brelan_ChoisitLesDeuxMeilleursKickers()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Sept, Suit.Coeur),
                    Card(CardRank.Sept, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Sept, Suit.Carreau),
                    Card(CardRank.As, Suit.Trefle),
                    Card(CardRank.Roi, Suit.Coeur),
                    Card(CardRank.Dame, Suit.Pique),
                    Card(CardRank.Deux, Suit.Trefle)
                )
            );

            AssertScore(score, HandRank.Brelan, CardRank.Sept, CardRank.As, CardRank.Roi);
        }

        [TestMethod]
        public void EvaluerScore_DoublePaire_ContientPaireBasseEtKicker()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Dame, Suit.Coeur),
                    Card(CardRank.Cinq, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Dame, Suit.Carreau),
                    Card(CardRank.Cinq, Suit.Coeur),
                    Card(CardRank.As, Suit.Trefle),
                    Card(CardRank.Roi, Suit.Trefle),
                    Card(CardRank.Deux, Suit.Pique)
                )
            );

            // Dans ton Score: Valeur = paire haute; Kickers = [paire basse, kicker]
            AssertScore(score, HandRank.DoublePaire, CardRank.Dame, CardRank.Cinq, CardRank.As);
        }

        [TestMethod]
        public void EvaluerScore_Paire_ChoisitLesTroisMeilleursKickers()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.Valet, Suit.Coeur),
                    Card(CardRank.Valet, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.As, Suit.Carreau),
                    Card(CardRank.Roi, Suit.Trefle),
                    Card(CardRank.Neuf, Suit.Coeur),
                    Card(CardRank.Huit, Suit.Pique),
                    Card(CardRank.Deux, Suit.Trefle)
                )
            );

            AssertScore(score, HandRank.Paire, CardRank.Valet, CardRank.As, CardRank.Roi, CardRank.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_CarteHaute_RetourneTop5RangsEnKickers()
        {
            var score = Eval(
                main: Hand(
                    Card(CardRank.As, Suit.Coeur),
                    Card(CardRank.Quatre, Suit.Pique)
                ),
                board: Board(
                    Card(CardRank.Roi, Suit.Carreau),
                    Card(CardRank.Dix, Suit.Trefle),
                    Card(CardRank.Neuf, Suit.Coeur),
                    Card(CardRank.Sept, Suit.Pique),
                    Card(CardRank.Deux, Suit.Trefle)
                )
            );

            AssertScore(score, HandRank.CarteHaute, CardRank.As,
                CardRank.Roi, CardRank.Dix, CardRank.Neuf, CardRank.Sept);
        }

        // =========================
        // Helpers
        // =========================

        private static Score Eval(HandCards main, TableCards board)
            => ScoreEvaluator.EvaluerScore(main, board);

        private static void AssertScore(Score score, HandRank expectedRang, CardRank expectedValeur, params CardRank[] expectedKickers)
        {
            Assert.AreEqual(expectedRang, score.Rang);
            Assert.AreEqual(expectedValeur, score.Valeur);

            CollectionAssert.AreEqual(expectedKickers, score.Kickers.ToArray(),
                $"Kickers attendus: [{string.Join(", ", expectedKickers)}] / Obtenus: [{string.Join(", ", score.Kickers)}]");
        }

        private static Card Card(CardRank rang, Suit couleur)
            => new Card(rang, couleur);

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
