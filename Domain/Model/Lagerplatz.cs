namespace Domain.Model;

public class Lagerplatz
{
    public Guid Id { get; set; }
    public Lagerreihe? Reihe { get; internal set; }

}
