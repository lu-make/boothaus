using Microsoft;

namespace Boothaus.Domain;

public class Lager
{  
    public double StandardMaxBreite { get; set; }
    public double StandardMaxLänge { get; set; }
    
    public List<Lagerreihe> Reihen { get; }

    public int AnzahlPlätze => Reihen.Sum(r => r.Plätze.Count);

    public void ReiheUpsert(Lagerreihe reihe)
    {
        reihe.Lager = this;
        Reihen.Add(reihe);
    }

    public Lager(double standardMaxBreite, double standardMaxLänge)
    {
        Assumes.True(standardMaxBreite > 0);
        Assumes.True(standardMaxLänge > 0);
        
        Reihen = [];
        StandardMaxBreite = standardMaxBreite;
        StandardMaxLänge = standardMaxLänge;
    }

    public bool Passt(Boot boot)
    {
        return boot.Breite <= StandardMaxBreite && boot.Rumpflänge <= StandardMaxLänge;
    }
}