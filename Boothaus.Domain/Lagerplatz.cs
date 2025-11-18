namespace Boothaus.Domain;

public class Lagerplatz
{
    public Guid Id { get; set; }
    public Lagerreihe? Reihe { get; internal set; }

}
