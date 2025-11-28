namespace Boothaus.Domain;

public class Lagerplatz : ModelBase
{ 
    public Lagerreihe Reihe { get; internal set; } 

    private readonly List<Auftrag> zuweisungen = new();
    public IReadOnlyCollection<Auftrag> Zuweisungen => zuweisungen;

    public Lagerplatz(Guid id, Lagerreihe reihe) : base(id)
    {
        Reihe = reihe;
    }

    public Lagerplatz(Lagerreihe reihe) : this(Guid.NewGuid(), reihe)
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


    public void ZuweisungenLeerenInSaison(Saison saison)
    {
        foreach (var zuweisung in zuweisungen.Where(z => z.Saison.Equals(saison)).ToList())
        {
            zuweisung.Platz = null;
            zuweisungen.Remove(zuweisung);
        }
    }

    public bool IstFreiImZeitraum(DateOnly von, DateOnly bis)
    { 
        return !zuweisungen.Any(z => z.Von <= bis && z.Bis >= von);
    }

    public Auftrag? GetNächsteZuweisung(Saison saison)
    {
        return zuweisungen
            .Where(z => z.Saison.Equals(saison))
            .OrderBy(z => z.Von)
            .FirstOrDefault();
    }

}
