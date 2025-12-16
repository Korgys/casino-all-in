using casino.core.Jeux.Poker.Actions;

namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public interface IStrategieJoueur
{
    Actions.Action ProposerAction(ContexteDeJeu contexte);
}
