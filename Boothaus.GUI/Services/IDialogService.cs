using Boothaus.Domain;
using Domain.Services;

namespace Boothaus.GUI.Services;

public interface IDialogService
{
    EingabemaskeResult<Lagerauftrag> AuftragErzeugen();
    EingabemaskeResult<Lagerauftrag> AuftragBearbeiten(Lagerauftrag auftrag);

    bool BootErzeugen();
    bool BootBearbeiten(Boot boot);

    void AboutAnzeigen();

    bool JaNeinWarnungDialogAnzeigen(string titel, string frage);
}

public class EingabemaskeResult<T>
{
    public bool Success { get; set; }
    public T? Entity { get; set; }
}

public class DialogService : IDialogService
{
    private readonly LagerApplicationService appService;

    public DialogService(LagerApplicationService appService)
    {
        this.appService = appService;
    }

    public EingabemaskeResult<Lagerauftrag> AuftragErzeugen()
    {
        var dialog = new AuftragMaske(appService);
        var result = dialog.ShowDialog();
        if (result == true)
        {
            return new EingabemaskeResult<Lagerauftrag>
            {
                Success = true,
                Entity = dialog.Auftrag
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
        var dialog = new AuftragMaske(appService, auftrag);
        var result = dialog.ShowDialog();
        if (result == true)
        {
            return new EingabemaskeResult<Lagerauftrag>
            {
                Success = true,
                Entity = dialog.Auftrag
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