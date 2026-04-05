using casino.console.Games.Commons;
using casino.core.Games.Poker;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Cards;

namespace casino.console.Games.Poker;

/// <summary>
/// Provides static methods for displaying poker-related information, such as player names, amounts, cards, and hands,
/// to the console with enhanced readability through color coding.
/// </summary>
internal static class ConsolePokerWriter
{
    private const string ResetCode = "\u001b[0m";

    public static void WritePlayerName(PokerPlayerState playerState)
    {
        ConsoleColor color;
        if (playerState.IsFolded || (playerState.Chips == 0 && playerState.LastAction != PokerTypeAction.AllIn))
            color = ConsoleColor.DarkGray;
        else if (playerState.IsHuman)
            color = ConsoleColor.Cyan;
        else
            color = ConsoleColor.DarkRed;

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
        var color = (card.Suit == Suit.Hearts || card.Suit == Suit.Diamonds)
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

    public static string FormatCard(Card card)
    {
        return WrapWithAnsi(GetCardAnsiColor(card), card.ToString());
    }

    public static string FormatPlayerName(PokerPlayerState playerState)
    {
        ConsoleColor color;
        if (playerState.IsFolded || (playerState.Chips == 0 && playerState.LastAction != PokerTypeAction.AllIn))
            color = ConsoleColor.DarkGray;
        else if (playerState.IsHuman)
            color = ConsoleColor.Cyan;
        else
            color = ConsoleColor.DarkRed;

        return WrapWithAnsi(GetAnsiColor(color), playerState.Name);
    }

    public static string FormatAmount(int amount)
    {
        return WrapWithAnsi(GetAnsiColor(ConsoleColor.Yellow), $"{amount}c");
    }

    public static string FormatHand(HandCards hand)
    {
        return string.Join(" ", hand.AsEnumerable().Select(FormatCard));
    }

    public static string FormatWinnerTag(string tag)
    {
        return WrapWithAnsi(GetAnsiColor(ConsoleColor.Green), tag);
    }

    private static string GetCardAnsiColor(Card card)
    {
        return card.Suit == Suit.Hearts || card.Suit == Suit.Diamonds
            ? "\u001b[38;2;255;0;0m"
            : "\u001b[38;2;0;255;255m";
    }

    private static string GetAnsiColor(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.DarkGray => "\u001b[90m",
            ConsoleColor.DarkRed => "\u001b[31m",
            ConsoleColor.Cyan => "\u001b[36m",
            ConsoleColor.Yellow => "\u001b[33m",
            ConsoleColor.Green => "\u001b[32m",
            _ => "\u001b[37m"
        };
    }

    private static string WrapWithAnsi(string colorCode, string value)
    {
        return $"{colorCode}{value}{ResetCode}";
    }
}
