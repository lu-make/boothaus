using Boothaus.Domain;
using DevExpress.Mvvm.DataAnnotations;
using System.ComponentModel;

namespace Boothaus.GUI.ViewModels;

public class AuftragListViewModel : INotifyPropertyChanged
{
    public AuftragStatusZelle Status => Modell.Platz is null ? AuftragStatusZelle.Offen : AuftragStatusZelle.Zugewiesen;
    public Auftrag Modell 
    { 
        get; 
        set
        {
            field = value;
            OnPropertyChanged(nameof(Modell));
            OnPropertyChanged(nameof(Status));
        }
    }
     
    public AuftragListViewModel(Auftrag auftrag)
    {
        Modell = auftrag;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public enum AuftragStatusZelle
{
    [DXImage("Icons/bullet_green.png")]
    Zugewiesen,
    [DXImage("Icons/bullet_yellow.png")]
    Offen,
}