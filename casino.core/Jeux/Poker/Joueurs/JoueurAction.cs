using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Jeux.Poker.Joueurs;

public class JoueurAction
{
    public JoueurActionType TypeAction { get; set; }
    public int Montant { get; set; } // Montant associé à l'action (si applicable)
    public JoueurAction(JoueurActionType typeAction, int montant = 0)
    {
        TypeAction = typeAction;
        Montant = montant;
    }
}
