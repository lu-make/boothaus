namespace Domain.Model;

public class Boot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public double Rumpflänge { get; set; }
    public double Breite { get; set; }
    public double Gewicht { get; set; } 
}
