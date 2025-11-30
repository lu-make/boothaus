using Boothaus.Domain;
using Boothaus.Services.Contracts; 

namespace Boothaus.Services.Persistence;

public class AuftragRepository : IAuftragRepository
{
    private List<Auftrag> aufträge = new();
    private bool initialisiert;
    public void InitialisiereMitDefaults(List<Auftrag> defaultAufträge)
    {
        if (initialisiert) throw new InvalidOperationException("Das Repository wurde bereits initialisiert."); 
        aufträge = defaultAufträge;
        initialisiert = true;
    }

    public Auftrag? Get(Guid id)
    {
        return aufträge.FirstOrDefault(a => a.Id == id);
    }

    public void Add(Auftrag auftrag)
    {
        if (aufträge.Contains(auftrag))
        {
            throw new InvalidOperationException("Der Auftrag ist bereits im Repository vorhanden.");
        }

        aufträge.Add(auftrag);
    }

    public IEnumerable<Auftrag> GetAll()
    {
        return aufträge;
    }

    public void Remove(Auftrag auftrag)
    {
        if (!aufträge.Contains(auftrag))
        {
            throw new InvalidOperationException("Der Auftrag existiert nicht im Repository.");
        }
        aufträge.Remove(auftrag);
    }

    public void Update(Auftrag auftrag)
    {
        var index = aufträge.IndexOf(auftrag);
        if (index != -1)
        {
            aufträge[index] = auftrag;
        }
        else
        {
            throw new InvalidOperationException("Der Auftrag existiert nicht im Repository.");
        }
    }

    public IEnumerable<Saison> GetSaisons()
    {
        return aufträge
            .Select(a => a.Saison)
            .Distinct()
            .OrderBy(s => s.Anfangsjahr);
    }

    public IEnumerable<Auftrag> GetBySaison(Saison saison)
    {
        return aufträge
            .Where(a => a.Saison.Anfangsjahr == saison.Anfangsjahr);
    }

    public void Upsert(Auftrag auftrag)
    {
        var index = aufträge.IndexOf(auftrag);
        if (index != -1)
        {
            aufträge[index] = auftrag;
        }
        else
        {
            aufträge.Add(auftrag);
        }
    }
}