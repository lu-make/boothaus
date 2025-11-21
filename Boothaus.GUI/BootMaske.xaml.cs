using Boothaus.GUI.ViewModels;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for BootMaske.xaml
/// </summary>
public partial class BootMaske : ThemedWindow
{ 
    public BootMaske(BootMaskeViewmodel viewmodel)
    {
        InitializeComponent();
        DataContext = viewmodel;

        Title = viewmodel.IstNeuesBoot ? "Boot erfassen" : "Boot bearbeiten"; 
        viewmodel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewmodel.Ergebnis) && viewmodel.Ergebnis.HasValue)
                DialogResult = viewmodel.Ergebnis;
        };
    }
}
