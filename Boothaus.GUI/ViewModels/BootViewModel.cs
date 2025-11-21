using Boothaus.Domain;
using Boothaus.GUI.Services;
using CommunityToolkit.Mvvm.Input;
using Domain.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Boothaus.GUI.ViewModels;

public class BootMaskeViewmodel : INotifyPropertyChanged
{
    private LagerApplicationService appService;

    private bool istNeuesBoot = false;

    public Boot? Boot { get; }

    public ICommand OkCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }

    public BootMaskeViewmodel(LagerApplicationService appService) : this (appService, null)
    {
        istNeuesBoot = true;
    }

    public BootMaskeViewmodel(LagerApplicationService appService, Boot? boot) 
    {

        this.appService = appService;
        Boot = boot;
        InitCommands();
    }

    private void InitCommands()
    {

    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public class BootverwaltungViewmodel : INotifyPropertyChanged
{
    private LagerApplicationService appService;
    private IDialogService dialogService;

    public ObservableCollection<Boot> Bootliste { get; private set; }
    public Boot? AusgewählterBootlisteneintrag { 
        get; 
        private set
        {
            field = value;
            OnPropertyChanged(nameof(AusgewählterBootlisteneintrag));
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ICommand BootErfassenCommand;
    public ICommand BootBearbeitenCommand;
    public ICommand BootLöschenCommand;

    public BootverwaltungViewmodel(LagerApplicationService appService, IDialogService dialogService)
    {
        this.appService = appService;
        this.dialogService = dialogService;
        InitCommands();
    }

    private void InitCommands()
    {
        BootErfassenCommand = new RelayCommand(execute: () =>
        {
            var result = dialogService.BootErfassen();
            if (result.Success)
            {
                var boot = result.Entity!;
                appService.ErfasseBoot(boot);
                Bootliste.Add(boot);
            }
        });

        BootBearbeitenCommand = new RelayCommand(execute: () =>
        {
            var result = dialogService.BootBearbeiten(AusgewählterBootlisteneintrag!);
            if (result.Success)
            {
                var boot = result.Entity!;
                appService.UpdateBoot(boot);

                var index = Bootliste.IndexOf(AusgewählterBootlisteneintrag!);
                Bootliste[index] = boot;
            }

        }, canExecute: () => AusgewählterBootlisteneintrag is not null);

        BootLöschenCommand = new RelayCommand(execute: () =>
        {
            var result = dialogService.JaNeinWarnungDialogAnzeigen("Boot löschen", "Möchten Sie das ausgewählte Boot wirklich löschen? ACHTUNG! Alle Aufträge die mit diesem Boot zusammenhängen, werden ebenfalls gelöscht!");
            if (!result) return;
            appService.LöscheBoot(AusgewählterBootlisteneintrag!);
            Bootliste.Remove(AusgewählterBootlisteneintrag!);

        }, canExecute: () => AusgewählterBootlisteneintrag is not null);

    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public class BootlisteViewModel : INotifyPropertyChanged
{
    public Boot Modell { get; }
    public BootlisteViewModel(Boot boot)
    {
        Modell = boot;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}