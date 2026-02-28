using casino.core.Games.Poker.Actions;

namespace casino.core.Games.Poker.Players.Strategies;

public interface IStrategiePlayer
{
    Actions.ActionJeu ProposerAction(ContexteDeJeu contexte);
}
