using Boothaus.Domain;
using Boothaus.Services.Contracts;
using System;

namespace Boothaus.Services.Persistence;

public class InMemoryAuftragRepository : IAuftragRepository
{
    private List<Lagerauftrag> aufträge = new();
    private bool initialisiert;
    public void InitialisiereMitDefaults(List<Lagerauftrag> defaultAufträge)
    {
        if (initialisiert) throw new InvalidOperationException("Das Repository wurde bereits initialisiert."); 
        aufträge = defaultAufträge;
        initialisiert = true;
    }

    public void Add(Lagerauftrag auftrag)
    {
        if (aufträge.Contains(auftrag))
        {
            throw new InvalidOperationException("Der Auftrag ist bereits im Repository vorhanden.");
        }

        aufträge.Add(auftrag);
    }

    public IEnumerable<Lagerauftrag> GetAll()
    {
        return aufträge;
    }

    public void Remove(Lagerauftrag auftrag)
    {
        if (!aufträge.Contains(auftrag))
        {
            throw new InvalidOperationException("Der Auftrag existiert nicht im Repository.");
        }
        aufträge.Remove(auftrag);
    }

    public void Update(Lagerauftrag auftrag)
    {
        var index = aufträge.FindIndex(a => a.Id == auftrag.Id);
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

    public IEnumerable<Lagerauftrag> GetBySaison(Saison saison)
    {
        return aufträge
            .Where(a => a.Saison.Anfangsjahr == saison.Anfangsjahr);
    }
}
