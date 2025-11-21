using Boothaus.Domain;
using System.ComponentModel;

namespace Boothaus.GUI.ViewModels;

public class AuftragListViewModel : INotifyPropertyChanged
{
    public Auftrag Modell 
    { 
        get; 
        set
        {
            field = value;
            OnPropertyChanged(nameof(Modell));
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