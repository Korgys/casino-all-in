using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Jeux.Poker.Actions;

public class Action
{
    public TypeAction TypeAction { get; set; }
    public int Montant { get; set; } // Montant associé à l'action (si applicable)
    public Action(TypeAction typeAction, int montant = 0)
    {
        TypeAction = typeAction;
        Montant = montant;
    }
}
