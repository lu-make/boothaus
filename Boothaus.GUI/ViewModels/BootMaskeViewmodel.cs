using Boothaus.Domain;
using CommunityToolkit.Mvvm.Input;
using Domain.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace Boothaus.GUI.ViewModels;

public class BootMaskeViewmodel : INotifyPropertyChanged
{
    private LagerApplicationService appService;
    public bool? Ergebnis
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Ergebnis));
        }
    }
    public bool IstNeuesBoot { get; private set; } = false;

    public Boot? Boot { get; private set; }

    public string Kontakt 
    { 
        get; 
        set
        {
            field = value;
            OnPropertyChanged(nameof(Kontakt));
        } 
    }

    public bool KontaktValid
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged(nameof(KontaktValid));
            OnPropertyChanged(nameof(KontaktEditRahmenfarbe));
        }
    } = true;

    public string KontaktValidationMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(KontaktValidationMessage));
        }
    }

    public System.Windows.Media.Brush KontaktEditRahmenfarbe => KontaktValid ? System.Windows.Media.Brushes.LightGray : System.Windows.Media.Brushes.Red;

    public string Name
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public bool NameValid
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged(nameof(NameValid));
            OnPropertyChanged(nameof(NameEditRahmenfarbe));
        }
    } = true;

    public string NameValidationMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(NameValidationMessage));
        }
    } = "";
    public System.Windows.Media.Brush NameEditRahmenfarbe => NameValid ? System.Windows.Media.Brushes.LightGray : System.Windows.Media.Brushes.Red;


    public double Rumpflänge
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Rumpflänge));
        }
    }

    public double Breite
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Breite));
        }
    }

    public bool DimensionenValid
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged(nameof(DimensionenValid));
            OnPropertyChanged(nameof(DimensionenEditRahmenfarbe));
        }
    } = true;

    public string DimensionValidationMessage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(DimensionValidationMessage));
        }
    } = "";



    public System.Windows.Media.Brush DimensionenEditRahmenfarbe => DimensionenValid ? System.Windows.Media.Brushes.LightGray : System.Windows.Media.Brushes.Red;


    public ICommand OkCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }

    public BootMaskeViewmodel(LagerApplicationService appService) : this (appService, null)
    {
        IstNeuesBoot = true;
    }

    public BootMaskeViewmodel(LagerApplicationService appService, Boot? boot) 
    {

        this.appService = appService;
        Boot = boot;
        if (boot is not null)
        {
            Name = boot.Name;
            Kontakt = boot.Kontakt;
            Rumpflänge = boot.Rumpflänge;
            Breite = boot.Breite;
        }
        InitCommands();
    }

    private void InitCommands()
    {
        OkCommand = new RelayCommand(execute: () =>
        { 
            SachenValidieren();
            if (NameValid && KontaktValid && DimensionenValid)
            {
                if (IstNeuesBoot)
                {
                    var neuesBoot = new Boot(Name, Kontakt, Rumpflänge, Breite);
                    Boot = neuesBoot;
                }
                else
                {
                    Boot!.Name = Name;
                    Boot.Kontakt = Kontakt;
                    Boot.Rumpflänge = Rumpflänge;
                    Boot.Breite = Breite; 
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
        NameValidieren();
        KontaktValidieren();
        GrößeValidieren();
    }

    private void NameValidieren()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            NameValid = false;
            NameValidationMessage = "Pflichtfeld";
            return;
        }

        NameValid = true;
        NameValidationMessage = string.Empty;
    }

    private void KontaktValidieren()
    {
        if (string.IsNullOrWhiteSpace(Kontakt))
        {
            KontaktValid = false;
            KontaktValidationMessage = "Pflichtfeld";
            return;
        }
        KontaktValid = true;
        KontaktValidationMessage = string.Empty;
    }

    private void GrößeValidieren()
    {
        if (Rumpflänge <= 0 || Breite <= 0)
        {
            DimensionenValid = false;
            DimensionValidationMessage = "Rumpflänge und Breite müssen größer als 0 sein.";
            return;
        }
        DimensionenValid = true;
        DimensionValidationMessage = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

