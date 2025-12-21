using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Scores;

namespace casino.core.tests.Jeux.Poker.Scores
{
    [TestClass]
    public class EvaluateurScoreTests
    {
        [TestMethod]
        public void EvaluerScore_QuinteFlushRoyale_RetourneQuinteFlushRoyale_As()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.As, Couleur.Coeur),
                    Card(RangCarte.Roi, Couleur.Coeur)
                ),
                board: Board(
                    Card(RangCarte.Dame, Couleur.Coeur),
                    Card(RangCarte.Valet, Couleur.Coeur),
                    Card(RangCarte.Dix, Couleur.Coeur),
                    Card(RangCarte.Deux, Couleur.Trefle),
                    Card(RangCarte.Trois, Couleur.Pique)
                )
            );

            AssertScore(score, RangMain.QuinteFlushRoyale, RangCarte.As);
        }

        [TestMethod]
        public void EvaluerScore_QuinteFlush_Wheel_RetourneQuinteFlush_5()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.As, Couleur.Pique),
                    Card(RangCarte.Deux, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Trois, Couleur.Pique),
                    Card(RangCarte.Quatre, Couleur.Pique),
                    Card(RangCarte.Cinq, Couleur.Pique),
                    Card(RangCarte.Neuf, Couleur.Carreau),
                    Card(RangCarte.Dame, Couleur.Coeur)
                )
            );

            AssertScore(score, RangMain.QuinteFlush, RangCarte.Cinq);
        }

        [TestMethod]
        public void EvaluerScore_Carre_RetourneCarre_ValeurDuCarre()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Roi, Couleur.Coeur),
                    Card(RangCarte.Roi, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Roi, Couleur.Carreau),
                    Card(RangCarte.Roi, Couleur.Trefle),
                    Card(RangCarte.Dix, Couleur.Coeur),
                    Card(RangCarte.Deux, Couleur.Trefle),
                    Card(RangCarte.Trois, Couleur.Pique)
                )
            );

            AssertScore(score, RangMain.Carre, RangCarte.Roi, RangCarte.Dix);
        }

        [TestMethod]
        public void EvaluerScore_Full_RetourneFull_ValeurDuBrelan()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Dame, Couleur.Coeur),
                    Card(RangCarte.Dame, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Dame, Couleur.Carreau),
                    Card(RangCarte.Deux, Couleur.Trefle),
                    Card(RangCarte.Deux, Couleur.Coeur),
                    Card(RangCarte.Neuf, Couleur.Pique),
                    Card(RangCarte.Valet, Couleur.Carreau)
                )
            );

            AssertScore(score, RangMain.Full, RangCarte.Dame, RangCarte.Deux);
        }

        [TestMethod]
        public void EvaluerScore_Couleur_RetourneCouleur_PlusHauteCarteDeLaCouleur()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.As, Couleur.Carreau),
                    Card(RangCarte.Neuf, Couleur.Carreau)
                ),
                board: Board(
                    Card(RangCarte.Deux, Couleur.Carreau),
                    Card(RangCarte.Quatre, Couleur.Carreau),
                    Card(RangCarte.Huit, Couleur.Carreau),
                    Card(RangCarte.Roi, Couleur.Coeur),
                    Card(RangCarte.Dix, Couleur.Pique)
                )
            );

            AssertScore(score, RangMain.Couleur, RangCarte.As, RangCarte.Neuf, RangCarte.Huit, RangCarte.Quatre, RangCarte.Deux);
        }

        [TestMethod]
        public void EvaluerScore_Suite_RetourneSuite_CarteHaute()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Cinq, Couleur.Coeur),
                    Card(RangCarte.Six, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Sept, Couleur.Carreau),
                    Card(RangCarte.Huit, Couleur.Trefle),
                    Card(RangCarte.Neuf, Couleur.Coeur),
                    Card(RangCarte.As, Couleur.Trefle),
                    Card(RangCarte.Deux, Couleur.Coeur)
                )
            );

            AssertScore(score, RangMain.Suite, RangCarte.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_Brelan_RetourneBrelan_ValeurDuBrelan()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Sept, Couleur.Coeur),
                    Card(RangCarte.Sept, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Sept, Couleur.Carreau),
                    Card(RangCarte.As, Couleur.Trefle),
                    Card(RangCarte.Dix, Couleur.Coeur),
                    Card(RangCarte.Deux, Couleur.Trefle),
                    Card(RangCarte.Trois, Couleur.Pique)
                )
            );

            AssertScore(score, RangMain.Brelan, RangCarte.Sept, RangCarte.As, RangCarte.Dix);
        }

        [TestMethod]
        public void EvaluerScore_DoublePaire_RetourneDoublePaire_PaireLaPlusHaute()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Dame, Couleur.Coeur),
                    Card(RangCarte.Cinq, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Dame, Couleur.Carreau),
                    Card(RangCarte.Cinq, Couleur.Coeur),
                    Card(RangCarte.As, Couleur.Trefle),
                    Card(RangCarte.Deux, Couleur.Trefle),
                    Card(RangCarte.Trois, Couleur.Pique)
                )
            );

            AssertScore(score, RangMain.DoublePaire, RangCarte.Dame, RangCarte.Cinq, RangCarte.As);
        }

        [TestMethod]
        public void EvaluerScore_Paire_RetournePaire_ValeurDeLaPaire()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Valet, Couleur.Coeur),
                    Card(RangCarte.Valet, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.As, Couleur.Carreau),
                    Card(RangCarte.Dix, Couleur.Trefle),
                    Card(RangCarte.Neuf, Couleur.Coeur),
                    Card(RangCarte.Deux, Couleur.Trefle),
                    Card(RangCarte.Trois, Couleur.Pique)
                )
            );

            AssertScore(score, RangMain.Paire, RangCarte.Valet, RangCarte.As, RangCarte.Dix, RangCarte.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_CarteHaute_RetourneCarteHaute_MaxRang()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.As, Couleur.Coeur),
                    Card(RangCarte.Dix, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Neuf, Couleur.Carreau),
                    Card(RangCarte.Huit, Couleur.Trefle),
                    Card(RangCarte.Six, Couleur.Coeur),
                    Card(RangCarte.Quatre, Couleur.Pique),
                    Card(RangCarte.Deux, Couleur.Trefle)
                )
            );

            AssertScore(score, RangMain.CarteHaute, RangCarte.As, RangCarte.Dix, RangCarte.Neuf, RangCarte.Huit, RangCarte.Six);
        }

        // =========================
        // Départages / cas limites / priorités
        // =========================

        [TestMethod]
        public void EvaluerScore_Carre_AjouteLeMeilleurKicker()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Roi, Couleur.Coeur),
                    Card(RangCarte.Roi, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Roi, Couleur.Carreau),
                    Card(RangCarte.Roi, Couleur.Trefle),
                    Card(RangCarte.As, Couleur.Carreau),
                    Card(RangCarte.Dame, Couleur.Trefle),
                    Card(RangCarte.Deux, Couleur.Trefle)
                )
            );

            AssertScore(score, RangMain.Carre, RangCarte.Roi, RangCarte.As);
        }

        [TestMethod]
        public void EvaluerScore_Full_DeuxBrelans_RetourneFull_MeilleurBrelanCommeTrips()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.As, Couleur.Coeur),
                    Card(RangCarte.As, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.As, Couleur.Carreau),
                    Card(RangCarte.Roi, Couleur.Coeur),
                    Card(RangCarte.Roi, Couleur.Pique),
                    Card(RangCarte.Roi, Couleur.Carreau),
                    Card(RangCarte.Deux, Couleur.Trefle)
                )
            );

            AssertScore(score, RangMain.Full, RangCarte.As, RangCarte.Roi);
        }

        [TestMethod]
        public void EvaluerScore_Full_ChoisitLaMeilleurePaireDispo()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Dame, Couleur.Coeur),
                    Card(RangCarte.Dame, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Dame, Couleur.Carreau),
                    Card(RangCarte.As, Couleur.Trefle),
                    Card(RangCarte.As, Couleur.Coeur),
                    Card(RangCarte.Deux, Couleur.Trefle),
                    Card(RangCarte.Deux, Couleur.Coeur)
                )
            );

            AssertScore(score, RangMain.Full, RangCarte.Dame, RangCarte.As);
        }

        [TestMethod]
        public void EvaluerScore_Couleur_PrendTop5Sur6CartesDeLaCouleur()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.As, Couleur.Carreau),
                    Card(RangCarte.Roi, Couleur.Carreau)
                ),
                board: Board(
                    Card(RangCarte.Dame, Couleur.Carreau),
                    Card(RangCarte.Neuf, Couleur.Carreau),
                    Card(RangCarte.Quatre, Couleur.Carreau),
                    Card(RangCarte.Deux, Couleur.Carreau),
                    Card(RangCarte.Dix, Couleur.Pique)
                )
            );

            // Valeur = top1, Kickers = top2..top5
            AssertScore(score, RangMain.Couleur, RangCarte.As,
                RangCarte.Roi, RangCarte.Dame, RangCarte.Neuf, RangCarte.Quatre);
        }

        [TestMethod]
        public void EvaluerScore_Suite_Wheel_RetourneSuite_5()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.As, Couleur.Coeur),
                    Card(RangCarte.Deux, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Trois, Couleur.Carreau),
                    Card(RangCarte.Quatre, Couleur.Trefle),
                    Card(RangCarte.Cinq, Couleur.Coeur),
                    Card(RangCarte.Dame, Couleur.Trefle),
                    Card(RangCarte.Neuf, Couleur.Pique)
                )
            );

            AssertScore(score, RangMain.Suite, RangCarte.Cinq);
        }

        [TestMethod]
        public void EvaluerScore_Suite_AvecDoublons_RetourneBonneCarteHaute()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Six, Couleur.Coeur),
                    Card(RangCarte.Six, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Cinq, Couleur.Carreau),
                    Card(RangCarte.Sept, Couleur.Trefle),
                    Card(RangCarte.Huit, Couleur.Coeur),
                    Card(RangCarte.Neuf, Couleur.Pique),
                    Card(RangCarte.As, Couleur.Trefle)
                )
            );

            AssertScore(score, RangMain.Suite, RangCarte.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_Brelan_ChoisitLesDeuxMeilleursKickers()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Sept, Couleur.Coeur),
                    Card(RangCarte.Sept, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Sept, Couleur.Carreau),
                    Card(RangCarte.As, Couleur.Trefle),
                    Card(RangCarte.Roi, Couleur.Coeur),
                    Card(RangCarte.Dame, Couleur.Pique),
                    Card(RangCarte.Deux, Couleur.Trefle)
                )
            );

            AssertScore(score, RangMain.Brelan, RangCarte.Sept, RangCarte.As, RangCarte.Roi);
        }

        [TestMethod]
        public void EvaluerScore_DoublePaire_ContientPaireBasseEtKicker()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Dame, Couleur.Coeur),
                    Card(RangCarte.Cinq, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Dame, Couleur.Carreau),
                    Card(RangCarte.Cinq, Couleur.Coeur),
                    Card(RangCarte.As, Couleur.Trefle),
                    Card(RangCarte.Roi, Couleur.Trefle),
                    Card(RangCarte.Deux, Couleur.Pique)
                )
            );

            // Dans ton Score: Valeur = paire haute; Kickers = [paire basse, kicker]
            AssertScore(score, RangMain.DoublePaire, RangCarte.Dame, RangCarte.Cinq, RangCarte.As);
        }

        [TestMethod]
        public void EvaluerScore_Paire_ChoisitLesTroisMeilleursKickers()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.Valet, Couleur.Coeur),
                    Card(RangCarte.Valet, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.As, Couleur.Carreau),
                    Card(RangCarte.Roi, Couleur.Trefle),
                    Card(RangCarte.Neuf, Couleur.Coeur),
                    Card(RangCarte.Huit, Couleur.Pique),
                    Card(RangCarte.Deux, Couleur.Trefle)
                )
            );

            AssertScore(score, RangMain.Paire, RangCarte.Valet, RangCarte.As, RangCarte.Roi, RangCarte.Neuf);
        }

        [TestMethod]
        public void EvaluerScore_CarteHaute_RetourneTop5RangsEnKickers()
        {
            var score = Eval(
                main: Main(
                    Card(RangCarte.As, Couleur.Coeur),
                    Card(RangCarte.Quatre, Couleur.Pique)
                ),
                board: Board(
                    Card(RangCarte.Roi, Couleur.Carreau),
                    Card(RangCarte.Dix, Couleur.Trefle),
                    Card(RangCarte.Neuf, Couleur.Coeur),
                    Card(RangCarte.Sept, Couleur.Pique),
                    Card(RangCarte.Deux, Couleur.Trefle)
                )
            );

            AssertScore(score, RangMain.CarteHaute, RangCarte.As,
                RangCarte.Roi, RangCarte.Dix, RangCarte.Neuf, RangCarte.Sept);
        }

        // =========================
        // Helpers
        // =========================

        private static Score Eval(CartesMain main, CartesCommunes board)
            => EvaluateurScore.EvaluerScore(main, board);

        private static void AssertScore(Score score, RangMain expectedRang, RangCarte expectedValeur, params RangCarte[] expectedKickers)
        {
            Assert.AreEqual(expectedRang, score.Rang);
            Assert.AreEqual(expectedValeur, score.Valeur);

            CollectionAssert.AreEqual(expectedKickers, score.Kickers.ToArray(),
                $"Kickers attendus: [{string.Join(", ", expectedKickers)}] / Obtenus: [{string.Join(", ", score.Kickers)}]");
        }

        private static Carte Card(RangCarte rang, Couleur couleur)
            => new Carte(rang, couleur);

        private static CartesMain Main(Carte a, Carte b)
            => new CartesMain(a, b);

        private static CartesCommunes Board(Carte a, Carte b, Carte c, Carte d, Carte e)
            => new CartesCommunes
            {
                Flop1 = a,
                Flop2 = b,
                Flop3 = c,
                Turn = d,
                River = e
            };
    }
}
