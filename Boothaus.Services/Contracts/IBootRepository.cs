using Boothaus.Domain;

namespace Boothaus.Services.Contracts;

public interface IBootRepository
{
    void InitialisiereMitDefaults(List<Boot> defaultBoote);
    IEnumerable<Boot> GetAll();

    Boot? Get(Guid id);
    void Add(Boot boot);
    void Update(Boot boot);
    void Remove(Boot boot);
    void Upsert(Boot boot);
}