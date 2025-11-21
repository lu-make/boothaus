using Boothaus.Domain;
using System.ComponentModel; 
using System.Windows.Media; 

namespace Boothaus.GUI.ViewModels;

public class LagerplatzViewModel : INotifyPropertyChanged
{
    public Auftrag? NächsteZuweisung 
    { 
        get; 
        private set;  
    }

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
    public bool IstGültigesDropZiel
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IstGültigesDropZiel));
            OnPropertyChanged(nameof(Border));
            OnPropertyChanged(nameof(BorderThickness));
        }
    }

    public bool IstDragDropAktiv
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IstGültigesDropZiel));
            OnPropertyChanged(nameof(Border));
            OnPropertyChanged(nameof(BorderThickness));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Anzeigetext
    {
        get
        {
            if (HatNächsteZuweisungInSaison)
            {
                return $"{NächsteZuweisung!.Boot}\n{NächsteZuweisung.Von:dd.MM.yyyy} - {NächsteZuweisung.Bis:dd.MM.yyyy}";
            }

            return "Frei";
        }
    }

    public bool HatNächsteZuweisungInSaison => NächsteZuweisung is not null && NächsteZuweisung.Saison.Equals(AusgewählteSaison);

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

    public System.Windows.Media.Brush Border
    {
        get
        {
            var brush = new SolidColorBrush();
            brush.Opacity = 1;
             
            if (IstDragDropAktiv && IstGültigesDropZiel)
            {
                brush.Color = Colors.Blue;
                return brush;    
            }
             
            
            brush.Color = Colors.Gray;
            return brush;
        }
    }

    public int BorderThickness
    {
        get
        {
            if (IstDragDropAktiv && IstGültigesDropZiel)
            {
                return 5;
            }
            return 1;
        }
    }
     
    public void Aktualisieren()
    { 
        var heute = DateOnly.FromDateTime(DateTime.Now);

        NächsteZuweisung = Modell.Zuweisungen
            .Where(z => z.Saison.Equals(AusgewählteSaison))
            .OrderBy(z => z.Von)
            .FirstOrDefault();

        OnPropertyChanged(nameof(AusgewählteSaison));
        OnPropertyChanged(nameof(HatNächsteZuweisungInSaison));
        OnPropertyChanged(nameof(Anzeigetext));
        OnPropertyChanged(nameof(Hintergrundfarbe));

    }


    public bool InZeitraumKeinAuftrag(Auftrag auftrag)
    {
        return Modell.IstFreiImZeitraum(auftrag.Von, auftrag.Bis);
    }

    public void AuftragZuweisen(Auftrag auftrag)
    {
        Modell.ZuweisungHinzufügen(auftrag);
    }
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
