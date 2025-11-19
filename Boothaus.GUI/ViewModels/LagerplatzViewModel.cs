using Boothaus.Domain;
using System.ComponentModel;

namespace Boothaus.GUI.ViewModels;

public class LagerplatzViewModel : INotifyPropertyChanged
{
    public Lagerplatz Modell { get; }
    public LagerplatzViewModel(Lagerplatz platz)
    {
        Modell = platz;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Anzeigetext
    {
        get
        {
            if (NächsteZuweisung is not null)
            {
                return $"{NächsteZuweisung.Auftrag.Boot}\n{NächsteZuweisung.Auftrag.Von:dd.MM.yyyy} - {NächsteZuweisung.Auftrag.Bis:dd.MM.yyyy}";
            } 
            return "Frei";
        }
    }

    public Color Hintergrundfarbe
    {
        get
        {
            if (NächsteZuweisung is not null)
            {
                return Color.PeachPuff;
            }

            return Color.PaleGreen; 
        }
    }

    public LagerplatzZuweisung? NächsteZuweisung;

    public void Aktualisieren()
    { 
        var heute = DateOnly.FromDateTime(DateTime.Now);

        NächsteZuweisung = Modell.Zuweisungen
            .Where(z => z.Auftrag.Bis >= heute)
            .OrderBy(z => z.Auftrag.Von)
            .FirstOrDefault();
         
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Anzeigetext)));
    }
}
