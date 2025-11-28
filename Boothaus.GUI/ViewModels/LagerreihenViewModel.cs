using Boothaus.Domain;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Boothaus.GUI.ViewModels;

 
public class LagerreihenViewModel : INotifyPropertyChanged
{
    public Lagerreihe Modell { 
        get; 
        set 
        {
            field = value;
            OnPropertyChanged(nameof(Modell));
        } 
    }
    public int Displaynummer => Modell.Nummer + 1;
    public ObservableCollection<LagerplatzViewModel> PlatzViewmodels { get; } = new(); 

    public Saison AusgewählteSaison
    {
        get;
        set
        {
            field = value;
            foreach (var platzVm in PlatzViewmodels)
            {
                platzVm.AusgewählteSaison = value;
            }
            OnPropertyChanged(nameof(AusgewählteSaison));
        }
    }

    public ICommand PlatzInReiheHinzufügenCommand { get; private set; }
    public ICommand PlatzAusReiheEntfernenCommand { get; private set; }


    public LagerreihenViewModel(Lagerreihe reihe)
    {
        Modell = reihe;
        foreach (var platz in reihe.Plätze)
        {
            PlatzViewmodels.Add(new LagerplatzViewModel(platz));
        }
        InitCommands();
    }

    private void InitCommands()
    {
        PlatzInReiheHinzufügenCommand = new RelayCommand(execute: () =>
        {
            var neuerPlatz = new Lagerplatz(Modell);
            Modell.PlatzHinzufügen(neuerPlatz);
            PlatzViewmodels.Add(new LagerplatzViewModel(neuerPlatz));
            (PlatzInReiheHinzufügenCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (PlatzAusReiheEntfernenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        },
        canExecute: () => PlatzViewmodels.Count < Constants.MaxPlätzeProReihe);

        PlatzAusReiheEntfernenCommand = new RelayCommand(execute: () =>
        {
            var letzterPlatz = PlatzViewmodels.Last();
            letzterPlatz.Modell.ZuweisungenLeeren();
            Modell.PlatzEntfernen(letzterPlatz.Modell);
            PlatzViewmodels.Remove(letzterPlatz);
            (PlatzInReiheHinzufügenCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (PlatzAusReiheEntfernenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }, 
        canExecute: () => PlatzViewmodels.Count > Constants.MinPlätzeProReihe);

    }

    public void Aktualisieren()
    {
        foreach (var platzVm in PlatzViewmodels)
        {
            platzVm.Aktualisieren();
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
