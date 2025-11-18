namespace Domain.Model;

public class Lager
{  
    public List<Lagerreihe> Reihen { get; set; } = new();

    public int AnzahlPlätze => Reihen.Sum(r => r.Plätze.Count);

    public void ReiheHinzufügen(Lagerreihe reihe)
    {
        Reihen.Add(reihe);
    } 

    public double StandardMaxBreite { get; set; }
    public double StandardMaxLänge { get; set; }
}
