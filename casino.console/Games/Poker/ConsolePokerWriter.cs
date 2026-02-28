using casino.console.Games.Commons;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.console.Games.Poker;

/// <summary>
/// Provides static methods for displaying poker-related information, such as player names, amounts, cards, and hands,
/// to the console with enhanced readability through color coding.
/// </summary>
internal class ConsolePokerWriter
{
    public static void WritePlayerName(PokerPlayerState playerState)
    {
        var color =
            (playerState.IsFolded || (playerState.Chips == 0 && playerState.LastAction != TypeActionJeu.Tapis)) ? ConsoleColor.DarkGray :
            playerState.IsHuman ? ConsoleColor.Cyan :
            ConsoleColor.DarkRed;

        using (ConsoleColorScope.Foreground(color))
            Console.Write(playerState.Name);
    }

    public static void WriteAmount(int amount)
    {
        using (ConsoleColorScope.Foreground(ConsoleColor.Yellow))
            Console.Write($"{amount}c");
    }

    public static void WriteCard(Card card)
    {
        var color = (card.Couleur == Couleur.Coeur || card.Couleur == Couleur.Carreau)
            ? ConsoleColor.Red
            : ConsoleColor.Cyan;

        using (ConsoleColorScope.Foreground(color))
            Console.Write(card.ToString());
    }

    public static void WriteHand(HandCards hand)
    {
        var cards = hand.AsEnumerable().ToList();
        for (int i = 0; i < cards.Count; i++)
        {
            if (i > 0) Console.Write(" ");
            WriteCard(cards[i]);
        }
    }

    public static void WriteCommunityCards(TableCards tableCards)
    {
        var cards = new List<Card?> { tableCards.Flop1, tableCards.Flop2, tableCards.Flop3, tableCards.Turn, tableCards.River }
            .Where(c => c is not null)
            .Cast<Card>()
            .ToList();

        for (int i = 0; i < cards.Count; i++)
        {
            if (i > 0) Console.Write(" ");
            WriteCard(cards[i]);
        }
    }
}
