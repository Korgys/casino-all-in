using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Players.Strategies;

public class OpportunisticStrategy : IPlayerStrategy
{
    public GameAction ProposerAction(GameContext contexte)
    {
        var actions = contexte.AvailableActions;
        if (actions.Count == 1)
            return new GameAction(actions.First());

        int nbActifs = Math.Max(2, contexte.Round.Players.Count(j => !j.IsFolded()));
        Phase phase = contexte.Round.Phase;

        // pBrut est en 0..100
        double pBrut = ProbabilityEvaluator.EstimerProbabiliteDeGagner(
            contexte.CurrentPlayer.Hand,
            contexte.Round.CommunityCards,
            contexte.Round.Players.Count,
            simulations: 1000);

        // Normalisation 0..1
        double p = BornerEntre0Et1(pBrut / 100.0);

        // Proba "neutre" ~ 1 / nbActifs
        double neutre = 1.0 / nbActifs;

        // Multiplicateurs par rapport au neutre (à ajuster)
        // Préflop on est + prudent (moins d'infos)
        double foldK = phase < Phase.Flop ? 0.75 : 0.70;  // fold si p < foldK * neutre
        double callK = phase < Phase.Flop ? 1.05 : 1.00;  // call si p < callK * neutre
        double raiseK = phase < Phase.Flop ? 1.35 : 1.25;  // raise/bet si p >= raiseK * neutre
        double shoveK = phase < Phase.Flop ? 1.90 : 1.70;  // shove si p >= shoveK * neutre

        double seuilCouche = foldK * neutre;
        double seuilSuivre = callK * neutre;
        double seuilRelance = raiseK * neutre;
        double seuilTapis = shoveK * neutre;

        // --- Décision ---

        // Tapis si énorme avantage relatif
        if (actions.Contains(TypeGameAction.Tapis) && p >= seuilTapis)
            return new GameAction(TypeGameAction.Tapis);

        // Si faible vs neutre => fold si possible
        if (p < seuilCouche)
        {
            if (actions.Contains(TypeGameAction.SeCoucher))
                return new GameAction(TypeGameAction.SeCoucher);

            if (actions.Contains(TypeGameAction.Suivre))
                return new GameAction(TypeGameAction.Suivre);

            return new GameAction(actions.First());
        }

        // Proche du neutre => suivre/check
        if (p < seuilRelance)
        {
            if (actions.Contains(TypeGameAction.Suivre))
                return new GameAction(TypeGameAction.Suivre);

            // Si pas de suivre (cas rare), petite mise min
            if (actions.Contains(TypeGameAction.Miser))
                return new GameAction(TypeGameAction.Miser, contexte.MinimumBet);

            return new GameAction(actions.First());
        }

        // Bon avantage => relancer/miser
        if (actions.Contains(TypeGameAction.Relancer))
            return new GameAction(TypeGameAction.Relancer, contexte.MinimumBet);

        if (actions.Contains(TypeGameAction.Miser))
            return new GameAction(TypeGameAction.Miser, contexte.MinimumBet);

        if (actions.Contains(TypeGameAction.Suivre))
            return new GameAction(TypeGameAction.Suivre);

        return new GameAction(actions.First());
    }

    private static double BornerEntre0Et1(double x) => x < 0 ? 0 : (x > 1 ? 1 : x);
}

