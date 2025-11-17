namespace Domain;

public class LagerService
{
    public Lagerkalender ErstelleLagerkalender()
    {
        // TODO
        return new Lagerkalender();
    }
}

public class Lagergraph
{
    public List<Lagerplatz> Lagerplätze { get; private set; } = new List<Lagerplatz>();
     

    public Lagergraph()
    {
        
    }

    public void LagerplatzHinzufügen(Lagerplatz lagerplatz)
    {
        Lagerplätze.Append(lagerplatz); 
        InitialisiereGraph();
    }

    public void LagerplatzEntfernen(Lagerplatz lagerplatz)
    {
        Lagerplätze.Remove(lagerplatz); 
        InitialisiereGraph();
    }

    /// <summary>
    /// Initialisiert den gerichteten azyklischen Graphen (DAG) der Lagerplätze basierend auf deren Blockierbeziehungen.
    /// </summary>
    /// <exception cref="InvalidOperationException">Wenn ein Zyklus existiert.</exception>
    private void InitialisiereGraph()
    { 
        List<Lagerplatz> plätze = Lagerplätze.ToList();
        Dictionary<Lagerplatz, int> eingehendeKanten = BerechneAnzahlEingehenderKanten(plätze);
        List<Lagerplatz> sortiertePlätze = BestimmeTopologischeSortierung(plätze, eingehendeKanten);
        BerechneDistanzZumHallentor(sortiertePlätze);
        BildeInverseBeziehungen(plätze);
    }

    /**
     * Prüft, ob die Blockierbeziehungen einen gerichteten azyklischen Graphen bilden.
     * Er darf weder Zyklen enthalten.
     * Ein Zyklus würde bedeuten, dass eine Bewegung unmöglich ist.
     * Beispiel: A blockiert B, B blockiert C, C blockiert A.
     * Oder: A blockiert B, B blockiert A.
     */
    private bool IstAzyklischerGraph(List<Lagerplatz> plätze)
    {
        return true;
    }

    /**
     * Berechnet für jeden Platz, wie viele Plätze ihn blockieren,
     * d.h. wie viele eingehende Kanten er hat.
     */
    private Dictionary<Lagerplatz, int> BerechneAnzahlEingehenderKanten(List<Lagerplatz> plätze)
    {
        var eingehendeKanten = new Dictionary<Lagerplatz, int>();

        /**
         * warum das funktioniert:
         * 1. für jeden Platz initialisieren wir die Anzahl der eingehenden Kanten auf 0
         * 2. dann gehen wir alle Plätze durch und für jeden blockierten Platz 
         *   erhöhen wir die Anzahl der eingehenden Kanten um 1
         *   (ein blockierter Platz hat eine eingehende Kante von dem Platz, der ihn blockiert)
         */

        foreach (Lagerplatz lagerplatz in plätze)
        {
            if (!eingehendeKanten.ContainsKey(lagerplatz))
            {
                eingehendeKanten.Add(lagerplatz, 0);
            }

            foreach (Lagerplatz blockierterPlatz in lagerplatz.BlockiertePlätze)
            {
                if (!eingehendeKanten.ContainsKey(blockierterPlatz))
                {
                    eingehendeKanten.Add(blockierterPlatz, 0);
                }

                eingehendeKanten[blockierterPlatz]++;
            }
        }

        return eingehendeKanten;
    }

    /**
     * Führt Kahn's Algorithmus aus, um eine topologische Sortierung 
     * der Lagerplätze aufgrund ihrer Blockierbeziehungen zu bestimmen.
     * 
     * Die resultierende Reihenfolge beginnt mit den vorderen Plätzen (nahe dem Hallentor)
     */
    private List<Lagerplatz> BestimmeTopologischeSortierung(List<Lagerplatz> plätze, Dictionary<Lagerplatz, int> eingehendeKanten)
    {
        var warteschlange = new Queue<Lagerplatz>();
        var sortiert = new List<Lagerplatz>();
        var startknoten = eingehendeKanten.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key);

        foreach (var knoten in startknoten)
        {
            warteschlange.Enqueue(knoten);
        }

        while (warteschlange.Count > 0)
        {
            Lagerplatz aktuellerPlatz = warteschlange.Dequeue();
            sortiert.Add(aktuellerPlatz);

            foreach (Lagerplatz blockierterPlatz in aktuellerPlatz.BlockiertePlätze)
            {
                eingehendeKanten[blockierterPlatz]--;

                if (eingehendeKanten[blockierterPlatz] == 0)
                {
                    warteschlange.Enqueue(blockierterPlatz);
                }
            }
        }

