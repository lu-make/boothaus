using Boothaus.Domain;
using Boothaus.Services.Contracts;
using Boothaus.Services.Persistence.SerializableDTOs;
using DeepEqual.Syntax;
using LiteDB; 

namespace Boothaus.Services.Persistence;

public class PersistenceService : IDisposable
{
    private SerDes serdes; 
    private readonly IAuftragRepository auftragRepo;
    private readonly ILagerRepository lagerRepo;
    private readonly IBootRepository bootRepo;
    private readonly LiteDatabase litedb;

    public PersistenceService(
        SerDes serdes,
        string dbpfad,
        IAuftragRepository auftragRepo,
        ILagerRepository lagerRepo,
        IBootRepository bootRepo)
    {
        this.serdes = serdes; 
        this.auftragRepo = auftragRepo;
        this.lagerRepo = lagerRepo;
        this.bootRepo = bootRepo;
        litedb = new LiteDatabase(dbpfad);
    }

    public void ZustandLaden()
    { 
        var col = litedb.GetCollection<RootDto>("Zustand");
        var zustand = col.FindOne(Query.All());
        if (zustand is null) 
        {
            bootRepo.InitialisiereMitDefaults(DefaultData.Boote());
            lagerRepo.InitialisiereMitDefaults(DefaultData.Lager());
            auftragRepo.InitialisiereMitDefaults(DefaultData.Aufträge(lagerRepo.GetLager(), bootRepo.GetAll()));
            ZustandSpeichern();
            return;
        }

        lagerRepo.Clear();
        auftragRepo.Clear();
        bootRepo.Clear();

        SpeichernAusDto(zustand); 
    }

    public void ZustandSpeichern()
    {
        var zustand = serdes.SerializeToDtos(
            lagerRepo.GetLager(),
            auftragRepo.GetAll(),
            bootRepo.GetAll());
         
        litedb.BeginTrans();
        var col = litedb.GetCollection<RootDto>("Zustand");
        col.DeleteAll();
        col.Insert(zustand);
        litedb.Commit();
    }


    public void DatenImportieren(string quellpfad)
    {
        var text = File.ReadAllText(quellpfad); 
        var importData = serdes.Deserialize(text); 
        SpeichernAusDto(importData);
    }

    private void SpeichernAusDto(RootDto root)
    {
        var neuesLager = new Lager(root.Lager.StandardMaxBreite, root.Lager.StandardMaxLänge);
        var neueBoote = new List<Boot>();
        var neueAufträge = new List<Auftrag>();

        foreach (var bootDto in root.Boote.ToList())
        {
            var boot = new Boot(bootDto.Id, bootDto.Name, bootDto.Kontakt, bootDto.Rumpflänge, bootDto.Breite);
            neueBoote.Add(boot);
        }

        foreach (var auftragDto in root.Aufträge.ToList())
        {
            var boot = neueBoote.First(b => b.Id == auftragDto.Boot);
            if (boot is null) throw new InvalidOperationException($"Boot mit ID {auftragDto.Boot} nicht gefunden.");
            var auftrag = new Auftrag(auftragDto.Id, neuesLager, boot, auftragDto.Von, auftragDto.Bis);
            neueAufträge.Add(auftrag);
        } 

        foreach (var reiheDto in root.Lager.Lagerreihen)
        {
            var reihe = new Lagerreihe(reiheDto.Nummer);
            foreach (var platzDto in reiheDto.Plätze)
            {
                var platz = new Lagerplatz(platzDto.Id, reihe);
                foreach (var zuweisung in platzDto.Zuweisungen)
                {
                    var auftrag = neueAufträge.FirstOrDefault(a => a.Id == zuweisung);
                    if (auftrag is null) throw new InvalidOperationException($"Auftrag mit ID {zuweisung} nicht gefunden.");
                    auftrag.Platz = platz;
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
        ZustandSpeichern();
    }

    public void DatenExportieren(string zielpfad)
    {
        var data = serdes.SerializeToJson(
            lagerRepo.GetLager(),
            auftragRepo.GetAll(),
            bootRepo.GetAll()
            );

        File.WriteAllText(zielpfad, data);
    }

    public bool HatUngespeicherteÄnderungen()
    {
        var col = litedb.GetCollection<RootDto>("Zustand");
        var alterZustand = col.FindOne(Query.All());
        var neuerZustand = serdes.SerializeToDtos(
            lagerRepo.GetLager(),
            auftragRepo.GetAll(),
            bootRepo.GetAll()
            );

        return !neuerZustand.IsDeepEqual(alterZustand);
    }

    public void Dispose()
    {
        litedb.Dispose();
    }
}