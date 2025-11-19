using Microsoft; 

namespace Boothaus.Domain;

public class Boot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public double Rumpflänge 
    { 
        get;
        set
        {
            Assumes.True(value > 0, nameof(Rumpflänge) + " muss größer als 0 sein.");
            field = value;
        } 
    }

    public double Breite 
    { 
        get; 
        set
        {
            Assumes.True(value > 0, nameof(Breite) + " muss größer als 0 sein.");
            field = value;
        }
    }
    public string Kontakt { get; set; }

    public Boot()
    {
        
    }

    public Boot(Guid id, string name, double rumpflänge, double breite, string kontakt)
    {  
        Id = id;
        Name = name;
        Rumpflänge = rumpflänge;
        Breite = breite;
        Kontakt = kontakt;
    }

    public override string ToString() => Name;
}
