using Boothaus.Domain;
using System.Collections.ObjectModel;

namespace Boothaus.GUI.ViewModels;

public class LagerViewModel
{
    public Lager Modell { get; }
    public ObservableCollection<LagerreihenViewModel> Reihen { get; } = new();

    public LagerViewModel(Lager lager)
    {
        Modell = lager;

        foreach (var reihe in lager.Reihen)
        {
            var reihenViewModel = new LagerreihenViewModel(reihe);

            foreach (var p in reihenViewModel.PlätzeVms)
            {
                p.Aktualisieren();
            }

            Reihen.Add(reihenViewModel);
        }
    }
}
