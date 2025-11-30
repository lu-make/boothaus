using Boothaus.Services.Persistence.SerializableDTOs;
using LiteDB; 

namespace Boothaus.Services.Persistence;

public class PersistenceService
{
    private LagerRepository lagerRepository;
    private AuftragRepository auftragRepository;
    private BootRepository bootRepository;
    private string dbpfad;

    public PersistenceService(
        LagerRepository lagerRepository, 
        AuftragRepository auftragRepository, 
        BootRepository bootRepository, 
        string dbpfad)
    {
        this.lagerRepository = lagerRepository;
        this.auftragRepository = auftragRepository;
        this.bootRepository = bootRepository;
        this.dbpfad = dbpfad;
    }

    public void InitialisiereMitDefaults()
    {

    }

    public void ZustandLaden()
    {


    }

    public void ZustandSpeichern()
    {
        var zustand = new ImportExportDto
        {
            Lager = new LagerDto(lagerRepository.GetLager()),
            Boote = bootRepository.GetAll().Select(b => new BootDto(b)).ToList(),
            Aufträge = auftragRepository.GetAll().Select(a => new AuftragDto(a)).ToList()
        };

        using var db = new LiteDatabase(dbpfad);
        var col = db.GetCollection<ImportExportDto>("Zustand");
        col.Upsert(zustand);
    }
}