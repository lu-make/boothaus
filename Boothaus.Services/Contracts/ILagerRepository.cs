using Boothaus.Domain;

namespace Boothaus.Services.Contracts;

public interface ILagerRepository
{
    void InitialisiereMitDefaults(Lager defaultLager);
    void InitialisiereLeer();
    Lager GetLager();
    void Save(Lager lager); 
    void Clear();
}