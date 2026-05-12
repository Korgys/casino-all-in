using casino.core.Games.Poker.Cards;

namespace casino.core.tests.Games.Poker.Cards;

[TestClass]
public class CarteTests
{
    [TestMethod]
    public void Constructeur_DoitInitialiserRangEtSuit()
    {
        // Arrange
        var rank = CardRank.Ace;
        var suit = Suit.Hearts;

        // Act
        var carte = new Card(rank, suit);

        // Assert
        Assert.AreEqual(rank, carte.Rank, "Le rank devrait être celui passé au constructeur.");
        Assert.AreEqual(suit, carte.Suit, "La suit devrait être celle passée au constructeur.");
    }

    [TestMethod]
    public void ToString_DoitRetournerRangCourtEtSuit()
    {
        // Arrange
        var carte = new Card(CardRank.Ace, Suit.Spades);

        // Act
        var result = carte.ToString();

        // Assert
        Assert.AreEqual("A♠", result, "Le format ToString n'est pas conforme.");
    }

    [TestMethod]
    public void ToString_DoitEtreDeterministe()
    {
        // Arrange
        var carte = new Card(CardRank.Ten, Suit.Diamonds);

        // Act
        var first = carte.ToString();
        var second = carte.ToString();

        // Assert
        Assert.AreEqual(first, second, "ToString devrait toujours retourner la même value.");
    }

    [TestMethod]
    public void Equals_DeuxCartesIdentiques_DoitRetournerTrue()
    {
        // Arrange
        var carte1 = new Card(CardRank.King, Suit.Clubs);
        var carte2 = new Card(CardRank.King, Suit.Clubs);

        // Act
        var equals = carte1.Equals(carte2);

        // Assert
        Assert.IsTrue(equals, "Deux cards avec même rank et suit devraient être égales.");
    }

    [TestMethod]
    public void Equals_DeuxCartesDifferentes_DoitRetournerFalse()
    {
        // Arrange
        var carte1 = new Card(CardRank.King, Suit.Clubs);
        var carte2 = new Card(CardRank.King, Suit.Hearts);

        // Act
        var equals = carte1.Equals(carte2);

        // Assert
        Assert.IsFalse(equals, "Deux cards avec une suit différente ne devraient pas être égales.");
    }

    [TestMethod]
    public void GetHashCode_DeuxCartesIdentiques_DoitRetournerMemeValeur()
    {
        // Arrange
        var carte1 = new Card(CardRank.Queen, Suit.Hearts);
        var carte2 = new Card(CardRank.Queen, Suit.Hearts);

        // Act
        var hash1 = carte1.GetHashCode();
        var hash2 = carte2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2, "Deux cards identiques doivent avoir le même hash.");
    }
}
