using Boothaus.Domain;
using Boothaus.GUI.ViewModels;
using Domain.Services;
using Microsoft.Win32;

namespace Boothaus.GUI.Services;

public class DialogService : IDialogService
{
    private readonly LagerApplicationService appService;

    public DialogService(LagerApplicationService appService)
    {
        this.appService = appService;
    }

    public EingabemaskeResult<Auftrag> AuftragErfassen()
    {
        var auftragMaskeVm = new AuftragMaskeViewmodel(appService, this);
        var dialog = new AuftragMaske(auftragMaskeVm);
        var result = dialog.ShowDialog();
        if (result == true)
        {
            return new EingabemaskeResult<Auftrag>
            {
                Success = true,
                Value = auftragMaskeVm.Auftrag
            };
        }
        else
        {
            return new EingabemaskeResult<Auftrag>
            {
                Success = false,
                Value = null
            };
        }
    }

    public EingabemaskeResult<Auftrag> AuftragBearbeiten(Auftrag auftrag)
    {
        var viewmodel = new AuftragMaskeViewmodel(appService, this, auftrag);
        var dialog = new AuftragMaske(viewmodel);
        var result = dialog.ShowDialog();
        if (result == true)
        {
            return new EingabemaskeResult<Auftrag>
            {
                Success = true,
                Value = viewmodel.Auftrag
            };
        }
        else
        {
            return new EingabemaskeResult<Auftrag>
            {
                Success = false,
                Value = null
            };
        }
    }

    public void BooteVerwalten()
    {
        var viewmodel = new BootverwaltungViewmodel(appService, this);
        var dialog = new Bootverwaltung(viewmodel);
        dialog.ShowDialog();
    }

    public EingabemaskeResult<Boot> BootErfassen()
    {
        var viewmodel = new BootMaskeViewmodel(appService);
        var dialog = new BootMaske(viewmodel);
        var result = dialog.ShowDialog();
        if (result == true)
        {
            return new EingabemaskeResult<Boot>
            {
                Success = true,
                Value = viewmodel.Boot
            };
        }
        else
        {
            return new EingabemaskeResult<Boot>
            {
                Success = false,
                Value = null
            };
        }
    }

    public EingabemaskeResult<Boot> BootBearbeiten(Boot boot)
    {
        var viewmodel = new BootMaskeViewmodel(appService, boot);
        var dialog = new BootMaske(viewmodel);
        var result = dialog.ShowDialog();
        if (result == true)
        {
            return new EingabemaskeResult<Boot>
            {
                Success = true,
                Value = viewmodel.Boot
            };
        }
        else
        {
            return new EingabemaskeResult<Boot>
            {
                Success = false,
                Value = null
            };
        }
    }

    public void AboutAnzeigen()
    {
        var about = new About();
        about.ShowDialog();
    }

    public bool JaNeinWarnungDialogAnzeigen(string titel, string frage)
    {
        var result = System.Windows.MessageBox.Show(frage, titel, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
        return result == System.Windows.MessageBoxResult.Yes;
    }

    public void OkWarnungDialogAnzeigen(string titel, string nachricht)
    {
        System.Windows.MessageBox.Show(nachricht, titel, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
    }

    public EingabemaskeResult<string> ImportAusDateiDialog()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Boothaus-Datensatz (*.boothaus.json)|*.boothaus.json",
            Title = "Daten aus Datei importieren"
        }; 

        var result = openFileDialog.ShowDialog();
        if (result == true) 
        {
            return new EingabemaskeResult<string>
            {
                Success = true,
                Value = openFileDialog.FileName
            };
        } 
        else
        {
            return new EingabemaskeResult<string>
            {
                Success = false,
                Value = null
            };
        } 
    }

    public EingabemaskeResult<string> ExportInDateiDialog()
    {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Boothaus-Datensatz (*.boothaus.json)|*.boothaus.json",
            Title = "Daten in Datei exportieren"
        };

        var result = saveFileDialog.ShowDialog();
        if (result == true)
        {
            return new EingabemaskeResult<string>
            {
                Success = true,
                Value = saveFileDialog.FileName
            };
        }
        else
        {
            return new EingabemaskeResult<string>
            {
                Success = false,
                Value = null
            };
        }
    }

    public void FehlermeldungAnzeigen(string nachricht)
    {
        System.Windows.MessageBox.Show(nachricht, "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
    }
}