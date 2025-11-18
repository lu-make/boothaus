using Boothaus.Domain;

namespace Boothaus.Services.Contracts;

public interface IAuftragRepository
{
    IEnumerable<Lagerauftrag> GetAll();
    void Add(Lagerauftrag auftrag);
    void Remove(Lagerauftrag auftrag);
    void Update(Lagerauftrag auftrag);
}