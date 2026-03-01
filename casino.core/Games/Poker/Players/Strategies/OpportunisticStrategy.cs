using casino.core.Common.Utils;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Rounds.Phases;
using casino.core.Games.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Players.Strategies;

public class OpportunisticStrategy : IPlayerStrategy
{
    public GameAction DecideAction(GameContext context)
    {
        var actions = context.AvailableActions;
        if (actions.Count == 1)
            return GetFirstGameAction(context);


        int nbActifs = Math.Max(2, context.Round.Players.Count(j => !j.IsFolded()));
        Phase phase = context.Round.Phase;

        // pBrut est en 0..100
        double pBrut = ProbabilityEvaluator.EstimateWinProbability(
            context.CurrentPlayer.Hand,
            context.Round.CommunityCards,
            context.Round.Players.Count,
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
        if (actions.Contains(PokerTypeAction.AllIn) && p >= seuilTapis)
            return new GameAction(PokerTypeAction.AllIn);

        // Si faible vs neutre => fold si possible
        if (p < seuilCouche)
        {
            if (actions.Contains(PokerTypeAction.Fold))
                return new GameAction(PokerTypeAction.Fold);

            if (actions.Contains(PokerTypeAction.Call))
                return new GameAction(PokerTypeAction.Call);

            return GetFirstGameAction(context);
        }

        // Proche du neutre => suivre/check
        if (p < seuilRelance)
        {
            if (actions.Contains(PokerTypeAction.Call))
                return new GameAction(PokerTypeAction.Call);

            // Si pas de suivre (cas rare), petite mise min
            if (actions.Contains(PokerTypeAction.Bet))
                return new GameAction(PokerTypeAction.Bet, context.MinimumBet);

            return new GameAction(actions.First());
        }

        // Bon avantage => relancer/miser
        if (actions.Contains(PokerTypeAction.Raise))
            return new GameAction(PokerTypeAction.Raise, context.MinimumBet);

        if (actions.Contains(PokerTypeAction.Bet))
            return new GameAction(PokerTypeAction.Bet, context.MinimumBet);

        if (actions.Contains(PokerTypeAction.Call))
            return new GameAction(PokerTypeAction.Call);

        return GetFirstGameAction(context);
    }

    private static GameAction GetFirstGameAction(GameContext context)
    {
        if (context.AvailableActions[0] == PokerTypeAction.Bet)
            return new GameAction(context.AvailableActions[0], context.MinimumBet);
        else
            return new GameAction(context.AvailableActions[0]);
    }

    private static double BornerEntre0Et1(double x) => x < 0 ? 0 : (x > 1 ? 1 : x);
}

