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

    public Boot(Guid id, string name, double rumpflänge, double breite, string kontakt) : base(id)
    {   
        Name = name;
        Rumpflänge = rumpflänge;
        Breite = breite;
        Kontakt = kontakt;
    }

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is not Boot anderes)
            return false;
        return Id == anderes.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
