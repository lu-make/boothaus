using Boothaus.Domain;

namespace Boothaus.Services.Persistence.SerializableDTOs;

public class RootDto
{
    public LagerDto Lager { get; set; }
    public List<BootDto> Boote { get; set; }
    public List<AuftragDto> Aufträge { get; set; }
    public RootDto()
    {
        Boote = new List<BootDto>();
        Aufträge = new List<AuftragDto>();
    }
}

public class LagerDto
{
    public decimal StandardMaxLänge { get; set; }
    public decimal StandardMaxBreite { get; set; }
    public List<LagerreiheDto> Lagerreihen { get; set; }
    public LagerDto() 
    {

        Lagerreihen = new List<LagerreiheDto>();
    }

    public LagerDto(Lager lager)
    {
        StandardMaxLänge = lager.MaxBootLänge;
        StandardMaxBreite = lager.MaxBootBreite;
        Lagerreihen = new List<LagerreiheDto>();
        foreach(var reihe in lager.Reihen)
        {
            Lagerreihen.Add(new LagerreiheDto(reihe));
        }
    }
}

public class LagerreiheDto
{
    public int Nummer { get; set; }
    public List<LagerplatzDto> Plätze { get; set; }
    public LagerreiheDto()
    {
        Plätze = new List<LagerplatzDto>();
    }
    public LagerreiheDto(Lagerreihe reihe)
    {
        Nummer = reihe.Nummer;
        Plätze = new List<LagerplatzDto>();
        foreach(var platz in reihe.Plätze)
        {
            Plätze.Add(new LagerplatzDto(platz));
        }
    }

    
}

public class LagerplatzDto
{
    public Guid Id { get; set; }
    public List<Guid> Zuweisungen { get; set; }

    public LagerplatzDto() 
    { 
        Zuweisungen = new List<Guid>();
    }
    public LagerplatzDto(Lagerplatz platz)
    {
        Id = platz.Id;
        Zuweisungen = new List<Guid>();
        foreach(var auftrag in platz.Zuweisungen)
        {
            Zuweisungen.Add(auftrag.Id);
        }
    }
}

public class AuftragDto
{
    public Guid Id { get; set; }
    public Guid Boot { get; set; } 
    public DateOnly Von { get; set; }
    public DateOnly Bis { get; set; }

    public AuftragDto() { }
    public AuftragDto(Auftrag auftrag)
    {
        Id = auftrag.Id;
        Boot = auftrag.Boot.Id; 
        Von = auftrag.Von;
        Bis = auftrag.Bis;

    }
}

public class BootDto
{
    public Guid Id { get; set; }
    public string Kontakt { get; set; }
    public string Name { get; set; }
    public decimal Rumpflänge { get; set; }
    public decimal Breite { get; set; }

    public BootDto() { }
    public BootDto(Boot boot)
    {
        Id = boot.Id;
        Kontakt = boot.Kontakt;
        Name = boot.Name;
        Rumpflänge = boot.Rumpflänge;
        Breite = boot.Breite;
    }
}
