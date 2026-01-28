using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Parties.Phases;
using casino.core.Jeux.Poker.Scores;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public class StrategieOpportuniste : IStrategieJoueur
{
    public ActionJeu ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        if (actions.Count == 1)
            return new ActionJeu(actions.First());

        int nbActifs = Math.Max(2, contexte.Partie.Joueurs.Count(j => !j.EstCouche()));
        Phase phase = contexte.Partie.Phase;

        // pBrut est en 0..100
        double pBrut = EvaluateurProbabilite.EstimerProbabiliteDeGagner(
            contexte.JoueurCourant.Main,
            contexte.Partie.CartesCommunes,
            contexte.Partie.Joueurs.Count,
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
        if (actions.Contains(TypeActionJeu.Tapis) && p >= seuilTapis)
            return new ActionJeu(TypeActionJeu.Tapis);

        // Si faible vs neutre => fold si possible
        if (p < seuilCouche)
        {
            if (actions.Contains(TypeActionJeu.SeCoucher))
                return new ActionJeu(TypeActionJeu.SeCoucher);

            if (actions.Contains(TypeActionJeu.Suivre))
                return new ActionJeu(TypeActionJeu.Suivre);

            return new ActionJeu(actions.First());
        }

        // Proche du neutre => suivre/check
        if (p < seuilRelance)
        {
            if (actions.Contains(TypeActionJeu.Suivre))
                return new ActionJeu(TypeActionJeu.Suivre);

            // Si pas de suivre (cas rare), petite mise min
            if (actions.Contains(TypeActionJeu.Miser))
                return new ActionJeu(TypeActionJeu.Miser, contexte.MiseMinimum);

            return new ActionJeu(actions.First());
        }

        // Bon avantage => relancer/miser
        if (actions.Contains(TypeActionJeu.Relancer))
            return new ActionJeu(TypeActionJeu.Relancer, contexte.MiseMinimum);

        if (actions.Contains(TypeActionJeu.Miser))
            return new ActionJeu(TypeActionJeu.Miser, contexte.MiseMinimum);

        if (actions.Contains(TypeActionJeu.Suivre))
            return new ActionJeu(TypeActionJeu.Suivre);

        return new ActionJeu(actions.First());
    }

    private static double BornerEntre0Et1(double x) => x < 0 ? 0 : (x > 1 ? 1 : x);
}

