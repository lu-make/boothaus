using Boothaus.Domain;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Boothaus.GUI.ViewModels;

public class LagerViewModel : INotifyPropertyChanged
{
    private Saison ausgewählteSaison;

    public Lager Modell 
    { 
        get; 
        set
        {
            field = value;
            OnPropertyChanged(nameof(Modell));
        }
    }

    public IEnumerable<LagerplatzViewModel> AllePlätze => ReihenViewmodels.SelectMany(r => r.PlatzViewmodels);


    public ObservableCollection<LagerreihenViewModel> ReihenViewmodels { get; } = new();

    public int AnzahlLagerreihen => ReihenViewmodels.Count;

    public void Update(Saison ausgewählteSaison)
    { 
        ReihenViewmodels.Clear();
        foreach (var reihe in Modell.Reihen)
        {
            var reihenViewmodel = new LagerreihenViewModel(reihe);

            foreach (var platzVm in reihenViewmodel.PlatzViewmodels)
            {
                platzVm.AusgewählteSaison = ausgewählteSaison; 
            }
            ReihenViewmodels.Add(reihenViewmodel);
        }
        this.ausgewählteSaison = ausgewählteSaison;
        OnPropertyChanged(nameof(AnzahlLagerreihen));
    }

    public LagerViewModel(Lager lager, Saison ausgewählteSaison)
    {
        Modell = lager;
        this.ausgewählteSaison = ausgewählteSaison;
        foreach (var reihe in lager.Reihen)
        {
            var reihenViewModel = new LagerreihenViewModel(reihe);

            foreach (var p in reihenViewModel.PlatzViewmodels)
            {
                p.Aktualisieren();
            }

            ReihenViewmodels.Add(reihenViewModel);
        }
    }
     

    public event PropertyChangedEventHandler? PropertyChanged;


    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
