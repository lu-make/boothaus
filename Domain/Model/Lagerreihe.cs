namespace Domain.Model;

public class Lagerreihe
{
    public int Nummer { get; set; }
    public Lager Lager { get; set; } 
    public List<Lagerplatz> Plätze { get; set; }
    public Lagerplatz this[int index]
    {
        get => Plätze[index];
        set => Plätze[index] = value!;
    }

    public Lagerreihe(int nummer, Lager lager)
    {
        Nummer = nummer;
        Plätze = new List<Lagerplatz>();
        Lager = lager;
    }

    public void PlatzHinzufügen(Lagerplatz platz)
    {
        if (platz.Reihe is not null)
        {
            throw new ArgumentException("Der Lagerplatz gehört bereits zu einer Reihe.");
        }

        Plätze.Add(platz);
        platz.Reihe = this;
    }

    public void PlatzEntfernen(Lagerplatz platz)
    {
        bool entfernt = Plätze.Remove(platz);
        
        if (!entfernt)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }

        platz.Reihe = null;
    }

    public IEnumerable<Lagerplatz> PlätzeVor(Lagerplatz platz)
    {
        var index = Plätze.IndexOf(platz);
        if (index == -1)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }
        return Plätze.Take(index);
    }

    public IEnumerable<Lagerplatz> PlätzeNach(Lagerplatz platz)
    {
        var index = Plätze.IndexOf(platz);
        if (index == -1)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }
        return Plätze.Skip(index + 1);
    }

}
