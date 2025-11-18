namespace Domain.Model;

public class LagerplatzZuweisung
{
    public Lagerauftrag Auftrag { get; set; }
    public Lagerplatz Platz { get; set; }

    public LagerplatzZuweisung(Lagerauftrag auftrag, Lagerplatz platz)
    {   
        Auftrag = auftrag;
        Platz = platz;
    }
}
