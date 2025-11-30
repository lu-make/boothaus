using Boothaus.Domain;
using Boothaus.Services.Contracts;
using Boothaus.Services.Persistence.SerializableDTOs;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode; 
namespace Boothaus.Services.Persistence;

public class SerDes
{
    private JsonSerializerOptions options;

    public SerDes()
    {

        options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement)
        };
    }

    public RootDto Deserialize(string data)
    {
        var rootDto = JsonSerializer.Deserialize<RootDto>(data, options);
        if (rootDto is null)
        {
            throw new InvalidOperationException("Deserialisierung fehlgeschlagen: Daten sind ungültig.");
        }
        return rootDto;
    }
      
    public RootDto SerializeToDtos(Lager lager, IEnumerable<Auftrag> aufträge, IEnumerable<Boot> boote)
    {
        var lagerDto = new LagerDto(lager);

        var aufträgeDtos = aufträge
            .Select(a => new AuftragDto(a))
            .ToList();

        var bootDtos = boote
            .Select(b => new BootDto(b))
            .ToList();

        return new RootDto
        {
            Lager = lagerDto,
            Boote = bootDtos,
            Aufträge = aufträgeDtos
        };
    }

    public string SerializeToJson(Lager lager, IEnumerable<Auftrag> aufträge, IEnumerable<Boot> boote)
    {
        var exportJson = JsonSerializer.Serialize(SerializeToDtos(lager, aufträge, boote));

        return exportJson; 
    }
}
