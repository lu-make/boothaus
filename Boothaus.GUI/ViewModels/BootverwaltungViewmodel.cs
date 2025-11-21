using Boothaus.Domain;
using Boothaus.GUI.Services;
using CommunityToolkit.Mvvm.Input;
using Domain.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Boothaus.GUI.ViewModels;

public class BootverwaltungViewmodel : INotifyPropertyChanged
{
    private LagerApplicationService appService;
    private IDialogService dialogService;

    public ObservableCollection<Boot> Bootliste { get; private set; }
    public Boot? AusgewählterBootlisteneintrag 
    { 
        get; 
        set
        {
            field = value;
            OnPropertyChanged(nameof(AusgewählterBootlisteneintrag));
            (BootBearbeitenCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (BootLöschenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }


    public ICommand BootErfassenCommand { get; private set; }
    public ICommand BootBearbeitenCommand { get; private set; }
    public ICommand BootLöschenCommand { get; private set; }

    public BootverwaltungViewmodel(LagerApplicationService appService, IDialogService dialogService)
    {
        this.appService = appService;
        this.dialogService = dialogService;

        Bootliste = new(appService.AlleBoote());

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
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

