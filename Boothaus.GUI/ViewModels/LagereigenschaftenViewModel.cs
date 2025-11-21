using Boothaus.Domain;

namespace Boothaus.GUI.ViewModels;

public class LagereigenschaftenViewModel
{ 
    public decimal MaxBreite { get; set; }
    public decimal MaxLänge { get; set; }

    public LagereigenschaftenViewModel(Lager lager)
    { 
        MaxBreite = lager.StandardMaxBreite;
        MaxLänge = lager.StandardMaxLänge;
    }
}