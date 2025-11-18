using Domain.Model;

namespace Domain.Services;

/// <summary>
/// Diese Klasse ist die Hauptschnittstelle für die Lagerverwaltung.
/// In ihr werden die Planungsvorgänge orchestriert.
/// Das bedeutet: Verwaltung von Lageraufträgen und Booten sowie
/// Zuweisung von Lagerplätzen zu den Aufträgen.
/// </summary>
public class LagerService
{
    public Lagerkalender ErstelleLagerkalender(Lager lager, List<Lagerauftrag> aufträge)
    {
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
                    var letzerIndex = reihe.Plätze.IndexOf(letzteZuweisungAusReihe.Platz);
                    var nächsterFreierPlatz = reihe[letzerIndex - 1];
                    zuweisung.Platz = nächsterFreierPlatz;
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

}
