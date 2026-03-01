using casino.core.Games.Poker.Cards;
using casino.core.Properties.Langages;
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

public static class PokerTypeActionExtensions
{
    public static string ToDisplayString(this PokerTypeAction pokerTypeAction)
    {
        return pokerTypeAction switch
        {
            PokerTypeAction.Fold => Resources.Fold,
            PokerTypeAction.Bet => Resources.Bet,
            PokerTypeAction.Call => Resources.Call,
            PokerTypeAction.Raise => Resources.Raise,
            PokerTypeAction.Check => Resources.Check,
            PokerTypeAction.AllIn => Resources.AllIn,
            _ => Resources.None
        };
    }
}