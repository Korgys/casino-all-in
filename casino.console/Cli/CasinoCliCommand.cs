using casino.core.Games.Poker;

namespace casino.console.Cli;

internal sealed record CasinoCliCommand(CasinoGameKind Game, PokerGameSetup? PokerSetup);
