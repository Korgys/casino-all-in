using Microsoft.VisualStudio.TestTools.UnitTesting;
using casino.core.Jeux.Poker.Cartes;
using casino.core.Jeux.Poker.Parties;
using casino.core.Jeux.Poker.Scores;

namespace casino.core.tests.Jeux.Poker.Scores
{
    [TestClass]
    public class EvaluateurScoreTests
    {
        [TestMethod]
        public void EvaluerScore_QuinteFlushRoyale_RetourneQuinteFlushRoyale_As()
        {
            // Construit une main + communes contenant 10-J-Q-K-A de la même couleur.
            var main = Main(
                C(RangCarte.As, Couleur.Coeur),
                C(RangCarte.Roi, Couleur.Coeur)
            );

            var communes = Communes(
                C(RangCarte.Dame, Couleur.Coeur),
                C(RangCarte.Valet, Couleur.Coeur),
                C(RangCarte.Dix, Couleur.Coeur),
                C(RangCarte.Deux, Couleur.Trèfle),
                C(RangCarte.Trois, Couleur.Pique)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.QuinteFlushRoyale, score.Rang);
            Assert.AreEqual(RangCarte.As, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_QuinteFlush_Wheel_RetourneQuinteFlush_5()
        {
            // Construit une quinte flush A-2-3-4-5 (As bas) de la même couleur.
            var main = Main(
                C(RangCarte.As, Couleur.Pique),
                C(RangCarte.Deux, Couleur.Pique)
            );

            var communes = Communes(
                C(RangCarte.Trois, Couleur.Pique),
                C(RangCarte.Quatre, Couleur.Pique),
                C(RangCarte.Cinq, Couleur.Pique),
                C(RangCarte.Neuf, Couleur.Carreau),
                C(RangCarte.Dame, Couleur.Coeur)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.QuinteFlush, score.Rang);
            Assert.AreEqual(RangCarte.Cinq, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_Carre_RetourneCarre_ValeurDuCarre()
        {
            // Construit un carré de Rois.
            var main = Main(
                C(RangCarte.Roi, Couleur.Coeur),
                C(RangCarte.Roi, Couleur.Pique)
            );

            var communes = Communes(
                C(RangCarte.Roi, Couleur.Carreau),
                C(RangCarte.Roi, Couleur.Trèfle),
                C(RangCarte.Dix, Couleur.Coeur),
                C(RangCarte.Deux, Couleur.Trèfle),
                C(RangCarte.Trois, Couleur.Pique)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.Carre, score.Rang);
            Assert.AreEqual(RangCarte.Roi, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_Full_RetourneFull_ValeurDuBrelan()
        {
            // Construire un full : Dames par les Dames + Deux par les Deux.
            var main = Main(
                C(RangCarte.Dame, Couleur.Coeur),
                C(RangCarte.Dame, Couleur.Pique)
            );

            var communes = Communes(
                C(RangCarte.Dame, Couleur.Carreau),
                C(RangCarte.Deux, Couleur.Trèfle),
                C(RangCarte.Deux, Couleur.Coeur),
                C(RangCarte.Neuf, Couleur.Pique),
                C(RangCarte.Valet, Couleur.Carreau)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.Full, score.Rang);
            Assert.AreEqual(RangCarte.Dame, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_Full_DeuxBrelans_RetourneFull_MeilleurBrelanCommeTrips()
        {
            // Construit deux brelans (A et K) : le full doit choisir le meilleur brelan comme trips (A).
            var main = Main(
                C(RangCarte.As, Couleur.Coeur),
                C(RangCarte.As, Couleur.Pique)
            );

            var communes = Communes(
                C(RangCarte.As, Couleur.Carreau),
                C(RangCarte.Roi, Couleur.Coeur),
                C(RangCarte.Roi, Couleur.Pique),
                C(RangCarte.Roi, Couleur.Carreau),
                C(RangCarte.Deux, Couleur.Trèfle)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.Full, score.Rang);
            Assert.AreEqual(RangCarte.As, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_Couleur_RetourneCouleur_PlusHauteCarteDeLaCouleur()
        {
            // Construit une couleur (non suite) et vérifier la plus haute carte.
            var main = Main(
                C(RangCarte.As, Couleur.Carreau),
                C(RangCarte.Neuf, Couleur.Carreau)
            );

            var communes = Communes(
                C(RangCarte.Deux, Couleur.Carreau),
                C(RangCarte.Quatre, Couleur.Carreau),
                C(RangCarte.Huit, Couleur.Carreau),
                C(RangCarte.Roi, Couleur.Coeur),
                C(RangCarte.Dix, Couleur.Pique)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.Couleur, score.Rang);
            Assert.AreEqual(RangCarte.As, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_Suite_RetourneSuite_CarteHaute()
        {
            // Construit une suite 5-6-7-8-9 et vérifier la carte haute (9).
            var main = Main(
                C(RangCarte.Cinq, Couleur.Coeur),
                C(RangCarte.Six, Couleur.Pique)
            );

            var communes = Communes(
                C(RangCarte.Sept, Couleur.Carreau),
                C(RangCarte.Huit, Couleur.Trèfle),
                C(RangCarte.Neuf, Couleur.Coeur),
                C(RangCarte.As, Couleur.Trèfle),
                C(RangCarte.Deux, Couleur.Coeur)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.Suite, score.Rang);
            Assert.AreEqual(RangCarte.Neuf, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_Brelan_RetourneBrelan_ValeurDuBrelan()
        {
            // Construit un brelan de 7.
            var main = Main(
                C(RangCarte.Sept, Couleur.Coeur),
                C(RangCarte.Sept, Couleur.Pique)
            );

            var communes = Communes(
                C(RangCarte.Sept, Couleur.Carreau),
                C(RangCarte.As, Couleur.Trèfle),
                C(RangCarte.Dix, Couleur.Coeur),
                C(RangCarte.Deux, Couleur.Trèfle),
                C(RangCarte.Trois, Couleur.Pique)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.Brelan, score.Rang);
            Assert.AreEqual(RangCarte.Sept, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_DoublePaire_RetourneDoublePaire_PaireLaPlusHaute()
        {
            // Construit une double paire (Dames + 5) -> valeur attendue : Dame.
            var main = Main(
                C(RangCarte.Dame, Couleur.Coeur),
                C(RangCarte.Cinq, Couleur.Pique)
            );

            var communes = Communes(
                C(RangCarte.Dame, Couleur.Carreau),
                C(RangCarte.Cinq, Couleur.Coeur),
                C(RangCarte.As, Couleur.Trèfle),
                C(RangCarte.Deux, Couleur.Trèfle),
                C(RangCarte.Trois, Couleur.Pique)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.DoublePaire, score.Rang);
            Assert.AreEqual(RangCarte.Dame, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_Paire_RetournePaire_ValeurDeLaPaire()
        {
            // Construit une paire de Valets.
            var main = Main(
                C(RangCarte.Valet, Couleur.Coeur),
                C(RangCarte.Valet, Couleur.Pique)
            );

            var communes = Communes(
                C(RangCarte.As, Couleur.Carreau),
                C(RangCarte.Dix, Couleur.Trèfle),
                C(RangCarte.Neuf, Couleur.Coeur),
                C(RangCarte.Deux, Couleur.Trèfle),
                C(RangCarte.Trois, Couleur.Pique)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.Paire, score.Rang);
            Assert.AreEqual(RangCarte.Valet, score.Valeur);
        }

        [TestMethod]
        public void EvaluerScore_CarteHaute_RetourneCarteHaute_MaxRang()
        {
            // Construit une main sans combinaison et vérifier la carte haute (As).
            var main = Main(
                C(RangCarte.As, Couleur.Coeur),
                C(RangCarte.Dix, Couleur.Pique)
            );

            var communes = Communes(
                C(RangCarte.Neuf, Couleur.Carreau),
                C(RangCarte.Huit, Couleur.Trèfle),
                C(RangCarte.Six, Couleur.Coeur),
                C(RangCarte.Quatre, Couleur.Pique),
                C(RangCarte.Deux, Couleur.Trèfle)
            );

            var score = EvaluateurScore.EvaluerScore(main, communes);

            Assert.AreEqual(RangMain.CarteHaute, score.Rang);
            Assert.AreEqual(RangCarte.As, score.Valeur);
        }

        private static Carte C(RangCarte rang, Couleur couleur)
            => new Carte(rang, couleur);

        private static CartesMain Main(Carte a, Carte b)
            => new CartesMain(a, b);

        private static CartesCommunes Communes(Carte a, Carte b, Carte c, Carte d, Carte e)
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
