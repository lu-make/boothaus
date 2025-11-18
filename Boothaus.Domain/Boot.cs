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

    public Boot()
    {
        
    }

    public Boot(Guid id, string name, double rumpflänge, double breite)
    {  
        Id = id;
        Name = name;
        Rumpflänge = rumpflänge;
        Breite = breite;
    }

    public override string ToString() => Name;
}
