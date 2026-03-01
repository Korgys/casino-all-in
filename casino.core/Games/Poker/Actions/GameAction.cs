using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Actions;

public class GameAction
{
    public TypeGameAction TypeAction { get; set; }
    public int Montant { get; set; } // Montant associé à l'action (si applicable)
    public GameAction(TypeGameAction typeAction, int montant = 0)
    {
        TypeAction = typeAction;
        Montant = montant;
    }
}
