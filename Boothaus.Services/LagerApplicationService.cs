using Boothaus.Services.Contracts;
using Boothaus.Domain; 

namespace Domain.Services;

/// <summary>
/// Diese Klasse ist die Hauptschnittstelle für die Lagerverwaltung.
/// In ihr werden die Planungsvorgänge orchestriert.
/// Das bedeutet: Verwaltung von Lageraufträgen und Booten sowie
/// Zuweisung von Lagerplätzen zu den Aufträgen.
/// </summary>
public class LagerApplicationService
{
    private IBootRepository bootRepository;
    private IAuftragRepository auftragRepository;
    private ILagerRepository lagerRepository;

    public LagerApplicationService(
        IBootRepository bootRepository, 
        IAuftragRepository auftragRepository, 
        ILagerRepository lagerRepository)
    {
        this.bootRepository = bootRepository;
        this.auftragRepository = auftragRepository;
        this.lagerRepository = lagerRepository;
    }

    public Lagerkalender ErstelleLagerkalender()
    {
        var lager = GetLager();
        var aufträge = auftragRepository.GetAll().ToList();
        var kalender = new Lagerkalender(); 
        var zuweisungen = PlätzeZuweisen(lager, aufträge); 
        kalender.Zuweisungen.AddRange(zuweisungen);
        return kalender;
    }

    private List<LagerplatzZuweisung> PlätzeZuweisen(Lager lager, List<Lagerauftrag> aufträge)
    {
        var zuweisungen = new List<LagerplatzZuweisung>();

        /* 
         * aufsteigend nach "matroschka"-Sortierung 
         * zuerst die Aufträge, die andere Aufträge zeitlich komplett einschließen
         * diese sollen möglichst weit hinten in einer Lagerreihe gelagert werden
         * längeres intervall zuerst
        */
        var sortierteAufträge = aufträge
            .OrderBy(a => a.Von)
            .ThenByDescending(a => a.Bis)
            .ToList();

        var reihen = lager.Reihen; 

        foreach (var auftrag in sortierteAufträge)
        {
            var zuweisung = new LagerplatzZuweisung(auftrag, null!);

            foreach (var reihe in reihen)
            {
 
                /*
                 * wenn die reihe noch keine zuweisungen hat, weise den auftrag dem hintersten platz zu
                 */
                if (!zuweisungen.Any(z => z.Platz.Reihe == reihe))
                {
                    var hintersterPlatz = reihe.Plätze.Last(); 
                    zuweisung.Platz = hintersterPlatz;
                    break;
                } 

                /*
                 * ist die reihe schon voll?
                 */
                if (zuweisungen.Count(z => z.Platz.Reihe == reihe) == reihe.Plätze.Count)
                {
                    // ja, versuche die nächste reihe
                    continue;
                }

                /*
                 * nimm den letzten auftrag, der dieser reihe zugewiesen wurde
                 */
                var letzteZuweisungAusReihe = zuweisungen
                    .Where(z => z.Platz.Reihe == reihe)
                    .Last();

                var ordnung = auftrag.VergleicheReihenordnung(letzteZuweisungAusReihe.Auftrag);

                if (ordnung == 1)
                {
                    // dieser auftrag kann vor den letzten auftrag in der reihe platziert werden
                    // (der kleinere index ist weiter vorne) 
                    var letzerIndex = reihe.Index(letzteZuweisungAusReihe.Platz);
                    var nächsterFreierPlatz = reihe[letzerIndex - 1];
                    zuweisung.Platz = nächsterFreierPlatz;
                    break;
                }

                // keine zuweisung. versuche die nächste reihe
                continue;
            }

            if (zuweisung.Platz is null)
            { 
                 throw new InvalidOperationException("Es konnte keine konsistente Lagerplatzzuweisung vorgenommen werden.");
            }

            zuweisungen.Add(zuweisung);
        }

        return zuweisungen;
    } 

    public IEnumerable<Boot> AlleBoote()
    {
        return bootRepository.GetAll();
    }

    public Boot ErzeugeBoot(string name, double länge, double breite, string kontakt)
    {
        var id = Guid.NewGuid();
        var boot = new Boot(
            id: id,
            name: name,
            rumpflänge: länge,
            breite: breite,
            kontakt: kontakt
        );

        bootRepository.Add(boot);
        return boot;
    }

    public void UpdateBoot(Boot boot)
    {
        bootRepository.Update(boot);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auftrag"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ErfasseAuftrag(Lagerauftrag auftrag)
    { 
        if (BootAuftragExistiertBereits(auftrag.Boot, auftrag.Von, auftrag.Bis))
        {
            throw new InvalidOperationException("Das Boot hat bereits einen Lagerauftrag in dem angegebenen Zeitraum.");
        }

        auftragRepository.Add(auftrag);
    }

    /// <summary>
    /// Regel: Wenn ein Boot schon einen Lagerauftrag in dem Zeitraum hat, darf kein neuer angelegt werden.
    /// </summary> 
    /// <param name="boot">Das Boot des neuen Auftrags</param>
    /// <param name="von">Das Startdatum des neuen Auftrags</param>
    /// <param name="bis">Das Enddatum des neuen Auftrags </param>
    /// <returns>Wahr, wenn ein Konflikt besteht (der Auftrag kann nicht angelegt werden), sonst Falsch.</returns>
    public bool BootAuftragExistiertBereits(Boot boot, DateOnly von, DateOnly bis)
    {
        bool hatKonflikt = auftragRepository.GetAll()
            .Any(a => a.Boot.Id == boot.Id &&
                      von <= a.Bis &&
                      bis >= a.Von);

        return hatKonflikt;
    }

    public void AktualisiereAuftrag(Lagerauftrag auftrag)
    {
        auftragRepository.Update(auftrag);
    }

    public IEnumerable<Lagerauftrag> AlleAufträge()
    {
        return auftragRepository.GetAll();
    }
     
    public Lager GetLager()
    {
        var lager = lagerRepository.GetLager(); 
        return lager;
    }

    public void InitLager(double standardMaxBreite, double standardMaxLänge)
    {
        var lager = new Lager(standardMaxBreite, standardMaxLänge);
        lagerRepository.Save(lager);
    }

    public void LöscheAuftrag(Lagerauftrag? auftrag)
    {
        if (auftrag is null) return;
        auftragRepository.Remove(auftrag);
    }
}
