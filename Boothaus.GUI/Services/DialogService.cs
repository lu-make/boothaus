using Boothaus.Domain;
using Boothaus.GUI.ViewModels;
using Domain.Services;

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
                Entity = auftragMaskeVm.Auftrag
            };
        }
        else
        {
            return new EingabemaskeResult<Auftrag>
            {
                Success = false,
                Entity = null
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
                Entity = viewmodel.Auftrag
            };
        }
        else
        {
            return new EingabemaskeResult<Auftrag>
            {
                Success = false,
                Entity = null
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
                Entity = viewmodel.Boot
            };
        }
        else
        {
            return new EingabemaskeResult<Boot>
            {
                Success = false,
                Entity = null
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
                Entity = viewmodel.Boot
            };
        }
        else
        {
            return new EingabemaskeResult<Boot>
            {
                Success = false,
                Entity = null
            };
        }
    }

    public void AboutAnzeigen()
    { 
        throw new NotImplementedException();
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
}