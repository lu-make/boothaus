namespace Boothaus.Domain;

public struct Saison
{ 
    public string Bezeichnung => $"{Anfangsjahr}/{Endjahr}";
    public int Anfangsjahr { get; init; }
    public int Endjahr => Anfangsjahr + 1;

    public Saison(int anfangsjahr)
    {
        Anfangsjahr = anfangsjahr;
    }

    public override string ToString() => Bezeichnung;
}
