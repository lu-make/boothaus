namespace Boothaus.Domain;

public class Lagerplatz : ModelBase
{ 
    public Lagerreihe? Reihe { get; internal set; } 

    private readonly List<Auftrag> zuweisungen = new();
    public IReadOnlyCollection<Auftrag> Zuweisungen => zuweisungen;


    public Lagerplatz() : base(Guid.NewGuid())
    {

    } 

    public void ZuweisungHinzufügen(Auftrag auftrag)
    { 
        if (!IstFreiImZeitraum(auftrag.Von, auftrag.Bis))
        {
            throw new InvalidOperationException("Der Lagerplatz ist im angegebenen Zeitraum bereits belegt.");
        }

        auftrag.Platz = this;
        zuweisungen.Add(auftrag);
    }

    public void ZuweisungEntfernen(Auftrag auftrag)
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

    public Auftrag? GetZuweisung(DateOnly datum)
    {
        return zuweisungen.FirstOrDefault(z => z.Von <= datum && z.Bis >= datum);
    }

}
