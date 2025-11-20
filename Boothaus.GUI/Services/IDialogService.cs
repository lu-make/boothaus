using Boothaus.Domain;

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
