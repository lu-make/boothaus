using Boothaus.Domain;
using Boothaus.Services.Contracts;

namespace Boothaus.Services.Persistence;

public class LagerRepository : ILagerRepository
{
    private Lager? lager;

    private bool initialisiert;
    
    public void InitialisiereMitDefaults(Lager lager)
    {
        if (initialisiert) throw new InvalidOperationException("Das Repository wurde bereits initialisiert.");
        this.lager = lager;
        initialisiert = true;
    }

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