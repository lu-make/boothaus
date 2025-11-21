using Boothaus.Domain;
using Boothaus.Services.Contracts;

namespace Boothaus.Services.Persistence;

public class InMemoryBootRepository : IBootRepository
{
    private List<Boot> boote = new();
    private bool initialisiert;
    public void InitialisiereMitDefaults(List<Boot> defaultBoote)
    {
        if (initialisiert) throw new InvalidOperationException("Das Repository wurde bereits initialisiert.");
        boote = defaultBoote;
        initialisiert = true;
    }

    public Boot? Get(Guid id)
    {
        return boote.FirstOrDefault(b => b.Id == id);
    }

    public void Add(Boot boot)
    {
        if (boote.Contains(boot))
        {
            throw new InvalidOperationException("Das Boot ist bereits im Repository vorhanden.");
        }
        boote.Add(boot);
    }

    public IEnumerable<Boot> GetAll()
    {
        return boote;
    }

    public void Remove(Boot boot)
    {
        if (!boote.Contains(boot)) 
        {
            throw new InvalidOperationException("Das Boot existiert nicht im Repository.");
        }

        boote.Remove(boot);
    }

    public void Update(Boot boot)
    {
        var index = boote.FindIndex(b => b.Id == boot.Id);
        if (index != -1)
        {
            boote[index] = boot;
        }
        else
        {
            throw new InvalidOperationException("Das Boot existiert nicht im Repository.");
        }
    }

    public void Upsert(Boot boot)
    {
        if (!boote.Contains(boot))
        {
            Add(boot);
        }
        else
        {
            Update(boot);
        }
    }
}
