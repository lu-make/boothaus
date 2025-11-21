using Microsoft; 

namespace Boothaus.Domain;

public class Boot : ModelBase
{
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

    public Boot() : base (Guid.NewGuid())
    {
        
    }

    public Boot(string name, string kontakt, double rumpflänge, double breite) : base(Guid.NewGuid())
    {   
        Name = name;
        Kontakt = kontakt;
        Rumpflänge = rumpflänge;
        Breite = breite;
    }

    public override string ToString() => Name;
     
}
