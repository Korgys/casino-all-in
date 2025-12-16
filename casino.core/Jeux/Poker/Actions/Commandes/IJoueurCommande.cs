using casino.core.Jeux.Poker.Parties;

namespace casino.core.Jeux.Poker.Actions.Commandes;

public interface IJoueurCommande
{
    void Execute(Partie partie);
}
