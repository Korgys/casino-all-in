using casino.core.Jeux.Poker.Joueurs.Strategies;

namespace casino.core.Jeux.Poker.Joueurs;

public class JoueurOrdi : Joueur
{
    public IJoueurStrategy Strategie { get; }

    public JoueurOrdi(string nom, int jetons, IJoueurStrategy? strategie = null) : base(nom, jetons)
    {
        Strategie = strategie ?? new StrategieRandom();
    }
}
