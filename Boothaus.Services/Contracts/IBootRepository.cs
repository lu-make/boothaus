using Boothaus.Domain;

namespace Boothaus.Services.Contracts;

public interface IBootRepository
{
    void InitialisiereMitDefaults(List<Boot> defaultBoote);
    IEnumerable<Boot> GetAll();
    void Add(Boot boot);
    void Update(Boot boot);
    void Remove(Boot boot);
}