using Boothaus.Domain;
using Boothaus.GUI.Services;
using CommunityToolkit.Mvvm.Input;
using Domain.Services; 
using System.Collections.ObjectModel;
using System.ComponentModel; 
using System.Windows.Input;

namespace Boothaus.GUI.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private IDialogService dialogService;
    private LagerApplicationService appService;

    // daten
    public ObservableCollection<Lagerauftrag> Aufträge { get; set; }
    public ObservableCollection<Saison> Saisons { get; set; }

    public Lagerauftrag? AusgewählterAuftrag
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(AusgewählterAuftrag));
            (AuftragBearbeitenCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (AuftragLöschenCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (AuftragLagerplatzZuweisenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }
    public Lagerplatz? AusgewählterPlatz
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(AusgewählterPlatz));
            (AuftragLagerplatzZuweisenCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (AuftragLagerplatzLösenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    public Saison AusgewählteSaison 
    {
        get; 
        set 
        {
            field = value;
            OnPropertyChanged(nameof(AusgewählteSaison));
            ApplyAuftragSaisonFilter();
            LagerViewModel.Update(AusgewählteSaison);
        } 
    }

    public LagerViewModel LagerViewModel
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(LagerViewModel));
        }
    }

    // commands 
    public ICommand AuftragErfassenCommand { get; private set; }
    public ICommand AuftragBearbeitenCommand { get; private set; }
    public ICommand AuftragLöschenCommand { get; private set; }

    public ICommand AuftragLagerplatzZuweisenCommand { get; private set; }
    public ICommand AuftragLagerplatzLösenCommand { get; private set; }

  //  public ICommand AufträgeNachSaisonFilternCommand { get; private set; }

    public ICommand LagereigenschaftenÖffnenCommand { get; private set; }
    
    public ICommand LagerkalenderErstellenCommand { get; private set; }
    
    public ICommand LagerreiheHinzufügenCommand { get; private set; }
    public ICommand LagerreiheEntfernenCommand { get; private set; }
    public ICommand BootAnlegenCommand { get; private set; }
    public ICommand BootBearbeitenCommand { get; private set; }
    public ICommand BootLöschenCommand { get; private set; }

    public ICommand AboutAnzeigenCommand { get; private set; }

    // events
    public event PropertyChangedEventHandler? PropertyChanged;

    // sub viewmodels 
    public MainViewModel(IDialogService dialogService, LagerApplicationService appService)
    {
        this.dialogService = dialogService;
        this.appService = appService;
        Aufträge = new ObservableCollection<Lagerauftrag>(appService.AlleAufträge());
        Saisons = new ObservableCollection<Saison>(appService.AlleSaisons());
        LagerViewModel = new LagerViewModel(appService.GetLager(), AusgewählteSaison); 
        AusgewählteSaison = Saisons.First();
        InitCommands();
    }

    private void InitCommands()
    {
        AuftragErfassenCommand = new RelayCommand(execute: () =>
        {
            var auftragmaskeResult = dialogService.AuftragErzeugen();
            if (auftragmaskeResult.Success)
            {
                var auftrag = auftragmaskeResult.Entity!;
                appService.ErfasseAuftrag(auftrag);
                Aufträge.Add(auftrag);
            }
            Saisons.Update(appService.AlleSaisons());
        }, canExecute: () => true);

        AuftragBearbeitenCommand = new RelayCommand(execute: () =>
        {
            var auftragmaskeResult = dialogService.AuftragBearbeiten(AusgewählterAuftrag!);
            if (auftragmaskeResult.Success)
            {
                var auftrag = auftragmaskeResult.Entity!; 
                UpdateAuftrag(auftrag);
            }
            Saisons.Update(appService.AlleSaisons());
        }, canExecute: () => AusgewählterAuftrag is not null);

        AuftragLöschenCommand = new RelayCommand(execute: () =>
        { 
            var result = dialogService.JaNeinWarnungDialogAnzeigen(titel: "Lagerauftrag löschen", frage: "Möchten Sie den ausgewählten Lagerauftrag wirklich löschen?");
            if (!result) return;
            appService.LöscheAuftrag(AusgewählterAuftrag);
            Aufträge.Remove(AusgewählterAuftrag!);

        }, canExecute: () => AusgewählterAuftrag is not null);

        AuftragLagerplatzZuweisenCommand = new RelayCommand(execute: () => 
        {
            AusgewählterAuftrag!.Platz = AusgewählterPlatz!;
            AusgewählterPlatz!.ZuweisungHinzufügen(AusgewählterAuftrag); 
            UpdateAuftrag(AusgewählterAuftrag);

        }, canExecute: () => appService.KannZuweisen(AusgewählterAuftrag, AusgewählterPlatz));

        AuftragLagerplatzLösenCommand = new RelayCommand(execute: () =>
        {
            AusgewählterPlatz!.ZuweisungEntfernen(AusgewählterAuftrag!);
            AusgewählterAuftrag!.Platz = null;
            UpdateAuftrag(AusgewählterAuftrag);
        }, canExecute: () => AusgewählterPlatz is not null);
         
        LagerkalenderErstellenCommand = new RelayCommand(execute: () =>
        {
            appService.ErstelleLagerkalender(AusgewählteSaison);
            LagerViewModel.Modell = appService.GetLager();
            LagerViewModel.Update(AusgewählteSaison);
        }, canExecute: () => true);
    }

    private void UpdateAuftrag(Lagerauftrag auftrag) 
    {
        appService.AktualisiereAuftrag(auftrag);
        var index = Aufträge.IndexOf(auftrag);
        Aufträge[index] = auftrag;
    }

    private void ApplyAuftragSaisonFilter()
    {
        Aufträge.Update(appService.AlleAufträgeInSaison(AusgewählteSaison));
    }
     

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
