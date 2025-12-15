using System;
using System.Collections.Generic;
using casino.core.Jeux.Poker.Parties;
using casino.core.Jeux.Poker.Scores;

namespace casino.core.Jeux.Poker.Joueurs;

public class ContexteDeJeu
{
    public Partie Partie { get; }
    public Joueur JoueurCourant { get; }
    public IReadOnlyList<JoueurActionType> ActionsPossibles { get; }
    public Score ScoreJoueur { get; }
    public int MiseMinimum => Math.Max(Partie.MiseActuelle, Partie.MiseDeDepart);

    public ContexteDeJeu(Partie partie, Joueur joueurCourant, IReadOnlyList<JoueurActionType> actionsPossibles)
    {
        Partie = partie;
        JoueurCourant = joueurCourant;
        ActionsPossibles = actionsPossibles;
        ScoreJoueur = EvaluateurScore.EvaluerScore(joueurCourant.Main, partie.CartesCommunes);
    }
}
