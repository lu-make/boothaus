using Boothaus.Domain;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Boothaus.GUI.ViewModels;

public class LagerViewModel : INotifyPropertyChanged
{
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

    public ICommand LagerreiheHinzufügenCommand { get; private set; }
    public ICommand LagerreiheEntfernenCommand { get; private set; }
    public LagerViewModel(Lager lager)
    {
        Modell = lager;
        foreach (var reihe in lager.Reihen)
        {
            var reihenViewModel = new LagerreihenViewModel(reihe);

            foreach (var p in reihenViewModel.PlatzViewmodels)
            {
                p.Aktualisieren();
            }

            ReihenViewmodels.Add(reihenViewModel);
        }
        InitCommands();
    }

    private void InitCommands()
    {
        LagerreiheHinzufügenCommand = new RelayCommand(execute: () =>
        {
            var neueReihe = new Lagerreihe(AnzahlLagerreihen);
            neueReihe.Lager = Modell;
            for (int i = 0; i < 10; i++)
            {
                neueReihe.PlatzHinzufügen(new Lagerplatz());
            }
            Modell.Reihen.Add(neueReihe);
            var reihenViewmodel = new LagerreihenViewModel(neueReihe);
            ReihenViewmodels.Add(reihenViewmodel);
            OnPropertyChanged(nameof(AnzahlLagerreihen));
            (LagerreiheEntfernenCommand as RelayCommand)?.NotifyCanExecuteChanged();

        });

        LagerreiheEntfernenCommand = new RelayCommand(execute: () =>
        {
            var letzteReihe = ReihenViewmodels.Last();

            foreach (var platz in letzteReihe.PlatzViewmodels)
            {
                platz.Modell.ZuweisungenLeeren();
            }

            Modell.Reihen.Remove(letzteReihe.Modell);
            ReihenViewmodels.Remove(letzteReihe);
            OnPropertyChanged(nameof(AnzahlLagerreihen));
            (LagerreiheEntfernenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }, canExecute: () => Modell.Reihen.Count > 1);
    }

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
        OnPropertyChanged(nameof(AnzahlLagerreihen));
    }



    public event PropertyChangedEventHandler? PropertyChanged;


    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
