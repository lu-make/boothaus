using Boothaus.Services.Contracts;
using Boothaus.Domain;
using System.Runtime.CompilerServices;
using System.ComponentModel.Design;
using Boothaus.Services.Persistence;

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
    private HashSet<Saison> saisons;
    private ImportExportService importExport;

    public LagerApplicationService(
        IBootRepository bootRepository,
        IAuftragRepository auftragRepository,
        ILagerRepository lagerRepository,
        ImportExportService importExport)
    {
        this.bootRepository = bootRepository;
        this.auftragRepository = auftragRepository;
        this.lagerRepository = lagerRepository;
        saisons = [new Saison(2025)];
        this.importExport = importExport;
    }

    /// <summary>
    /// Verteilt automatisch Lagerplätze an alle Lageraufträge in der Saison
    /// </summary>
    /// <param name="saison">Die Saison die berücksichtigt werden soll</param>
    /// <returns>Wahr, wenn alle Aufträge konsistent vergeben werden konnten, sonst Falsch</returns>
    public bool ErstelleLagerkalender(Saison saison)
    {
        var lager = GetLager();
        ResetInSaison(saison);
        var aufträge = auftragRepository.GetBySaison(saison).ToList(); 
        var ergebnis = PlätzeZuweisen(lager, aufträge);
        lagerRepository.Save(lager);
        return ergebnis;
    }

    private bool PlätzeZuweisen(Lager lager, List<Auftrag> aufträge)
    {
        /* 
         * aufsteigend nach "matroschka"-Sortierung 
         * zuerst die Aufträge, die andere Aufträge zeitlich komplett einschließen
         * diese sollen möglichst weit hinten in einer Lagerreihe gelagert werden
         * längeres intervall zuerst
        */
        var sortierteAufträge = aufträge
            .Where(a => lager.Passt(a.Boot))
            .OrderBy(a => a.Von)
            .ThenByDescending(a => a.Bis)
            .ToList();

        var reihen = lager.Reihen;

        var konsistent = true;

        foreach (var auftrag in sortierteAufträge)
        {
            if (auftrag.Platz is not null)
            {
                // dieser auftrag hat schon eine zuweisung
                continue;
            }

            foreach (var reihe in reihen)
            { 
                /*
                 * wenn die reihe noch keine zuweisungen hat, weise den auftrag dem hintersten platz zu
                 */ 
                if (reihe.IstFreiImZeitraum(auftrag.Von, auftrag.Bis))
                {
                    var hintersterPlatz = reihe.Plätze.First();
                    auftrag.Platz = hintersterPlatz;
                    hintersterPlatz.ZuweisungHinzufügen(auftrag);
                    break;
                }

                /*
                 * ist die reihe schon voll?
                 */
                if (reihe.IstVoll(auftrag.Von, auftrag.Bis))
                {
                    // ja, versuche die nächste reihe
                    continue;
                }

                /*
                 * nimm den letzten auftrag, der dieser reihe zugewiesen wurde
                 * ( an dieser stelle garantiert ) 
                 */
                var letzteZuweisungAusReihe = reihe
                    .VordersterBelegterPlatz(auftrag.Von, auftrag.Bis)!
                    .GetNächsteZuweisung(auftrag.Saison)!;
                    
                var ordnung = auftrag.VergleicheReihenordnung(letzteZuweisungAusReihe);

                if (ordnung == 1)
                {
                    // dieser auftrag kann vor den letzten auftrag in der reihe platziert werden
                    // (der größere index ist weiter vorne) 
                    var letzerIndex = reihe.Index(letzteZuweisungAusReihe.Platz!);
                    var nächsterFreierPlatz = reihe[letzerIndex + 1];
                    nächsterFreierPlatz.ZuweisungHinzufügen(auftrag);
                    auftrag.Platz = nächsterFreierPlatz;
                    break;
                }

                // keine zuweisung. versuche die nächste reihe
                continue;
            }

            if (auftrag.Platz is null)
            {
                konsistent = false;
            }
             
        }

        return konsistent;
    } 

    /// <summary>
    /// Holt alle bestehenden Boote
    /// </summary>
    /// <returns>Eine Liste von Booten</returns>
    public IEnumerable<Boot> AlleBoote()
    {
        return bootRepository.GetAll();
    }

    /// <summary>
    /// Erfasst ein neues Boot
    /// </summary>
    /// <param name="boot">Das neue Boot</param>
    public void ErfasseBoot(Boot boot)
    {
        bootRepository.Add(boot);
    }

    public void UpdateBoot(Boot boot)
    {
        bootRepository.Update(boot);
    }

    /// <summary>
    /// Erfasst einen neuen Lagerauftrag.
    /// </summary>
    /// <param name="auftrag">Der neue Auftrag</param>
    /// <exception cref="InvalidOperationException">Das Boot darf in dem Zeitraum keinen bestehenden Auftrag haben.</exception>
    public void ErfasseAuftrag(Auftrag auftrag)
    { 
        if (BootAuftragExistiertBereits(auftrag.Boot, auftrag.Von, auftrag.Bis))
        {
            throw new InvalidOperationException("Das Boot hat bereits einen Lagerauftrag in dem angegebenen Zeitraum.");
        }
        saisons.Add(auftrag.Saison);
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

    /// <summary>
    /// Überschreibt einen Auftrag.
    /// </summary>
    /// <param name="auftrag">Der existierende Auftrag</param>
    public void AktualisiereAuftrag(Auftrag auftrag)
    {
        auftragRepository.Update(auftrag);
    }

    /// <summary>
    /// Gibt alle Lageraufträge als Liste zurück.
    /// </summary>
    /// <returns>Eine Liste aller Lageraufträge.</returns>
    public IEnumerable<Auftrag> AlleAufträge()
    {
        return auftragRepository.GetAll();
    }

    /// <summary>
    /// Gibt eine Liste aller Lageraufträge zurück, die sich in der gegebenen Saison befinden.
    /// </summary>
    /// <param name="saison">Die Saison, nach welcher gefiltert werden soll (z.B. 25/26).</param>
    /// <returns>Eine Liste von Lageraufträgen in der Saison.</returns>
    public IEnumerable<Auftrag> AlleAufträgeInSaison(Saison saison)
    {
        return auftragRepository.GetBySaison(saison);
    }

    /// <summary>
    /// Gibt eine Liste aller definierten Saisons zurück.
    /// 
    /// Beispiel: Existieren Aufträge mit Saison 25/26 und 26/27, wird eine Liste mit den beiden Saisons zurückgegeben.
    /// </summary>
    /// <returns>Eine Liste von Saisons.</returns>
    public IEnumerable<Saison> AlleSaisons()
    {
        return saisons;
    }

    /// <summary>
    /// Gibt das Lager zurück.
    /// </summary>
    /// <returns>Das Lager.</returns>
    public Lager GetLager()
    {
        var lager = lagerRepository.GetLager(); 
        return lager;
    }

    /// <summary>
    /// Initialisiert das Lager mit einer maximalen Länge und Breite pro Boot.
    /// </summary>
    /// <param name="standardMaxBreite">Die maximale Breite eines Bootes in diesem Lager</param>
    /// <param name="standardMaxLänge">Die maximale Länge eines Bootes in diesem Lager</param>
    public void InitLager(decimal standardMaxBreite, decimal standardMaxLänge)
    {
        var lager = new Lager(standardMaxBreite, standardMaxLänge);
        lagerRepository.Save(lager);
    }

    /// <summary>
    /// Löscht einen bestehenden Auftrag.
    /// </summary>
    /// <param name="auftrag">Der zu löschende Auftrag</param>
    public void LöscheAuftrag(Auftrag? auftrag)
    {
        if (auftrag is null) return;
        auftragRepository.Remove(auftrag);
    }

    /// <summary>
    /// Prüft, ob ein gegebener Auftrag einem gegebenen Platz zugewiesen werden darf.
    /// </summary>
    /// <param name="auftrag">Der Lagerauftrag, der zugewiesen werden soll.</param>
    /// <param name="platz">Der Lagerplatz, auf den zugewiesen werden soll.</param>
    /// <returns>Wahr, wenn der Auftrag dem Platz zugewiesen werden darf, sonst falsch.</returns>
    public bool KannZuweisen(Auftrag? auftrag, Lagerplatz? platz)
    {
        if (auftrag is null || platz is null) return false;
        if (!auftrag.IstGültigesBoot(auftrag.Boot)) return false;
        if (!Auftrag.IstGültigesDatumspaar(auftrag.Von, auftrag.Bis)) return false;
        if (!platz.IstFreiImZeitraum(auftrag.Von, auftrag.Bis)) return false;
        if (platz.Reihe is null) return false;

        // ist diese reihe komplett frei? dann darf der hinterste platz belegt werden
        if (platz.Reihe.IstFreiImZeitraum(auftrag.Von, auftrag.Bis))
            return platz.Reihe.Index(platz) == 0;

        // Es darf nur auf den hintersten freien platz der reihe zugewiesen werden
        var vordersterBelegterPlatz = platz.Reihe.VordersterBelegterPlatz(auftrag.Von, auftrag.Bis);
        if (vordersterBelegterPlatz is null) return false;
        var zuweisungDavor = vordersterBelegterPlatz?.GetNächsteZuweisung(auftrag.Saison);
        if (zuweisungDavor is null) return false;
        var ordnung = auftrag.VergleicheReihenordnung(zuweisungDavor);
        if (ordnung != 1) return false;
        if (platz.Reihe.Index(platz) != platz.Reihe.Index(vordersterBelegterPlatz) + 1) return false;
        return true;
    }

    /// <summary>
    /// Produziert eine Liste von Lagerplätzen, die für einen gegebenen Auftrag zur Verfügung stehen.
    /// </summary>
    /// <param name="auftrag">Der Lagerauftrag, der zugewiesen werden soll.</param>
    /// <returns>Eine Liste von Lagerplätzen, denen der Auftrag zugewiesen werden darf.</returns>
    public IEnumerable<Lagerplatz> FindeGültigePlätze(Auftrag auftrag)
    {
        var lager = GetLager();
        var gültigePlätze = new List<Lagerplatz>();
        foreach (var reihe in lager.Reihen)
        {
            foreach (var platz in reihe.Plätze)
            {
                if (KannZuweisen(auftrag, platz))
                {
                    gültigePlätze.Add(platz);
                }
            }
        }
        return gültigePlätze;
    }

    /// <summary>
    /// Die Aufträge und Lagerplatzverteilungen dieser Saison werden in die nächste Saison kopiert.
    /// Bestehende Aufträge in der nächsten Saison bleiben unberührt.
    /// </summary>
    /// <param name="ausgewählteSaison">Die aktuelle Saison.</param>
    public void DupliziereSaisonInNächsteSaison(Saison ausgewählteSaison)
    {
        var nächsteSaison = new Saison(ausgewählteSaison.Anfangsjahr + 1);
        ResetInSaison(nächsteSaison);
        var aufträge = auftragRepository.GetBySaison(ausgewählteSaison).ToList();
        foreach (var auftrag in aufträge)
        {
            var platz = auftrag.Platz;

            var bestehenderAuftragInNächsterSaison = auftragRepository.GetBySaison(nächsteSaison)
                .FirstOrDefault(a => a.Boot.Id == auftrag.Boot.Id &&
                          a.Von == auftrag.Von.AddYears(1) &&
                          a.Bis == auftrag.Bis.AddYears(1));

            if (bestehenderAuftragInNächsterSaison is not null) 
            {
                bestehenderAuftragInNächsterSaison.Boot = auftrag.Boot;
                bestehenderAuftragInNächsterSaison.Platz = platz;
                bestehenderAuftragInNächsterSaison.Platz?.ZuweisungHinzufügen(bestehenderAuftragInNächsterSaison);

                auftragRepository.Update(bestehenderAuftragInNächsterSaison);
            } 
            else 
            {
                var neuerAuftrag = new Auftrag(
                    lager: auftrag.Lager,
                    boot: auftrag.Boot,
                    von: auftrag.Von.AddYears(1),
                    bis: auftrag.Bis.AddYears(1)
                );

                neuerAuftrag.Platz = platz;
                platz?.ZuweisungHinzufügen(neuerAuftrag);
                auftragRepository.Add(neuerAuftrag);
            }

        } 
        saisons.Add(nächsteSaison);
    }

    /// <summary>
    /// Setzt den gesamten Lagerkalender zurück (entfernt alle Zuweisungen)
    /// </summary>
    public void Reset()
    {
        foreach (var auftrag in auftragRepository.GetAll())
        {
            auftrag.Platz = null;
        }

        foreach (var reihe in lagerRepository.GetLager().Reihen)
        {
            foreach (var platz in reihe.Plätze)
            {
                platz.ZuweisungenLeeren();
            }
        }
    }

    /// <summary>
    /// Setzt den Lagerkalender für eine Saison zurück (d.h. entfernt alle Zuweisungen)
    /// </summary>
    /// <param name="saison">Die Saison in der die Lagerplätze zurückgesetzt werden sollen</param>
    public void ResetInSaison(Saison saison)
    { 
        foreach (var auftrag in auftragRepository.GetBySaison(saison))
        {
            auftrag.Platz = null;
        }
        foreach (var reihe in lagerRepository.GetLager().Reihen)
        {
            foreach (var platz in reihe.Plätze)
            {
                platz.ZuweisungenLeerenInSaison(saison);
            }
        }

    }

    /// <summary>
    /// Löscht ein Boot und alle damit zusammenhängenden Aufträge.
    /// </summary>
    /// <param name="boot">Das Boot welches gelöscht werden soll</param>
    public void LöscheBoot(Boot boot)
    {
        var aufträge = auftragRepository.GetAll()
            .Where(a => a.Boot.Id == boot.Id)
            .ToList();

        foreach (var auftrag in aufträge)
        {
            auftrag.Platz?.ZuweisungEntfernen(auftrag);
            auftragRepository.Remove(auftrag);
        }

        bootRepository.Remove(boot);
    }

    /// <summary>
    /// Exportiert den gesamten Datensatz in eine JSON-Datei an den angegebenen Zielpfad.
    /// </summary>
    /// <param name="zielpfad">Der Pfad der Datei</param>
    public void DatenExportieren(string zielpfad)
    {
        importExport.DatenExportieren(zielpfad);
    }

    /// <summary>
    /// Importiert einen Datensatz aus einer JSON-Datei vom angegebenen Quellpfad.
    /// </summary>
    /// <param name="quellpfad">Der Pfad der Datei</param>
    public void DatenImportieren(string quellpfad)
    {
        importExport.DatenImportieren(quellpfad);
    }
}