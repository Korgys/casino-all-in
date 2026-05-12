using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class CartesCommunesTests
{
    [TestMethod]
    public void AsEnumerable_QuandAucuneCarte_DoitRetournerListeVide()
    {
        // Arrange
        var communityCards = new TableCards();

        // Act
        var cards = communityCards.AsEnumerable().ToList();

        // Assert
        Assert.IsEmpty(cards);
    }

    [TestMethod]
    public void AsEnumerable_DoitRetournerCartesNonNull_DansLOrdreFlopTurnRiver()
    {
        // Arrange
        var flop1 = new Card(CardRank.Two, Suit.Diamonds);
        var flop2 = new Card(CardRank.Three, Suit.Hearts);
        var flop3 = new Card(CardRank.Four, Suit.Spades);
        var turn = new Card(CardRank.Ace, Suit.Clubs);
        var river = new Card(CardRank.King, Suit.Hearts);

        var communityCards = new TableCards
        {
            Flop1 = flop1,
            Flop2 = flop2,
            Flop3 = flop3,
            Turn = turn,
            River = river
        };

        // Act
        var cards = communityCards.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(5, cards);
        Assert.AreSame(flop1, cards[0]);
        Assert.AreSame(flop2, cards[1]);
        Assert.AreSame(flop3, cards[2]);
        Assert.AreSame(turn, cards[3]);
        Assert.AreSame(river, cards[4]);
    }

    [TestMethod]
    public void ToString_QuandAucuneCarte_DoitRetournerChaineVide()
    {
        // Arrange
        var communityCards = new TableCards();

        // Act
        var s = communityCards.ToString();

        // Assert
        Assert.AreEqual(string.Empty, s);
    }

    [TestMethod]
    public void ToString_QuandCartesPresentes_DoitFaireJoinAvecVirgules()
    {
        // Arrange
        var communityCards = new TableCards
        {
            Flop1 = new Card(CardRank.Ace, Suit.Spades),   // "A Pique"
            Turn = new Card(CardRank.Ten, Suit.Hearts)    // "10 Coeur"
        };

        // Act
        var s = communityCards.ToString();

        // Assert
        Assert.AreEqual("A♠, 10♥", s);
    }

    [TestMethod]
    public void AsEnumerable_QuandCartesIntermediairesNull_DoitIgnorerLesNulls()
    {
        // Arrange
        var flop1 = new Card(CardRank.Ace, Suit.Spades);
        var river = new Card(CardRank.Queen, Suit.Diamonds);

        var communityCards = new TableCards
        {
            Flop1 = flop1,
            // Flop2 null
            // Flop3 null
            // Turn null
            River = river
        };

        // Act
        var cards = communityCards.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(2, cards);
        Assert.AreSame(flop1, cards[0]);
        Assert.AreSame(river, cards[1]);
    }
}
