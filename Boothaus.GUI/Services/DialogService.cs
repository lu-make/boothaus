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

    public EingabemaskeResult<Lagerauftrag> AuftragErzeugen()
    {
        var auftragMaskeVm = new AuftragMaskeViewmodel(appService);
        var dialog = new AuftragMaske(auftragMaskeVm);
        var result = dialog.ShowDialog();
        if (result == true)
        {
            return new EingabemaskeResult<Lagerauftrag>
            {
                Success = true,
                Entity = auftragMaskeVm.Auftrag
            };
        }
        else
        {
            return new EingabemaskeResult<Lagerauftrag>
            {
                Success = false,
                Entity = null
            };
        }
    }

    public EingabemaskeResult<Lagerauftrag> AuftragBearbeiten(Lagerauftrag auftrag)
    {
        var viewmodel = new AuftragMaskeViewmodel(appService, auftrag);
        var dialog = new AuftragMaske(viewmodel);
        var result = dialog.ShowDialog();
        if (result == true)
        {
            return new EingabemaskeResult<Lagerauftrag>
            {
                Success = true,
                Entity = viewmodel.Auftrag
            };
        }
        else
        {
            return new EingabemaskeResult<Lagerauftrag>
            {
                Success = false,
                Entity = null
            };
        }
    }

    public bool BootErzeugen()
    {
        throw new NotImplementedException();
    }

    public bool BootBearbeiten(Boot boot)
    { 
        throw new NotImplementedException();
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
}