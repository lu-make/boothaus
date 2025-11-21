using Boothaus.Domain;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Boothaus.GUI.ViewModels;

 
public class LagerreihenViewModel
{
    public Lagerreihe Modell { get; }
    public int Displaynummer => Modell.Nummer + 1;
    public ObservableCollection<LagerplatzViewModel> PlatzViewmodels { get; } = new(); 

    public ICommand LagerplatzHinzufügenCommand;
    public ICommand LagerplatzEntfernenCommand;

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
        LagerplatzHinzufügenCommand = new RelayCommand(execute: () =>
        {
            var neuerPlatz = new Lagerplatz();
            Modell.PlatzHinzufügen(neuerPlatz);
            PlatzViewmodels.Add(new LagerplatzViewModel(neuerPlatz));
        },
        canExecute: () => PlatzViewmodels.Count <= 10);

        LagerplatzEntfernenCommand = new RelayCommand(execute: () =>
        {
            var letzterPlatz = PlatzViewmodels.Last();
            letzterPlatz.Modell.ZuweisungenLeeren();
            Modell.PlatzEntfernen(letzterPlatz.Modell);
            PlatzViewmodels.Remove(letzterPlatz);
        }, 
        canExecute: () => PlatzViewmodels.Count > 1);

    }
}
