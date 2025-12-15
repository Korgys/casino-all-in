using casino.core.Jeux.Poker.Cartes;
using System;
using System.Collections.Generic;
using System.Text;

namespace casino.core.Jeux.Poker.Scores;

public class Score
{
    public RangMain Rang { get; set; }
    public RangCarte Valeur { get; set; }

    public override string ToString()
    {
        return $"{Rang} de {Valeur}";
    }
}
