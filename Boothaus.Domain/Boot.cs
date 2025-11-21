using Microsoft; 

namespace Boothaus.Domain;

public class Boot : ModelBase
{
    public string Name { get; set; } = "";
    public decimal Rumpflänge 
    { 
        get;
        set
        {
            Assumes.True(value > 0, nameof(Rumpflänge) + " muss größer als 0 sein.");
            field = value;
        } 
    }

    public decimal Breite 
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

    public Boot(Guid id, string name, string kontakt, decimal rumpflänge, decimal breite) : base(id)
    {
        Name = name;
        Kontakt = kontakt;
        Rumpflänge = rumpflänge;
        Breite = breite;
    }

    public Boot(string name, string kontakt, decimal rumpflänge, decimal breite) 
        : this(Guid.NewGuid(), name, kontakt, rumpflänge, breite) { }

    public override string ToString() => Name;
     
}
