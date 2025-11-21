using Microsoft;

namespace Boothaus.Domain;

public class Lager
{  
    public decimal StandardMaxBreite
    {
        get;
        set
        {
            Assumes.True(value > 0);
            field = value;
        }
    }

    public decimal StandardMaxLänge 
    { 
        get; 
        set
        {
            Assumes.True(value > 0);
            field = value;
        }
    }
    
    public List<Lagerreihe> Reihen { get; }

    public void ReiheUpsert(Lagerreihe reihe)
    {
        reihe.Lager = this;
        Reihen.Add(reihe);
    }

    public Lager(decimal standardMaxBreite, decimal standardMaxLänge)
    {
        Assumes.True(standardMaxBreite > 0);
        Assumes.True(standardMaxLänge > 0);
        
        Reihen = [];
        StandardMaxBreite = standardMaxBreite;
        StandardMaxLänge = standardMaxLänge;
    }

    public bool Passt(Boot boot)
    {
        return Passt(boot.Rumpflänge, boot.Breite);
    }

    public bool Passt(decimal länge, decimal breite)
    {
        return breite <= StandardMaxBreite && länge <= StandardMaxLänge;
    }
}