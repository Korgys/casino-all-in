namespace casino.core.Jeux.Poker.Joueurs.Strategies;

public interface IJoueurStrategy
{
    JoueurAction ProposerAction(ContexteDeJeu contexte);
}
