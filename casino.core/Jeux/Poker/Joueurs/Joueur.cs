using casino.core.Jeux.Poker.Actions;
using casino.core.Jeux.Poker.Cartes;

namespace casino.core.Jeux.Poker.Joueurs;

public class Joueur
{
    public string Nom { get; set; }

    private int _jetons;
    public int Jetons
    {
        get => _jetons;
        set => _jetons = value > 0 ? value : 0;
    }

    public CartesMain Main { get; set; }

    public bool EstCouche { get; set; } = false;
    public bool EstTapis { get; set; } = false;
    public TypeActionJeu DerniereAction { get; internal set; }

    public Joueur(string nom, int jetons)
    {
        Nom = nom;
        Jetons = jetons;
    }

    internal void Reinitialiser()
    {
        DerniereAction = Jetons > 0 ? TypeActionJeu.Aucune : TypeActionJeu.SeCoucher;
        EstCouche = Jetons < 0;
        EstTapis = false;
    }
}
