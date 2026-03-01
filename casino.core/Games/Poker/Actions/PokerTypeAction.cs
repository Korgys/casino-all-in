using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Games.Poker.Actions;

/// <summary>
/// Available actions in poker
/// </summary>
public enum PokerTypeAction
{
    None = 0,
    Fold = 1,
    Bet = 2,
    Call = 3,
    Raise = 4,
    Check = 5,
    AllIn = 6
}