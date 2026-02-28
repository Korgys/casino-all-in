using System;
using System.Collections.Generic;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Parties;
using casino.core.Games.Poker.Scores;

namespace casino.core.Games.Poker.Players;

public class ContexteDeJeu
{
    public Partie Partie { get; }
    public Player PlayerCourant { get; }
    public IReadOnlyList<TypeActionJeu> ActionsPossibles { get; }
    public Score ScorePlayer { get; }
    public int MinimumBet => Math.Max(Partie.CurrentBet, Partie.StartingBet);

    public ContexteDeJeu(Partie partie, Player PlayerCourant, IReadOnlyList<TypeActionJeu> actionsPossibles)
    {
        Partie = partie;
        PlayerCourant = PlayerCourant;
        ActionsPossibles = actionsPossibles;
        ScorePlayer = EvaluateurScore.EvaluerScore(PlayerCourant.Hand, partie.CommunityCards);
    }
}
