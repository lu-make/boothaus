namespace Boothaus.Domain;

/// <summary>
/// Das Ergebnis des ganzen Vorgangs.
/// </summary>
public class Lagerkalender
{
    public List<LagerplatzZuweisung> Zuweisungen { get; set; } = new();

}