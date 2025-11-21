using Boothaus.Domain;

namespace Boothaus.GUI.Services;

public interface IDialogService
{
    EingabemaskeResult<Auftrag> AuftragErzeugen();
    EingabemaskeResult<Auftrag> AuftragBearbeiten(Auftrag auftrag);
    bool BootErzeugen();
    bool BootBearbeiten(Boot boot);
    void AboutAnzeigen();
    bool JaNeinWarnungDialogAnzeigen(string titel, string frage);
}
