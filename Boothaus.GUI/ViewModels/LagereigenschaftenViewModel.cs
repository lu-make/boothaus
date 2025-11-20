using Boothaus.Domain;

namespace Boothaus.GUI.ViewModels;

public class LagereigenschaftenViewModel
{
    public int Plätze { get; set; }
    public double MaxBreite { get; set; }
    public double MaxLänge { get; set; }

    public LagereigenschaftenViewModel(Lager lager)
    {
        Plätze = lager.AnzahlPlätze;
        MaxBreite = lager.StandardMaxBreite;
        MaxLänge = lager.StandardMaxLänge;
    }
}