using Boothaus.Domain;
using Boothaus.Services.Contracts;
using Boothaus.Services.Persistence.SerializableDTOs;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Boothaus.Services.Persistence;

public class ImportExportService
{
    private readonly IAuftragRepository auftragRepo;
    private readonly ILagerRepository lagerRepo;
    private readonly IBootRepository bootRepo;

    private JsonSerializerOptions options;
    public ImportExportService(IAuftragRepository auftragRepo, ILagerRepository lagerRepo, IBootRepository bootRepo)
    {
        this.auftragRepo = auftragRepo;
        this.lagerRepo = lagerRepo;
        this.bootRepo = bootRepo;

        options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement)
        };

    }

    public void DatenImportieren(string quellpfad)
    {
        var text = File.ReadAllText(quellpfad); 

        var importData = JsonSerializer.Deserialize<ImportExportDto>(text, options);

        if (importData is null)
        {
            throw new InvalidOperationException("Importierte Daten sind ungültig.");
        }

        var neuesLager = new Lager(importData.Lager.StandardMaxBreite, importData.Lager.StandardMaxLänge);
        var neueBoote = new List<Boot>();
        var neueAufträge = new List<Auftrag>();

        foreach (var bootDto in importData.Boote.ToList())
        {
            var boot = new Boot(bootDto.Id, bootDto.Name, bootDto.Kontakt, bootDto.Rumpflänge, bootDto.Breite);
            neueBoote.Add(boot);
        }

        foreach (var auftragDto in importData.Aufträge.ToList())
        {
            var boot = neueBoote.First(b => b.Id == auftragDto.Boot);
            if (boot is null) throw new InvalidOperationException($"Boot mit ID {auftragDto.Boot} nicht gefunden.");
            var auftrag = new Auftrag(auftragDto.Id, neuesLager, boot, auftragDto.Von, auftragDto.Bis);
            neueAufträge.Add(auftrag);
        }


        foreach (var reiheDto in importData.Lager.Lagerreihen)
        {
            var reihe = new Lagerreihe(reiheDto.Nummer);
            foreach (var platzDto in reiheDto.Plätze)
            {
                var platz = new Lagerplatz(platzDto.Id, reihe);
                foreach (var zuweisung in platzDto.Zuweisungen)
                {
                    var auftrag = auftragRepo.Get(zuweisung);
                    if (auftrag is null) throw new InvalidOperationException($"Auftrag mit ID {zuweisung} nicht gefunden.");
                    platz.ZuweisungHinzufügen(auftrag); 
                }
                reihe.PlatzHinzufügen(platz);

            }
            neuesLager.ReiheUpsert(reihe);
        }

        foreach (var auftrag in neueAufträge)
        {
            auftragRepo.Upsert(auftrag);
        }
        foreach (var boot in neueBoote)
        {
            bootRepo.Upsert(boot);
        }

        lagerRepo.Save(neuesLager);

    }

    public void DatenExportieren(string zielpfad)
    {  
        var lagerDto = new LagerDto(lagerRepo.GetLager());

        var aufträgeDtos = auftragRepo.GetAll()
            .Select(a => new AuftragDto(a))
            .ToList();

        var bootDtos = bootRepo.GetAll()
            .Select(b => new BootDto(b))
            .ToList();

        var exportJson = JsonSerializer.Serialize(
            new ImportExportDto
            {
                Lager = lagerDto,
                Boote = bootDtos,
                Aufträge = aufträgeDtos
            }, options);

        File.WriteAllText(zielpfad, exportJson);

    }
}
