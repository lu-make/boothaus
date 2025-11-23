using Boothaus.Domain;
using Boothaus.GUI.Services;
using CommunityToolkit.Mvvm.Input;
using Domain.Services;
using System.Collections.ObjectModel;
using System.ComponentModel; 
using System.Windows.Input; 
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace Boothaus.GUI.ViewModels;

public partial class AuftragMaskeViewmodel : INotifyPropertyChanged
{
    private Lager lager;
    private LagerApplicationService appService;
    private IDialogService dialogService;

    public bool? Ergebnis
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Ergebnis));
        }
    }

    public Boot? Boot
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Boot));
        }
    }

    public bool BootValid { 
        get; 
        private set 
        {
            field = value;
            OnPropertyChanged(nameof(BootValid));
            OnPropertyChanged(nameof(BootauswahlRahmenfarbe));
        } 
    } = true;

    public string BootValidationMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(BootValidationMessage));
        }
    } = "";

    public Brush BootauswahlRahmenfarbe => BootValid? Brushes.Gray : Brushes.Red;
    
    public bool DatumspaarValid
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged(nameof(DatumspaarValid));
            OnPropertyChanged(nameof(DatumauswahlRahmenfarbe));
        }
    } = true;

    public string DatumspaarValidationMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(DatumspaarValidationMessage));
        }
    } = "";

    public Brush DatumauswahlRahmenfarbe => DatumspaarValid? Brushes.Gray : Brushes.Red;

    public DateOnly? Von
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Von));
        }
    }

    public DateOnly? Bis
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Bis));
        }
    }

    public Auftrag? Auftrag { get; set; }
    public bool IstNeuerAuftrag { get; private set; }

    public ObservableCollection<Boot> AlleBoote { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand NeuesBootCommand { get; private set; }
    public ICommand OkCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }

    public AuftragMaskeViewmodel(LagerApplicationService appService, IDialogService dialogService)
        : this(appService, dialogService, null)
    {
        IstNeuerAuftrag = true;
    }

    public AuftragMaskeViewmodel(LagerApplicationService appService, IDialogService dialogService, Auftrag? auftrag)
    {
        this.appService = appService;
        this.dialogService = dialogService;
        lager = appService.GetLager();
        AlleBoote = new ObservableCollection<Boot>(appService.AlleBoote());
        if (auftrag is not null)
        {
            Auftrag = auftrag;
            Boot = auftrag.Boot; 
            Von = auftrag.Von;
            Bis = auftrag.Bis;
        }
        InitCommands();
    }

    private void InitCommands()
    {
        NeuesBootCommand = new RelayCommand(execute: () =>
        {
            var result = dialogService.BootErfassen();
            if (result.Success)
            {
                var boot = result.Value!;
                appService.ErfasseBoot(boot);
                AlleBoote.Add(boot);
                Boot = boot;
            } 
        });

        OkCommand = new RelayCommand(execute: () => 
        {
            SachenValidieren();
            if (BootValid && DatumspaarValid)
            {  
                if (IstNeuerAuftrag)
                {
                    Auftrag = new Auftrag(lager, Boot!, Von!.Value, Bis!.Value);
                } 
                else
                {
                    Auftrag!.Boot = Boot!;
                    Auftrag.Von = Von!.Value;
                    Auftrag.Bis = Bis!.Value;
                }
                Ergebnis = true;
            }
        });

        CancelCommand = new RelayCommand(execute: () =>
        {
            Ergebnis = false;
        });
    }

    private void SachenValidieren()
    {
        BootValidieren();
        DatenValidieren();
        ValidiereAuftragKonfliktfrei();
    }

    private void BootValidieren()
    {
        if (Boot is null)
        {
            BootValid = false;
            BootValidationMessage = "Pflichtfeld";
            return;
        }

        if (!lager.Passt(Boot)) 
        {
            BootValid = false;
            BootValidationMessage = "Das Boot passt nicht ins Lager";
        }

        BootValid = true;
        BootValidationMessage = string.Empty;
    }


    private void DatenValidieren()
    {
        if (!Von.HasValue || !Bis.HasValue || !Auftrag.IstGültigesDatumspaar(Von!.Value, Bis!.Value))
        {
            DatumspaarValidationMessage = "Sie müssen einen gültigen Zeitraum auswählen."; 
            DatumspaarValid = false;
            return;
        }

        if (!IstNeuerAuftrag 
            && Auftrag!.Platz is Lagerplatz platz 
            && !platz.Reihe.IstZeitraumErlaubtAnIndex(Auftrag.Saison, Von!.Value, Bis!.Value, platz.Reihe.Index(platz)))
        {
            DatumspaarValidationMessage = "Der Auftrag hat schon eine Platzzuweisung. Für diesen Platz ist der angegebene Zeitraum ungültig.";
            DatumspaarValid = false;
            return;
        }

        DatumspaarValid = true;
        DatumspaarValidationMessage = string.Empty;
    }

     
    private void ValidiereAuftragKonfliktfrei()
    {
        if (!BootValid || !DatumspaarValid) return;


        if (IstNeuerAuftrag && appService.BootAuftragExistiertBereits(Boot!, Von!.Value, Bis!.Value))
        {
            BootValid = false;
            DatumspaarValid = false;
            DatumspaarValidationMessage = "Das Boot hat bereits einen Lagerauftrag in dem angegebenen Zeitraum.";
        }
    }

    private void OnPropertyChanged(string propertyName)
    { 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}