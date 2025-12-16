using casino.core.Jeux.Poker.Parties;

namespace casino.core.Jeux.Poker.Actions;

public class RequeteAction
{
    public RequeteAction(string joueurNom, IReadOnlyList<TypeActionJeu> actionsPossibles, int miseMinimum, int miseActuelle, int pot, object etatTable)
    {
        JoueurNom = joueurNom;
        ActionsPossibles = actionsPossibles;
        MiseMinimum = miseMinimum;
        MiseActuelle = miseActuelle;
        Pot = pot;
        EtatTable = etatTable;
    }

    public string JoueurNom { get; }

    public IReadOnlyList<TypeActionJeu> ActionsPossibles { get; }

    public int MiseMinimum { get; }

    public int MiseActuelle { get; }

    public int Pot { get; }

    public object EtatTable { get; }
}
