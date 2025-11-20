namespace Boothaus.Domain;

public abstract class ModelBase
{
    public Guid Id { get; init; }

    protected ModelBase(Guid id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ModelBase anderes)
            return false;
        return Id == anderes.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}