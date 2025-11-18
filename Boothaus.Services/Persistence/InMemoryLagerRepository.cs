using Boothaus.Domain;
using Boothaus.Services.Contracts;

namespace Boothaus.Services.Persistence;

public class InMemoryLagerRepository : ILagerRepository
{
    private Lager? lager;

    public Lager GetLager()
    {
        if (lager is not null) return lager;
        throw new InvalidOperationException("Es ist kein Lager definiert.");
    }

    public void Save(Lager lager)
    {
        this.lager = lager;
    }
}