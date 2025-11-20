using Boothaus.Domain;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Boothaus.GUI.ViewModels;

public class LagerreihenViewModel
{
    public Lagerreihe Modell { get; }
    public ObservableCollection<LagerplatzViewModel> PlätzeVms { get; } = new();


    public ICommand LagerplatzHinzufügenCommand;
    public ICommand LagerplatzEntfernenCommand;

    public LagerreihenViewModel(Lagerreihe reihe)
    {
        Modell = reihe;
        foreach (var platz in reihe.Plätze)
        {
            PlätzeVms.Add(new LagerplatzViewModel(platz));
        }
        InitCommands();
    }

    private void InitCommands()
    {
        LagerplatzHinzufügenCommand = new RelayCommand(execute: () =>
        {
            var neuerPlatz = new Lagerplatz();
            Modell.PlatzHinzufügen(neuerPlatz);
            PlätzeVms.Add(new LagerplatzViewModel(neuerPlatz));
        },
        canExecute: () => PlätzeVms.Count <= 10);

        LagerplatzEntfernenCommand = new RelayCommand(execute: () =>
        {
            var letzterPlatz = PlätzeVms.Last();
            letzterPlatz.Modell.ZuweisungenLeeren();
            Modell.PlatzEntfernen(letzterPlatz.Modell);
            PlätzeVms.Remove(letzterPlatz);
        }, 
        canExecute: () => PlätzeVms.Count > 1);

    }
}
