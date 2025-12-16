using casino.core.Jeux.Poker.Joueurs.Strategies;

namespace casino.core.Jeux.Poker.Joueurs;

public class JoueurOrdi : Joueur
{
    public IStrategieJoueur Strategie { get; }

    public JoueurOrdi(string nom, int jetons, IStrategieJoueur? strategie = null) : base(nom, jetons)
    {
        Strategie = strategie ?? new StrategieRandom();
    }
}
