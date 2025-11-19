namespace Boothaus.Domain;

public class LagerplatzZuweisung
{
    public Lagerauftrag Auftrag { get; set; }
    public Lagerplatz Platz 
    { 
        get; 
        set
        {
            if (value is null) return;
            value.FügeZuweisungHinzu(this);
            field = value;
        }
    }
    public LagerplatzZuweisung(Lagerauftrag auftrag, Lagerplatz platz)
    {   
        Auftrag = auftrag;
        Platz = platz;
        platz?.FügeZuweisungHinzu(this);
    }
}
