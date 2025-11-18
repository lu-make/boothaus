using Boothaus.Domain;
using Boothaus.Services.Contracts;

namespace Boothaus.Services.Persistence;

public class InMemoryAuftragRepository : IAuftragRepository
{
    private List<Lagerauftrag> aufträge = new();

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
}
