using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Jeux.Poker.Joueurs;

public enum JoueurActionType
{
    Aucune = 0,
    SeCoucher = 1,
    Miser = 2,
    Suivre = 3,
    Relancer = 4,
    Check = 5,
    Tapis = 6
}
