namespace Boothaus.Domain;

public class Lagerreihe
{
    public int Nummer { get; set; }
    public Lager? Lager { get; set; } 

    private List<Lagerplatz> plätze = [];

    public IReadOnlyList<Lagerplatz> Plätze => plätze;
    public Lagerplatz this[int index]
    {
        get => Plätze[index]; 
    }

    public Lagerreihe(int nummer)
    {
        Nummer = nummer; 
    }

    public void PlatzHinzufügen(Lagerplatz platz)
    {
        if (platz.Reihe is not null)
        {
            throw new ArgumentException("Der Lagerplatz gehört bereits zu einer Reihe.");
        }

        platz.Reihe = this;
        plätze.Add(platz);
    }

    public void PlatzEntfernen(Lagerplatz platz)
    {
        bool entfernt = plätze.Remove(platz);
        
        if (!entfernt)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }

        platz.Reihe = null;
    }

    public int Index(Lagerplatz platz)
    {
        var index = plätze.IndexOf(platz);
        if (index == -1)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }
        return index;
    }

    /// <summary>
    /// Alle Lagerplätze, die in der Reihe vor dem angegebenen Lagerplatz kommen.
    /// "Vor" bedeutet weiter weg vom Hallentor.
    /// </summary>
    /// <param name="platz">Der Lagerplatz</param>
    /// <returns>Eine Sammlung von Lagerplätzen</returns>
    /// <exception cref="ArgumentException">Wenn der übergebene Platz nicht Teil der Reihe ist</exception>
    public IEnumerable<Lagerplatz> PlätzeVor(Lagerplatz platz)
    {
        var index = plätze.IndexOf(platz);
        if (index == -1)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }

        return Plätze.Take(index);
    }

    /// <summary>
    /// Alle Lagerplätze, die in der Reihe nach dem angegebenen Lagerplatz kommen.
    /// "Nach" bedeutet näher am Hallentor.
    /// </summary>
    /// <param name="platz">Der Lagerplatz</param>
    /// <returns>Eine Sammlung von Lagerplätzen</returns>
    /// <exception cref="ArgumentException">Wenn der übergebene Platz nicht Teil der Reihe ist</exception>
    public IEnumerable<Lagerplatz> PlätzeHinter(Lagerplatz platz)
    {
        var index = plätze.IndexOf(platz);
        if (index == -1)
        {
            throw new ArgumentException("Der angegebene Lagerplatz gehört nicht zu dieser Reihe.");
        }
        return Plätze.Skip(index + 1);
    }

    public bool IstFreiImZeitraum(DateOnly von, DateOnly bis)
    {
        return plätze.All(p => p.IstFreiImZeitraum(von, bis));
    }

    public bool IstVoll(DateOnly von, DateOnly bis)
    {
        return plätze.All(p => !p.IstFreiImZeitraum(von, bis));
    }

    public Lagerplatz? VordersterBelegterPlatz(DateOnly von, DateOnly bis)
    {
         return Plätze.LastOrDefault(p => !p.IstFreiImZeitraum(von, bis));
    }

    public bool IstZeitraumErlaubtAnIndex(Saison saison, DateOnly von, DateOnly bis, int index)
    {
        var erlaubt = true;

        if (index < 0 || index >= Plätze.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Der Index liegt außerhalb des gültigen Bereichs.");
        }

        // 1. gibt es hinter diesem index eine belegung?
        // -> 1.1 wenn ja, ist reihenordnung mit hinterem auftrag 1?

        var hintererPlatz = Plätze.ElementAtOrDefault(index - 1);
        var hintereBelegung = hintererPlatz?.GetNächsteZuweisung(saison);

        if (hintereBelegung is not null && hintereBelegung.VergleicheReihenordnung(von, bis) != -1)
        {
            erlaubt = false;
            return erlaubt;
        }


        // 2. gibt es vor diesem index eine belegung?
        // -> 2.1 wenn ja, ist reihenordnung mit vorderem auftrag -1?

        var vordererPlatz = Plätze.ElementAtOrDefault(index + 1);
        var vordereBelegung = vordererPlatz?.GetNächsteZuweisung(saison);
        if (vordereBelegung is not null && vordereBelegung.VergleicheReihenordnung(von, bis) != 1)
        {
            erlaubt = false;
            return erlaubt;
        }

        return erlaubt;
    }
     
}
