using Boothaus.Domain;
using Boothaus.GUI.ViewModels;
using Domain.Services;
using System.Windows;
using System.Windows.Media;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for AuftragMaske.xaml
/// </summary>
public partial class AuftragMaske : Window
{
    public AuftragMaske(AuftragMaskeViewmodel viewmodel)
    {
        InitializeComponent();
        DataContext = viewmodel;
        Title = viewmodel.IstNeuerAuftrag ? "Auftrag erfassen" : "Auftrag bearbeiten";
        viewmodel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewmodel.Ergebnis) && viewmodel.Ergebnis.HasValue)
                DialogResult = viewmodel.Ergebnis;
        };
    }
}
