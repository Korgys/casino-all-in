namespace casino.core.Games.Roulette;

public sealed record RouletteBet(RouletteBetKind Kind, int Amount, int? Number = null);
