using Microsoft; 

namespace Boothaus.Domain;

public class Boot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public double Rumpflänge { get; set; }
    public double Breite { get; set; }

    public Boot(Guid id, string name, double rumpflänge, double breite)
    { 
        Assumes.True(rumpflänge > 0, nameof(rumpflänge) + " muss größer als 0 sein.");
        Assumes.True(breite > 0, nameof(breite) + " muss größer als 0 sein.");
         
        Id = id;
        Name = name;
        Rumpflänge = rumpflänge;
        Breite = breite;
    }
}