        if (sortiert.Count != plätze.Count)
        {
            throw new InvalidOperationException(
                "Die Blockierbeziehungen enthalten mindestens einen Zyklus. " +
                "Ein solcher Graph erlaubt kein korrektes Ein- und Auslagern."
            );
        }

        return sortiert;
    }

    /**
     * Berechnet für jeden Platz die Distanz zum Hallentor bzw. die Tiefe
     * ( Entfernung vom Hallentor entlang des Graphen ) 
     * Vordere Plätze: Tiefe 0
     * Ein Platz hat immer Tiefe = Maximale Tiefe seiner Blockierer + 1
     */
    private void BerechneDistanzZumHallentor(List<Lagerplatz> topologischSortiert)
    {
        foreach (Lagerplatz platz in topologischSortiert)
        {
            platz.DistanzZumHallentor = 0;
        }

        foreach (Lagerplatz vordererPlatz in topologischSortiert)
        {
            foreach (Lagerplatz hintererPlatz in vordererPlatz.BlockiertePlätze)
            {
                hintererPlatz.DistanzZumHallentor
                    = Math.Max(hintererPlatz.DistanzZumHallentor, vordererPlatz.DistanzZumHallentor + 1);
            }
        }
    }

    /*
     * Baut die inverse Relation "BlockierendePlätze" auf.
     *
     * Wenn A -> B (A blockiert B),
     * dann wird B.BlockierendePlätze um A erweitert.
     *
     * Diese Relation wird für spätere Prüfungen benötigt:
     * z. B. "Welche Plätze müssen frei sein, um B zu erreichen?" 
     */

    private void BildeInverseBeziehungen(List<Lagerplatz> plätze)
    {
        foreach (Lagerplatz platz in plätze)
        {
            platz.BlockierendePlätze = new List<Lagerplatz>();

            foreach (Lagerplatz blockierterPlatz in platz.BlockiertePlätze)
            {
                blockierterPlatz.BlockierendePlätze.Add(platz);
            }
        }

    }
}

public class Lagerraster
{
    public int Breite { get; set; }
    public int Länge { get; set; }

    public Lagerplatz?[,] Felder { get; set; }

    public Lagerraster(int breite, int länge)
    {
        Breite = breite;
        Länge = länge;
        Felder = new Lagerplatz?[breite, länge];
    }

    
}

public class Lager
{
    public Lagergraph Graph { get; set; }
    public Lagerraster Raster { get; set; }
}

public class Hallentor
{
    public int PositionX { get; set; }
    public int PositionY { get; set; }
}

public class Lagerplatz
{
    public Guid Id { get; set; }
    public double MaxBreite { get; set; }
    public double MaxLänge { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public List<Lagerplatz> BlockiertePlätze { get; set; } = new List<Lagerplatz>();
    public List<Lagerplatz> BlockierendePlätze { get; set; } = new List<Lagerplatz>(); 
    public int DistanzZumHallentor { get; internal set; }
     
    public bool Passt(Boot boot)
    {
        return boot.Breite <= MaxBreite && boot.Rumpflänge <= MaxLänge;
    } 
}

public class Boot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public double Rumpflänge { get; set; }
    public double Breite { get; set; }
    public double Gewicht { get; set; } 
}

public class Lagerauftrag
{
    public Boot Boot { get; set; }
    public DateOnly Von { get; set; }
    public DateOnly Bis { get; set; }

    public Lagerauftrag(Boot boot, DateOnly von, DateOnly bis)
    {
        if (von > bis)
        {
            throw new ArgumentException("Das Datum 'Von' darf nicht nach dem Datum 'Bis' liegen.");
        }
         
        Boot = boot;
        Von = von;
        Bis = bis;
    }
}

public class LagerplatzZuweisung
{
    public Lagerauftrag Auftrag { get; set; }
    public Lagerplatz Platz { get; set; }

    public LagerplatzZuweisung(Lagerauftrag auftrag, Lagerplatz platz)
    { 
        if (!platz.Passt(auftrag.Boot))
        {
            throw new ArgumentException("Das Boot passt nicht auf den Lagerplatz.");
        }

        Auftrag = auftrag;
        Platz = platz;
    }
}

/// <summary>
/// Das Ergebnis des ganzen Vorgangs.
/// </summary>
public class Lagerkalender
{
    public List<LagerplatzZuweisung> Zuweisungen { get; set; } = new();

}