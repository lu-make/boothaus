using Boothaus.Domain;
using System.Collections.ObjectModel;

namespace Boothaus.GUI.ViewModels;

public class LagerreihenViewModel 
{
    public Lagerreihe Modell { get; }
    public ObservableCollection<LagerplatzViewModel> Plätze { get; } = new();

    public LagerreihenViewModel(Lagerreihe reihe)
    {
        Modell = reihe;
        foreach (var platz in reihe.Plätze)
        {
            Plätze.Add(new LagerplatzViewModel(platz));
        }
    }
}
