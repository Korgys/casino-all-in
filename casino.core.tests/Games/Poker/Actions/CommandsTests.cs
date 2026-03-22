using System;
using System.Collections.Generic;
using System.Linq;
using casino.core.Games.Poker.Actions;
using casino.core.Games.Poker.Actions.Commands;
using casino.core.Games.Poker.Cards;
using casino.core.Games.Poker.Players;
using casino.core.Games.Poker.Rounds;
using casino.core.tests.Fakes;
using casino.core.tests.Games.Poker.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace casino.core.tests.Games.Poker.Actions.Commands;

[TestClass]
public class CommandsTests
{
    private static IEnumerable<Card> CreateSimpleCards() => Enumerable.Repeat(new Card(CardRank.Deux, Suit.Hearts), 10);

    [TestMethod]
    public void CheckCommand_WhenThereIsNoBet_ShouldSetLastActionToCheck()
    {
        var player = new HumanPlayer("Alice", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        var command = new CheckCommand(player);
        command.Execute(round);

        Assert.AreEqual(PokerTypeAction.Check, player.LastAction);
    }

    [TestMethod]
    public void CheckCommand_WhenCurrentBetIsMatched_ShouldSetLastActionToCheck()
    {
        var player = new HumanPlayer("Alice", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        PlayerTestHelper.SetCurrentBet(round, 10);
        round.SetBetFor(player, 10);

        var command = new CheckCommand(player);
        command.Execute(round);

        Assert.AreEqual(PokerTypeAction.Check, player.LastAction);
    }

    [TestMethod]
    public void CheckCommand_WhenCurrentBetIsNotZero_ShouldThrowInvalidOperationException()
    {
        var player = new HumanPlayer("Alice", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        PlayerTestHelper.SetCurrentBet(round, 10);

        var command = new CheckCommand(player);
        Assert.Throws<InvalidOperationException>(() => command.Execute(round));
    }

    [TestMethod]
    public void BetCommand_WhenValid_ShouldUpdatePotCurrentBetAndChips()
    {
        var player = new HumanPlayer("Bob", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        var command = new BetCommand(player, 25);
        command.Execute(round);

        Assert.AreEqual(PokerTypeAction.Bet, player.LastAction);
        Assert.AreEqual(75, player.Chips);
        Assert.AreEqual(25, round.CurrentBet);
        Assert.AreEqual(25, round.GetBetFor(player));
        Assert.AreEqual(25, round.Pot);
    }

    [TestMethod]
    public void BetCommand_WhenAmountIsZero_ShouldThrowArgumentException()
    {
        var player = new HumanPlayer("Bob", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        var command = new BetCommand(player, 0);
        Assert.Throws<ArgumentException>(() => command.Execute(round));
    }

    [TestMethod]
    public void RaiseCommand_WhenValid_ShouldUpdateBetPotAndChips()
    {
        var player = new HumanPlayer("Carol", 200);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        PlayerTestHelper.SetCurrentBet(round, 10);

        var command = new RaiseCommand(player, 50);
        command.Execute(round);

        Assert.AreEqual(PokerTypeAction.Raise, player.LastAction);
        Assert.AreEqual(150, player.Chips);
        Assert.AreEqual(50, round.CurrentBet);
        Assert.AreEqual(50, round.GetBetFor(player));
        Assert.AreEqual(50, round.Pot);
    }

    [TestMethod]
    public void RaiseCommand_WhenAmountConsumesAllChips_ShouldBecomeAllIn()
    {
        var player = new HumanPlayer("Dave", 20);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        var command = new RaiseCommand(player, 20);
        command.Execute(round);

        Assert.AreEqual(PokerTypeAction.AllIn, player.LastAction);
        Assert.AreEqual(0, player.Chips);
        Assert.AreEqual(20, round.GetBetFor(player));
        Assert.IsGreaterThanOrEqualTo(20, round.CurrentBet);
        Assert.IsGreaterThanOrEqualTo(20, round.Pot);
    }

    [TestMethod]
    public void RaiseCommand_WhenAmountIsNotHigherThanCurrentBet_ShouldThrowArgumentException()
    {
        var player = new HumanPlayer("Eve", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        PlayerTestHelper.SetCurrentBet(round, 50);

        var command = new RaiseCommand(player, 50);
        Assert.Throws<ArgumentException>(() => command.Execute(round));
    }

    [TestMethod]
    public void CallCommand_WhenValid_ShouldReduceChipsAndIncreasePot()
    {
        var player = new HumanPlayer("Frank", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        PlayerTestHelper.SetCurrentBet(round, 40);
        round.SetBetFor(player, 10);

        var command = new CallCommand(player);
        command.Execute(round);

        Assert.AreEqual(PokerTypeAction.Call, player.LastAction);
        Assert.AreEqual(70, player.Chips);
        Assert.AreEqual(40, round.GetBetFor(player));
        Assert.IsGreaterThanOrEqualTo(30, round.Pot);
    }

    [TestMethod]
    public void CallCommand_WhenThereIsNoDifferenceToCall_ShouldThrowInvalidOperationException()
    {
        var player = new HumanPlayer("Gina", 100);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        PlayerTestHelper.SetCurrentBet(round, 0);
        round.SetBetFor(player, 0);

        var command = new CallCommand(player);
        Assert.Throws<InvalidOperationException>(() => command.Execute(round));
    }

    [TestMethod]
    public void AllInCommand_ShouldSetChipsToZeroAndUpdateContribution()
    {
        var player = new HumanPlayer("Hank", 30);
        var round = new Round(new List<Player> { player }, new FakeDeck(CreateSimpleCards()));

        round.SetBetFor(player, 5);

        var command = new AllInCommand(player);
        command.Execute(round);

        Assert.AreEqual(PokerTypeAction.AllIn, player.LastAction);
        Assert.AreEqual(0, player.Chips);
        Assert.AreEqual(35, round.GetBetFor(player));
        Assert.IsGreaterThanOrEqualTo(35, round.CurrentBet);
        Assert.IsGreaterThanOrEqualTo(30, round.Pot);
    }
}
