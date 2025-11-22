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
            (AufträgeLöschenCommand as RelayCommand)?.NotifyCanExecuteChanged(); 
        }
    } 

    public ObservableCollection<AuftragListViewModel> AusgewählteAuftragListeneinträge { get; set; }

    public Saison AusgewählteSaison 
    {
        get; 
        set 
        {
            field = value;
            OnPropertyChanged(nameof(AusgewählteSaison));
            RefreshAuftragliste();
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
    public ICommand AufträgeLöschenCommand { get; private set; }
    public ICommand InNächsteSaisonDuplizierenCommand { get; private set; }
    public ICommand EinstellungenAnzeigenCommand { get; private set; }
    public ICommand LagerkalenderErstellenCommand { get; private set; }
    public ICommand AboutAnzeigenCommand { get; private set; }
    public ICommand ResetZuweisungenCommand { get; private set; }
    public ICommand BooteVerwaltenCommand { get; private set; }
    public ICommand ImportCommand { get; private set; }
    public ICommand ExportCommand { get; private set; }
    public ICommand BeendenCommand { get; private set; }

    // events
    public event PropertyChangedEventHandler? PropertyChanged;

    // sub viewmodels 
    public MainViewModel(IDialogService dialogService, LagerApplicationService appService)
    {
        this.dialogService = dialogService;
        this.appService = appService; 

        LagerViewModel = new LagerViewModel(appService.GetLager()); 

        AuftragListe = new ObservableCollection<AuftragListViewModel>(appService
            .AlleAufträge().Select(auftrag => new AuftragListViewModel(auftrag)));

        AusgewählteAuftragListeneinträge = new ObservableCollection<AuftragListViewModel>();

        Saisons = new ObservableCollection<Saison>(appService.AlleSaisons());
        AusgewählteSaison = Saisons.First();

        InitCommands();
    }

    private void InitCommands()
    {
        AuftragErfassenCommand = new RelayCommand(execute: () =>
        {
            try
            {
                var auftragmaskeResult = dialogService.AuftragErfassen();
                if (auftragmaskeResult.Success)
                {
                    var auftrag = auftragmaskeResult.Value!;
                    appService.ErfasseAuftrag(auftrag);
                    AuftragListe.Add(new AuftragListViewModel(auftrag));
                }
                Saisons.Update(appService.AlleSaisons());
            }
            catch (Exception e)
            {
                dialogService.FehlermeldungAnzeigen($"Fehler beim Erfassen des Auftrags: {e.Message}");
            }
        });

        AuftragBearbeitenCommand = new RelayCommand(execute: () =>
        {
            try
            {
                var ausgewählterAuftrag = AusgewählterAuftragListeneintrag?.Modell;
                var auftragmaskeResult = dialogService.AuftragBearbeiten(ausgewählterAuftrag!);
                if (auftragmaskeResult.Success)
                {
                    var auftrag = auftragmaskeResult.Value!;
                    UpdateAuftrag(auftrag);
                }
                Saisons.Update(appService.AlleSaisons());
            }
            catch (Exception e)
            {
                dialogService.FehlermeldungAnzeigen($"Fehler beim Bearbeiten des Auftrags: {e.Message}");
            }
        }, canExecute: () => AusgewählteAuftragListeneinträge.Count == 1);

        AufträgeLöschenCommand = new RelayCommand(execute: () =>
        {
            try
            {
                var result = dialogService.JaNeinWarnungDialogAnzeigen(titel: "Lagerauftrag löschen", frage: "Möchten Sie die ausgewählten Lageraufträge wirklich löschen?");
                if (!result) return;

                foreach (var auftrag in AusgewählteAuftragListeneinträge.ToList())
                {
                    appService.LöscheAuftrag(auftrag.Modell);
                    AuftragListe.Remove(auftrag);
                }

            }
            catch (Exception e)
            {
                dialogService.FehlermeldungAnzeigen($"Fehler beim Löschen des Auftrags: {e.Message}");
            }

        }, canExecute: () => AusgewählteAuftragListeneinträge.Count >= 1);

        LagerkalenderErstellenCommand = new RelayCommand(execute: () =>
        {
            try
            {
                var alleBoote = appService.AlleAufträgeInSaison(AusgewählteSaison).Select(a => a.Boot);
                var lager = appService.GetLager();
                var passend = alleBoote.All(boot => lager.Passt(boot));

                if (!passend)
                {
                    dialogService.OkWarnungDialogAnzeigen("Lagerkalender", "Es gibt Aufträge mit Booten, die nicht in dieses Lager passen. Diese können nicht zugewiesen werden.");
                }

                var ergebnis = appService.ErstelleLagerkalender(AusgewählteSaison);
                LagerViewModel.Modell = lager;
                LagerViewModel.Update(AusgewählteSaison);
                RefreshAuftragliste();

                if (!ergebnis)
                {
                    dialogService.OkWarnungDialogAnzeigen("Lagerkalender erstellen", "Es konnte kein vollständiger Lagerkalender erstellt werden. Bitte überprüfen Sie die Lagerplatzzuweisungen.");
                }
            }
            catch (Exception e)
            {
                dialogService.FehlermeldungAnzeigen($"Fehler beim Erstellen des Lagerkalenders: {e.Message}");
            }
        });

        InNächsteSaisonDuplizierenCommand = new RelayCommand(execute: () =>
        {
            try
            {
                appService.DupliziereSaisonInNächsteSaison(AusgewählteSaison);
                Saisons.Update(appService.AlleSaisons());
                RefreshAuftragliste();
                AusgewählteSaison = Saisons.First(s => s.Anfangsjahr == AusgewählteSaison.Anfangsjahr + 1);
            }
            catch (Exception e)
            {
                dialogService.FehlermeldungAnzeigen($"Fehler beim Übertragen in die nächste Saison: {e.Message}");
            }

        });

        ResetZuweisungenCommand = new RelayCommand(execute: () =>
        {
            try
            {
                var ausgewählterAuftrag = AusgewählterAuftragListeneintrag?.Modell;
                var result = dialogService.JaNeinWarnungDialogAnzeigen(titel: "Lager zurücksetzen", frage: "Möchten Sie alle Lagerzuweisungen in der Saison zurücksetzen?");
                if (!result) return;
                appService.ResetInSaison(AusgewählteSaison);
                LagerViewModel.Update(AusgewählteSaison);
            }
            catch (Exception e)
            {
                dialogService.FehlermeldungAnzeigen($"Fehler beim Zurücksetzen: {e.Message}");
            }
        });

        BooteVerwaltenCommand = new RelayCommand(execute: () =>
        {
            try
            {
                dialogService.BooteVerwalten();
                RefreshAuftragliste();

            }
            catch (Exception e)
            {
                dialogService.FehlermeldungAnzeigen($"Fehler beim Öffnen der Bootverwaltung: {e.Message}");
            }
        });

        ImportCommand = new RelayCommand(execute: () =>
        {
            try
            {
                var pfadResult = dialogService.ImportAusDateiDialog();
                if (!pfadResult.Success) return;
                appService.DatenImportieren(pfadResult.Value!);
                Saisons.Update(appService.AlleSaisons());
                LagerViewModel.Modell = appService.GetLager();
                LagerViewModel.Update(AusgewählteSaison);
            }
            catch (Exception e)
            {
                dialogService.FehlermeldungAnzeigen($"Fehler beim Importieren: {e.Message}");
            }
        });

        ExportCommand = new RelayCommand(execute: () =>
        {
            try
            {
                var pfadResult = dialogService.ExportInDateiDialog();
                if (!pfadResult.Success) return;
                appService.DatenExportieren(pfadResult.Value!);
            }
            catch (Exception e)
            {
                dialogService.FehlermeldungAnzeigen($"Fehler beim Exportieren: {e.Message}");
            }
        });

        BeendenCommand = new RelayCommand(execute: () =>
        {
            Environment.Exit(0);
        });

        AboutAnzeigenCommand = new RelayCommand(execute: dialogService.AboutAnzeigen);


        EinstellungenAnzeigenCommand = new RelayCommand(execute: dialogService.EinstellungenAnzeigen);
    }

    private void UpdateAuftrag(Auftrag auftrag) 
    {
        appService.AktualisiereAuftrag(auftrag);
        var index = AuftragListe.FindIndex(avm => avm.Modell == auftrag);
        AuftragListe[index].Modell = auftrag;
    }

    public void RefreshAuftragliste()
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
