using Boothaus.Domain;

namespace Boothaus.Services.Contracts;

public interface ILagerRepository
{
    void InitialisiereMitDefaults(Lager defaultLager);
    Lager GetLager();
    void Save(Lager lager);
}