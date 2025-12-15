using System;
using System.Linq;

namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public class StrategieRandom : IJoueurStrategy
{
    public JoueurAction ProposerAction(ContexteDeJeu contexte)
    {
        var actions = contexte.ActionsPossibles;
        var action = actions[Random.Shared.Next(actions.Count)];

        int montant = action switch
        {
            JoueurActionType.Miser => contexte.MiseMinimum,
            JoueurActionType.Relancer => CalculerRelance(contexte),
            _ => 0
        };

        return new JoueurAction(action, montant);
    }

    private static int CalculerRelance(ContexteDeJeu contexte)
    {
        var miseActuelle = contexte.Partie.MiseActuelle;
        var minimum = Math.Max(miseActuelle + 1, contexte.MiseMinimum);
        var maximum = Math.Max(minimum, Math.Min(contexte.JoueurCourant.Jetons, miseActuelle + contexte.Partie.MiseDeDepart * 3));

        if (minimum >= contexte.JoueurCourant.Jetons)
        {
            return contexte.JoueurCourant.Jetons;
        }

        return Random.Shared.Next(minimum, maximum + 1);
    }
}
