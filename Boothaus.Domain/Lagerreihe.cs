namespace Boothaus.Domain;

public class Lagerreihe
{
    public int Nummer { get; set; }
    public Lager? Lager { get; set; } 

    private List<Lagerplatz> plätze = [];

    public IReadOnlyList<Lagerplatz> Plätze => plätze;
    public Lagerplatz this[int index]
    {
        get => Plätze[index]; 
    }

    public Lagerreihe(int nummer)
    {
        Nummer = nummer; 
    }

    public void PlatzHinzufügen(Lagerplatz platz)
    {
        if (platz.Reihe is not null)
        {
            throw new ArgumentException("Der Lagerplatz gehört bereits zu einer Reihe.");
        }

        platz.Reihe = this;
        plätze.Add(platz);
    }

    public void PlatzEntfernen(Lagerplatz platz)
    {
        bool entfernt = plätze.Remove(platz);
        
        if (!entfernt)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }

        platz.Reihe = null;
    }

    public int Index(Lagerplatz platz)
    {
        var index = plätze.IndexOf(platz);
        if (index == -1)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }
        return index;
    }

    public IEnumerable<Lagerplatz> PlätzeVor(Lagerplatz platz)
    {
        var index = plätze.IndexOf(platz);
        if (index == -1)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }

        return Plätze.Take(index);
    }

    public IEnumerable<Lagerplatz> PlätzeNach(Lagerplatz platz)
    {
        var index = plätze.IndexOf(platz);
        if (index == -1)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }
        return Plätze.Skip(index + 1);
    }

}
