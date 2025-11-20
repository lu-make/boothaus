using Microsoft;

namespace Boothaus.Domain;

public class Lagerauftrag
{
    public Guid Id { get; init; }
    public Lager Lager { get; init; } 
    public Saison Saison { get; init; }

    public Lagerplatz? Platz { get; set; }
    public Boot Boot 
    { 
        get; 
        set 
        {
            Assumes.True(IstGültigesBoot(value), "Das Boot passt nicht in das Lager.");
            field = value;
        } 
    }

    public DateOnly Von 
    { 
        get; 
        set
        {
            Assumes.True(IstGültigesStartdatum(value), "'Von'-Datum muss vor dem 'Bis'-Datum liegen.");
            field = value;
        }
    }

    public DateOnly Bis 
    { 
        get; 
        set
        { 
            Assumes.True(IstGültigesEnddatum(value), "'Bis'-Datum muss nach dem 'Von'-Datum liegen.");
            field = value;
        }

    }
     
    public Lagerauftrag(Guid id, Lager lager, Boot boot, DateOnly von, DateOnly bis)
    {
        Id = id;
        Lager = lager;
        Boot = boot;
        Von = von;
        Bis = bis;
        Saison = new Saison { Anfangsjahr = von.Year };
    }

    /// <summary>
    /// Matrjoschka-Reihung der Lageraufträge.
    /// Jeder Auftrag hat ein Zeitintervall (von Datum bis Datum)
    /// Ein Auftrag a0 kann in die Reihung vor einen Auftrag a1, wenn: 
    /// a0.von >= a1.von UND a0.bis <= a1.bis
    /// </summary>
    /// <param name="anderer">Der andere Lagerauftrag</param>
    /// <returns>
    /// -1: wenn dieser Auftrag echt nach dem anderen geordnet ist (d.h. dieser Auftrag umschließt den anderen)
    /// 0: wenn es keine gültige Reihung gibt (die beiden Aufträge können nicht derselben Reihe zugewiesen werden, 
    /// ohne dass die Termine kollidieren)
    /// 1: wenn dieser Auftrag echt vor dem anderen geordnet ist (d.h. der andere Auftrag umschließt diesen)
    /// </returns>  
    public int VergleicheReihenordnung(Lagerauftrag anderer)
    {
        // dieser auftrag umschließt den anderen auftrag
        if (anderer.Von >= Von && anderer.Bis <= Bis)
        {
            return -1;
        }

        // anderer auftrag umschließt diesen auftrag
        if (Von >= anderer.Von && Bis <= anderer.Bis)
        {
            return 1;
        }

        // diese aufträge können nicht in derselben reihe gelagert werden
        return 0;
    }

    public override string ToString() => $"{Boot.Name} ({Von} - {Bis})";

    public bool IstGültigesStartdatum(DateOnly? datum)
    {
        return (Bis == default || datum < Bis);
    } 

    public bool IstGültigesEnddatum(DateOnly? datum)
    {
        return (Von == default || datum > Von);
    }

    public static bool IstGültigesDatumspaar(DateOnly von, DateOnly bis)
    {
        return von < bis;
    }

    public bool IstGültigesBoot(Boot boot)
    {
        return Lager.Passt(boot);
    }

  

}
