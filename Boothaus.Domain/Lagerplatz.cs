namespace Boothaus.Domain;

public class Lagerplatz : ModelBase
{ 
    public Lagerreihe? Reihe { get; internal set; } 

    private readonly List<Lagerauftrag> zuweisungen = new();
    public IReadOnlyCollection<Lagerauftrag> Zuweisungen => zuweisungen;


    public Lagerplatz() : base(Guid.NewGuid())
    {

    } 

    public void ZuweisungHinzufügen(Lagerauftrag auftrag)
    { 
        if (!IstFreiImZeitraum(auftrag.Von, auftrag.Bis))
        {
            throw new InvalidOperationException("Der Lagerplatz ist im angegebenen Zeitraum bereits belegt.");
        }

        auftrag.Platz = this;
        zuweisungen.Add(auftrag);
    }

    public void ZuweisungEntfernen(Lagerauftrag auftrag)
    {
        if (!zuweisungen.Contains(auftrag))
        {
            throw new InvalidOperationException("Die Zuweisung existiert nicht für diesen Lagerplatz.");
        }

        auftrag.Platz = null;
        zuweisungen.Remove(auftrag);
    }

    public void ZuweisungenLeeren()
    {
        foreach (var zuweisung in zuweisungen)
        {
            zuweisung.Platz = null;
        }
        zuweisungen.Clear();
    }

    public bool IstFreiImZeitraum(DateOnly von, DateOnly bis)
    { 
        return !zuweisungen.Any(z => z.Von <= bis && z.Bis >= von);
    }

    public Lagerauftrag? GetZuweisung(DateOnly datum)
    {
        return zuweisungen.FirstOrDefault(z => z.Von <= datum && z.Bis >= datum);
    }

}
