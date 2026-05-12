using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class CartesMainTests
{
    [TestMethod]
    public void Constructeur_DoitInitialiserFirstEtSecond()
    {
        // Arrange
        var c1 = new Card(CardRank.Ace, Suit.Spades);
        var c2 = new Card(CardRank.King, Suit.Hearts);

        // Act
        var main = new HandCards(c1, c2);

        // Assert
        Assert.AreSame(c1, main.First);
        Assert.AreSame(c2, main.Second);
    }

    [TestMethod]
    public void AsEnumerable_DoitRetournerDeuxCartes_DansLeBonOrdre()
    {
        // Arrange
        var c1 = new Card(CardRank.Ace, Suit.Spades);
        var c2 = new Card(CardRank.King, Suit.Hearts);
        var main = new HandCards(c1, c2);

        // Act
        var cards = main.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(2, cards);
        Assert.AreSame(c1, cards[0]);
        Assert.AreSame(c2, cards[1]);
    }

    [TestMethod]
    public void AsEnumerable_QuandSecondEstNull_DoitRetournerSeulementFirst()
    {
        // Arrange
        var c1 = new Card(CardRank.Ace, Suit.Spades);
        var main = new HandCards(c1, second: null!);

        // Act
        var cards = main.AsEnumerable().ToList();

        // Assert
        Assert.HasCount(1, cards);
        Assert.AreSame(c1, cards[0]);
    }

    [TestMethod]
    public void ToString_DoitConcatenerLesDeuxCartes()
    {
        // Arrange
        var c1 = new Card(CardRank.Ace, Suit.Spades);     // "A Pique"
        var c2 = new Card(CardRank.King, Suit.Hearts);    // "K Coeur"
        var main = new HandCards(c1, c2);

        // Act
        var s = main.ToString();

        // Assert
        Assert.AreEqual("A♠, K♥", s);
    }
}
