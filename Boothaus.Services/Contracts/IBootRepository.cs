using Boothaus.Domain;

namespace Boothaus.Services.Contracts;

public interface IBootRepository
{
    IEnumerable<Boot> GetAll();
    void Add(Boot boot);
    void Update(Boot boot);
    void Remove(Boot boot);
}