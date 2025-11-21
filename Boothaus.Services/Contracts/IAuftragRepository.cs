using Boothaus.Domain;

namespace Boothaus.Services.Contracts;

public interface IAuftragRepository
{
    void InitialisiereMitDefaults(List<Auftrag> defaultAufträge);
    IEnumerable<Auftrag> GetAll();
    IEnumerable<Saison> GetSaisons();
    IEnumerable<Auftrag> GetBySaison(Saison saison);
    Auftrag? Get(Guid id);
    void Add(Auftrag auftrag);
    void Remove(Auftrag auftrag);
    void Update(Auftrag auftrag);
    void Upsert(Auftrag auftrag);
}