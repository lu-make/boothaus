using Boothaus.Domain;

namespace Boothaus.Services.Contracts;

public interface IAuftragRepository
{
    void InitialisiereMitDefaults(List<Auftrag> defaultAufträge);
    IEnumerable<Auftrag> GetAll();
    IEnumerable<Saison> GetSaisons();
    IEnumerable<Auftrag> GetBySaison(Saison saison);
    void Add(Auftrag auftrag);
    void Remove(Auftrag auftrag);
    void Update(Auftrag auftrag);
}