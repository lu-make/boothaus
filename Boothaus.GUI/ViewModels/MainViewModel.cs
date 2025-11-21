using Boothaus.Domain;
using Boothaus.GUI.Services;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Extensions;
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
    public ObservableCollection<AuftragListViewModel> AuftragListe { get; set; }
    public ObservableCollection<Saison> Saisons { get; set; }

    public AuftragListViewModel? AusgewählterAuftragListeneintrag
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(AusgewählterAuftragListeneintrag));
            (AuftragBearbeitenCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (AuftragLöschenCommand as RelayCommand)?.NotifyCanExecuteChanged(); 
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
     
    public ICommand InNächsteSaisonDuplizierenCommand { get; private set; }

    public ICommand LagereigenschaftenÖffnenCommand { get; private set; }
    
    public ICommand LagerkalenderErstellenCommand { get; private set; }
    
    public ICommand LagerreiheHinzufügenCommand { get; private set; }
    public ICommand LagerreiheEntfernenCommand { get; private set; }
    public ICommand BootAnlegenCommand { get; private set; }
    public ICommand BootBearbeitenCommand { get; private set; }
    public ICommand BootLöschenCommand { get; private set; }

    public ICommand AboutAnzeigenCommand { get; private set; }
    public ICommand ResetZuweisungenCommand { get; private set; }

    public ICommand BooteVerwaltenCommand { get; private set; }

    // events
    public event PropertyChangedEventHandler? PropertyChanged;

    // sub viewmodels 
    public MainViewModel(IDialogService dialogService, LagerApplicationService appService)
    {
        this.dialogService = dialogService;
        this.appService = appService;

        LagerViewModel = new LagerViewModel(appService.GetLager(), AusgewählteSaison); 

        AuftragListe = new ObservableCollection<AuftragListViewModel>(appService
            .AlleAufträge().Select(auftrag => new AuftragListViewModel(auftrag)));
        
        Saisons = new ObservableCollection<Saison>(appService.AlleSaisons());
        AusgewählteSaison = Saisons.First();

        InitCommands();
    }

    private void InitCommands()
    {
        AuftragErfassenCommand = new RelayCommand(execute: () =>
        {
            var auftragmaskeResult = dialogService.AuftragErfassen();
            if (auftragmaskeResult.Success)
            {
                var auftrag = auftragmaskeResult.Entity!;
                appService.ErfasseAuftrag(auftrag);
                AuftragListe.Add(new AuftragListViewModel(auftrag));
            }
            Saisons.Update(appService.AlleSaisons());
        });

        AuftragBearbeitenCommand = new RelayCommand(execute: () =>
        {
            var ausgewählterAuftrag = AusgewählterAuftragListeneintrag?.Modell; 
            var auftragmaskeResult = dialogService.AuftragBearbeiten(ausgewählterAuftrag!);
            if (auftragmaskeResult.Success)
            {
                var auftrag = auftragmaskeResult.Entity!; 
                UpdateAuftrag(auftrag);
            }
            Saisons.Update(appService.AlleSaisons());
        }, canExecute: () => AusgewählterAuftragListeneintrag is not null);

        AuftragLöschenCommand = new RelayCommand(execute: () =>
        {
            var ausgewählterAuftrag = AusgewählterAuftragListeneintrag?.Modell;
            var result = dialogService.JaNeinWarnungDialogAnzeigen(titel: "Lagerauftrag löschen", frage: "Möchten Sie den ausgewählten Lagerauftrag wirklich löschen?");
            if (!result) return;
            appService.LöscheAuftrag(ausgewählterAuftrag);
            AuftragListe.Remove(AusgewählterAuftragListeneintrag!);

        }, canExecute: () => AusgewählterAuftragListeneintrag is not null);
         
        LagerkalenderErstellenCommand = new RelayCommand(execute: () =>
        {
            //appService.ResetInSaison(AusgewählteSaison);
            appService.ErstelleLagerkalender(AusgewählteSaison);
            LagerViewModel.Modell = appService.GetLager();
            LagerViewModel.Update(AusgewählteSaison);
        });

        InNächsteSaisonDuplizierenCommand = new RelayCommand(execute: () =>
        {
            appService.DupliziereSaisonInNächsteSaison(AusgewählteSaison);
            Saisons.Update(appService.AlleSaisons());
            AuftragListe.Update(appService.AlleAufträgeInSaison(AusgewählteSaison).Select(auftrag => new AuftragListViewModel(auftrag)));
            AusgewählteSaison = Saisons.First(s => s.Anfangsjahr == AusgewählteSaison.Anfangsjahr + 1);

        });

        ResetZuweisungenCommand = new RelayCommand(execute: () =>
        {
            var ausgewählterAuftrag = AusgewählterAuftragListeneintrag?.Modell;
            var result = dialogService.JaNeinWarnungDialogAnzeigen(titel: "Lager zurücksetzen", frage: "Möchten Sie alle Lagerzuweisungen in der Saison zurücksetzen?");
            if (!result) return;
            appService.ResetInSaison(AusgewählteSaison);
            LagerViewModel.Update(AusgewählteSaison);
        });

        BooteVerwaltenCommand = new RelayCommand(execute: () =>
        {
            dialogService.BooteVerwalten();
            AuftragListe.Update(appService.AlleAufträgeInSaison(AusgewählteSaison).Select(auftrag => new AuftragListViewModel(auftrag)));
        });
    }

    private void UpdateAuftrag(Auftrag auftrag) 
    {
        appService.AktualisiereAuftrag(auftrag);
        var index = AuftragListe.FindIndex(avm => avm.Modell == auftrag);
        AuftragListe[index].Modell = auftrag;
    }

    private void ApplyAuftragSaisonFilter()
    {
        AuftragListe.Update(appService.AlleAufträgeInSaison(AusgewählteSaison).Select(auftrag => new AuftragListViewModel(auftrag)));
    }
     
    public IEnumerable<LagerplatzViewModel> FindeGültigePlätze(Auftrag auftrag)
    {
        var gültigePlätze = appService.FindeGültigePlätze(auftrag).ToList();
        return LagerViewModel.ReihenViewmodels
            .SelectMany(r => r.PlatzViewmodels)
            .Where(pvm => gültigePlätze.Contains(pvm.Modell)); 
    }

    public bool KannZuweisen(Auftrag auftrag, Lagerplatz platz)
    {
        return appService.KannZuweisen(auftrag, platz);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
