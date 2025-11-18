using Boothaus.Domain;

namespace Boothaus.Services.Contracts;

public interface ILagerRepository
{
    Lager GetLager();
    void Save(Lager lager);
}