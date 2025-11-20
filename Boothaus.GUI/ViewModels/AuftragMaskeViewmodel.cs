using Boothaus.Domain; 
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
    private LagerApplicationService service;

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

    public Lagerauftrag? Auftrag { get; set; }
    private bool istNeuerAuftrag;

    public ObservableCollection<Boot> AlleBoote { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand OkCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }

    public AuftragMaskeViewmodel(LagerApplicationService service)
        : this(service, null)
    {
        istNeuerAuftrag = true;
    }

    public AuftragMaskeViewmodel(LagerApplicationService service, Lagerauftrag? auftrag)
    {
        this.service = service;
        lager = service.GetLager();
        AlleBoote = new ObservableCollection<Boot>(service.AlleBoote());
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
        OkCommand = new RelayCommand(execute: () => 
        {
            SachenValidieren();
            if (BootValid && DatumspaarValid)
            {  
                if (istNeuerAuftrag)
                {
                    Auftrag = new Lagerauftrag(lager, Boot!, Von!.Value, Bis!.Value);
                } 
                else
                {
                    Auftrag!.Boot = Boot!;
                    Auftrag.Von = Von!.Value;
                    Auftrag.Bis = Bis!.Value;
                }
                Ergebnis = true;
            }
        }, 
        canExecute: () => true);

        CancelCommand = new RelayCommand(execute: () =>
        {
            Ergebnis = false;
        }, 
        canExecute: () => true);
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
        if (!Von.HasValue || !Bis.HasValue || !Lagerauftrag.IstGültigesDatumspaar(Von!.Value, Bis!.Value))
        {
            DatumspaarValidationMessage = "Sie müssen einen gültigen Zeitraum auswählen."; 
            DatumspaarValid = false;
            return;
        }
        DatumspaarValid = true;
        DatumspaarValidationMessage = string.Empty;
    }


    private void ValidiereAuftragKonfliktfrei()
    {
        if (!BootValid || !DatumspaarValid) return;


        if (istNeuerAuftrag && service.BootAuftragExistiertBereits(Boot!, Von!.Value, Bis!.Value))
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