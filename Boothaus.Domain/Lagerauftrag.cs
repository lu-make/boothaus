namespace Boothaus.Domain;

public class Lagerauftrag
{
    public Lager Lager { get; set; }
    public Boot Boot { get; set; }
    public DateOnly Von { get; set; }
    public DateOnly Bis { get; set; }

    public Lagerauftrag(Lager lager, Boot boot, DateOnly von, DateOnly bis)
    {
        if (von > bis)
        {
            throw new ArgumentException("Das Datum 'Von' darf nicht nach dem Datum 'Bis' liegen.");
        }

        if (boot.Rumpflänge > lager.StandardMaxLänge || boot.Breite > lager.StandardMaxBreite)
        {
            throw new ArgumentException("Das Boot passt nicht in das Lager.");
        }

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
}
