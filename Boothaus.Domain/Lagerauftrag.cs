using Microsoft;

namespace Boothaus.Domain;

public class Lagerauftrag
{
    public Guid Id { get; init; }
    public Lager Lager { get; set; }
    public Boot Boot 
    { 
        get; 
        set 
        { 
            Assumes.True(value.Rumpflänge <= Lager.StandardMaxLänge, "Das Boot passt nicht in das Lager (Länge überschritten).");
            Assumes.True(value.Breite <= Lager.StandardMaxBreite, "Das Boot passt nicht in das Lager (Breite überschritten).");
            field = value;
        } 
    }

    public DateOnly Von 
    { 
        get; 
        set
        {
            if (Bis != default) Assumes.True(value < Bis, "'Von'-Datum muss vor dem 'Bis'-Datum liegen.");
            field = value;
        }
    }

    public DateOnly Bis 
    { 
        get; 
        set
        {
            if (Von != default) Assumes.True(value > Von, "'Bis'-Datum muss nach dem 'Von'-Datum liegen.");
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
}
