using Boothaus.Domain;
using CommunityToolkit.Mvvm.Input;
using Domain.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace Boothaus.GUI.ViewModels;

public class EinstellungenViewModel : INotifyPropertyChanged
{
    private readonly LagerApplicationService appService;

    public event PropertyChangedEventHandler? PropertyChanged;

    private Lager lager;

    public decimal MeineMaxBreite { 
        get; 
        set
        {
            field = value;
            OnPropertyChanged(nameof(MeineMaxBreite));
            (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }
    public decimal MeineMaxLänge
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(MeineMaxLänge));
            (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    public ICommand OkCommand { get; private set; }
    public ICommand ApplyCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public bool? Ergebnis
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Ergebnis));
        }
    }

    private bool hatÄnderungen => MeineMaxBreite != lager.MaxBootBreite || MeineMaxLänge != lager.MaxBootLänge;

    public EinstellungenViewModel(LagerApplicationService appService)
    { 
        this.lager = appService.GetLager();
        MeineMaxBreite = lager.MaxBootBreite;
        MeineMaxLänge = lager.MaxBootLänge;
        this.appService = appService;
        InitCommands();
    }

    private void InitCommands()
    {
        OkCommand = new RelayCommand(execute: () =>
        {
            Übernehmen();
            Ergebnis = true;
        });

        ApplyCommand = new RelayCommand(execute: Übernehmen, canExecute: () => hatÄnderungen);

        CancelCommand = new RelayCommand(execute: () =>
        {
            Ergebnis = false;
        });
    }

    private void Übernehmen()
    {
        lager.MaxBootBreite = MeineMaxBreite;
        lager.MaxBootLänge = MeineMaxLänge;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}