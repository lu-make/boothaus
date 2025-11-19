namespace Boothaus.Domain;

public class Lagerplatz
{
    public Guid Id { get; set; }
    public Lagerreihe? Reihe { get; internal set; } 

    private readonly List<LagerplatzZuweisung> zuweisungen = new();
    public IReadOnlyCollection<LagerplatzZuweisung> Zuweisungen => zuweisungen;
 
    internal void FügeZuweisungHinzu(LagerplatzZuweisung zuweisung)
    { 
        zuweisungen.Add(zuweisung);
    }

    public bool IstFreiImZeitraum(DateOnly von, DateOnly bis)
    { 
        return !zuweisungen.Any(z => z.Auftrag.Von <= bis && z.Auftrag.Bis >= von);
    }
}
