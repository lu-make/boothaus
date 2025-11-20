using Boothaus.Domain;
using System.ComponentModel;
using System.Windows.Media;

namespace Boothaus.GUI.ViewModels;

public class LagerplatzViewModel : INotifyPropertyChanged
{
    private Lagerauftrag? nächsteZuweisung;

    public LagerplatzViewModel(Lagerplatz platz)
    {
        Modell = platz;
    }

    public Lagerplatz Modell { get; }

    public Saison AusgewählteSaison
    {
        get; 
        set
        {
            field = value;
            Aktualisieren();
        }
    }

    public bool IstAusgewählt { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Anzeigetext
    {
        get
        {
            if (HatNächsteZuweisungInSaison)
            {
                return $"{nächsteZuweisung!.Boot}\n{nächsteZuweisung.Von:dd.MM.yyyy} - {nächsteZuweisung.Bis:dd.MM.yyyy}";
            } 

            return "Frei";
        }
    }

    public bool HatNächsteZuweisungInSaison => nächsteZuweisung is not null && nächsteZuweisung.Saison.Equals(AusgewählteSaison);

    public System.Windows.Media.Brush Hintergrundfarbe
    {
        get
        {
            if (HatNächsteZuweisungInSaison)
            {
                return new SolidColorBrush(Colors.PeachPuff);
            }

            return new SolidColorBrush(Colors.PaleGreen);
        }
    }

    public System.Windows.Media.Brush Border => IstAusgewählt
        ? new SolidColorBrush(Colors.Blue)
        : new SolidColorBrush(Colors.Transparent);

    public void Aktualisieren()
    { 
        var heute = DateOnly.FromDateTime(DateTime.Now);

        nächsteZuweisung = Modell.Zuweisungen
            .Where(z => z.Bis >= heute)
            .OrderBy(z => z.Von)
            .FirstOrDefault();

        OnPropertyChanged(nameof(AusgewählteSaison));
        OnPropertyChanged(nameof(HatNächsteZuweisungInSaison));
        OnPropertyChanged(nameof(Anzeigetext));
        OnPropertyChanged(nameof(Hintergrundfarbe));

    }


    public bool KannAuftragZuweisen(Lagerauftrag auftrag)
    {
        return Modell.IstFreiImZeitraum(auftrag.Von, auftrag.Bis);
    }

    public void AuftragZuweisen(Lagerauftrag auftrag)
    {
        Modell.ZuweisungHinzufügen(auftrag);
    }
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
