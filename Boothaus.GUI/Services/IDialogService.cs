using Boothaus.Domain;

namespace Boothaus.GUI.Services;

public interface IDialogService
{
    EingabemaskeResult<Auftrag> AuftragErfassen();
    EingabemaskeResult<Auftrag> AuftragBearbeiten(Auftrag auftrag);
    void BooteVerwalten();
    EingabemaskeResult<Boot> BootErfassen();
    EingabemaskeResult<Boot> BootBearbeiten(Boot boot);
    void AboutAnzeigen();
    bool JaNeinWarnungDialogAnzeigen(string titel, string frage);
    void OkWarnungDialogAnzeigen(string titel, string nachricht);
    void FehlermeldungAnzeigen(string nachricht);

    EingabemaskeResult<string> ImportAusDateiDialog();
    EingabemaskeResult<string> ExportInDateiDialog();
    void EinstellungenAnzeigen();
}
